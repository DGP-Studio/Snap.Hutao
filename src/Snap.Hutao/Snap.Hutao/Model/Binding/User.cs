// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Model.Binding;

/// <summary>
/// 用于视图绑定的用户
/// </summary>
public class User : Observable
{
    private readonly EntityUser inner;

    private UserGameRole? selectedUserGameRole;
    private bool isInitialized;

    /// <summary>
    /// 构造一个新的绑定视图用户
    /// </summary>
    /// <param name="user">用户实体</param>
    private User(EntityUser user)
    {
        inner = user;
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public UserInfo? UserInfo { get; private set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    public List<UserGameRole> UserGameRoles { get; private set; } = default!;

    /// <summary>
    /// 用户信息
    /// </summary>
    public UserGameRole? SelectedUserGameRole
    {
        get => selectedUserGameRole;
        private set => Set(ref selectedUserGameRole, value);
    }

    /// <inheritdoc cref="EntityUser.IsSelected"/>
    public bool IsSelected
    {
        get => inner.IsSelected;
        set => inner.IsSelected = value;
    }

    /// <inheritdoc cref="EntityUser.Cookie"/>
    public string? Cookie
    {
        get => inner.Cookie;
        set => inner.Cookie = value;
    }

    /// <summary>
    /// 内部的用户实体
    /// </summary>
    public EntityUser Entity { get => inner; }

    /// <summary>
    /// 是否初始化完成
    /// </summary>
    public bool IsInitialized { get => isInitialized; }

    /// <summary>
    /// 将cookie的字符串形式转换为字典
    /// </summary>
    /// <param name="cookie">cookie的字符串形式</param>
    /// <returns>包含cookie信息的字典</returns>
    public static IDictionary<string, string> MapCookie(string cookie)
    {
        SortedDictionary<string, string> cookieDictionary = new();

        string[] values = cookie.TrimEnd(';').Split(';');
        foreach (string[] parts in values.Select(c => c.Split('=', 2)))
        {
            string cookieName = parts[0].Trim();
            string cookieValue = parts.Length == 1 ? string.Empty : parts[1].Trim();

            cookieDictionary.Add(cookieName, cookieValue);
        }

        return cookieDictionary;
    }

    /// <summary>
    /// 从数据库恢复用户
    /// </summary>
    /// <param name="inner">数据库实体</param>
    /// <param name="userClient">用户客户端</param>
    /// <param name="userGameRoleClient">角色客户端</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户是否初始化完成，若Cookie失效会返回 <see langword="false"/> </returns>
    internal static async Task<User?> ResumeAsync(
        EntityUser inner,
        UserClient userClient,
        BindingClient userGameRoleClient,
        CancellationToken token = default)
    {
        User user = new(inner);
        bool successful = await user.ResumeInternalAsync(userClient, userGameRoleClient, token).ConfigureAwait(false);
        return successful ? user : null;
    }

    /// <summary>
    /// 初始化用户
    /// </summary>
    /// <param name="cookie">cookie</param>
    /// <param name="userClient">用户客户端</param>
    /// <param name="userGameRoleClient">角色客户端</param>
    /// <param name="authClient">授权客户端</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户是否初始化完成，若Cookie失效会返回 <see langword="false"/> </returns>
    internal static async Task<User?> CreateAsync(
        IDictionary<string, string> cookie,
        UserClient userClient,
        BindingClient userGameRoleClient,
        AuthClient authClient,
        CancellationToken token = default)
    {
        string simplifiedCookie = ToCookieString(cookie);
        EntityUser inner = EntityUser.Create(simplifiedCookie);
        User user = new(inner);
        bool successful = await user.CreateInternalAsync(cookie, userClient, userGameRoleClient, authClient, token).ConfigureAwait(false);
        return successful ? user : null;
    }

    /// <summary>
    /// 尝试升级到Stoken
    /// </summary>
    /// <param name="addition">额外的token</param>
    /// <param name="authClient">验证客户端</param>
    /// <param name="token">取消令牌</param>
    /// <returns>是否升级成功</returns>
    internal async Task<bool> TryUpgradeByLoginTicketAsync(IDictionary<string, string> addition, AuthClient authClient, CancellationToken token)
    {
        IDictionary<string, string> cookie = MapCookie(Cookie!);
        if (addition.TryGetValue(CookieKeys.LOGIN_TICKET, out string? loginTicket))
        {
            cookie[CookieKeys.LOGIN_TICKET] = loginTicket;
        }

        if (addition.TryGetValue(CookieKeys.LOGIN_UID, out string? loginUid))
        {
            cookie[CookieKeys.LOGIN_UID] = loginUid;
        }

        bool result = await TryRequestStokenAndAddToCookieAsync(cookie, authClient, token).ConfigureAwait(false);

        if (result)
        {
            Cookie = ToCookieString(cookie);
        }

        return result;
    }

    /// <summary>
    /// 添加 Stoken
    /// </summary>
    /// <param name="addition">额外的cookie</param>
    internal void AddStoken(IDictionary<string, string> addition)
    {
        IDictionary<string, string> cookie = MapCookie(Cookie!);

        if (addition.TryGetValue(CookieKeys.STOKEN, out string? stoken))
        {
            cookie[CookieKeys.STOKEN] = stoken;
        }

        if (addition.TryGetValue(CookieKeys.STUID, out string? stuid))
        {
            cookie[CookieKeys.STUID] = stuid;
        }
    }

    private static string ToCookieString(IDictionary<string, string> cookie)
    {
        return string.Join(';', cookie.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }

    private static async Task<bool> TryRequestStokenAndAddToCookieAsync(IDictionary<string, string> cookie, AuthClient authClient, CancellationToken token)
    {
        if (cookie.TryGetValue(CookieKeys.LOGIN_TICKET, out string? loginTicket))
        {
            string? loginUid = cookie.GetValueOrDefault(CookieKeys.LOGIN_UID) ?? cookie.GetValueOrDefault(CookieKeys.LTUID);

            if (loginUid != null)
            {
                Dictionary<string, string> stokens = await authClient
                    .GetMultiTokenByLoginTicketAsync(loginTicket, loginUid, token)
                    .ConfigureAwait(false);

                if (stokens.TryGetValue(CookieKeys.STOKEN, out string? stoken) && stokens.TryGetValue(CookieKeys.LTOKEN, out string? ltoken))
                {
                    cookie[CookieKeys.STOKEN] = stoken;
                    cookie[CookieKeys.LTOKEN] = ltoken;
                    cookie[CookieKeys.STUID] = cookie[CookieKeys.LTUID];

                    return true;
                }
            }
        }

        return false;
    }

    private async Task<bool> ResumeInternalAsync(
        UserClient userClient,
        BindingClient userGameRoleClient,
        CancellationToken token = default)
    {
        if (isInitialized)
        {
            return true;
        }

        await PrepareUserInfoAndUserGameRolesAsync(userClient, userGameRoleClient, token).ConfigureAwait(false);

        isInitialized = true;

        return UserInfo != null && UserGameRoles.Any();
    }

    private async Task<bool> CreateInternalAsync(
        IDictionary<string, string> cookie,
        UserClient userClient,
        BindingClient userGameRoleClient,
        AuthClient authClient,
        CancellationToken token = default)
    {
        if (isInitialized)
        {
            return true;
        }

        if (await TryRequestStokenAndAddToCookieAsync(cookie, authClient, token).ConfigureAwait(false))
        {
            Cookie = ToCookieString(cookie);
        }

        await PrepareUserInfoAndUserGameRolesAsync(userClient, userGameRoleClient, token).ConfigureAwait(false);

        isInitialized = true;

        return UserInfo != null && UserGameRoles.Any();
    }

    private async Task PrepareUserInfoAndUserGameRolesAsync(UserClient userClient, BindingClient userGameRoleClient, CancellationToken token)
    {
        UserInfo = await userClient
            .GetUserFullInfoAsync(this, token)
            .ConfigureAwait(false);

        UserGameRoles = await userGameRoleClient
            .GetUserGameRolesAsync(this, token)
            .ConfigureAwait(false);

        SelectedUserGameRole = UserGameRoles.FirstOrFirstOrDefault(role => role.IsChosen);
    }
}