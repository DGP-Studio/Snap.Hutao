// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Extension;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Model.Binding.User;

/// <summary>
/// 用于视图绑定的用户
/// </summary>
public class User : ObservableObject
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
    /// 用户信息, 请勿访问set
    /// </summary>
    public UserGameRole? SelectedUserGameRole
    {
        get => selectedUserGameRole;
        set => SetProperty(ref selectedUserGameRole, value);
    }

    /// <inheritdoc cref="EntityUser.IsSelected"/>
    public bool IsSelected
    {
        get => inner.IsSelected;
        set => inner.IsSelected = value;
    }

    /// <inheritdoc cref="EntityUser.CookieToken"/>
    public Cookie? CookieToken
    {
        get => inner.CookieToken;
        set => inner.CookieToken = value;
    }

    /// <inheritdoc cref="EntityUser.Ltoken"/>
    public Cookie? Ltoken
    {
        get => inner.Ltoken;
        set => inner.Ltoken = value;
    }

    /// <inheritdoc cref="EntityUser.Stoken"/>
    public Cookie? Stoken
    {
        get => inner.Stoken;
        set
        {
            inner.Stoken = value;
        }
    }

    /// <summary>
    /// 内部的用户实体
    /// </summary>
    public EntityUser Entity { get => inner; }

    /// <summary>
    /// 从数据库恢复用户
    /// </summary>
    /// <param name="inner">数据库实体</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户是否初始化完成，若Cookie失效会返回 <see langword="false"/> </returns>
    internal static async Task<User?> ResumeAsync(EntityUser inner, CancellationToken token = default)
    {
        User user = new(inner);
        bool successful = await user.InitializeCoreAsync(token).ConfigureAwait(false);
        return successful ? user : null;
    }

    /// <summary>
    /// 创建并初始化用户
    /// </summary>
    /// <param name="cookie">cookie</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户是否初始化完成，若Cookie失效会返回 <see langword="null"/> </returns>
    internal static async Task<User?> CreateAsync(Cookie cookie, CancellationToken token = default)
    {
        // 这里只负责创建实体用户，稍后在用户服务中保存到数据库
        EntityUser entity = EntityUser.Create(cookie);

        entity.Aid = cookie.GetValueOrDefault(Cookie.STUID);
        entity.Mid = cookie.GetValueOrDefault(Cookie.MID);

        if (entity.Aid != null && entity.Mid != null)
        {
            User user = new(entity);
            bool initialized = await user.InitializeCoreAsync(token).ConfigureAwait(false);

            return initialized ? user : null;
        }
        else
        {
            return null;
        }
    }

    private async Task<bool> InitializeCoreAsync(CancellationToken token = default)
    {
        if (isInitialized)
        {
            return true;
        }

        if (Stoken == null)
        {
            return false;
        }

        using (IServiceScope scope = Ioc.Default.CreateScope())
        {
            UserInfo = await scope.ServiceProvider
                .GetRequiredService<UserClient2>()
                .GetUserFullInfoAsync(Entity, token)
                .ConfigureAwait(false);

            // 自动填充 Ltoken
            if (Ltoken == null)
            {
                string? ltoken = await scope.ServiceProvider
                    .GetRequiredService<PassportClient2>()
                    .GetLtokenBySTokenAsync(Entity, token)
                    .ConfigureAwait(false);

                if (ltoken != null)
                {
                    Cookie ltokenCookie = Cookie.Parse($"ltuid={Entity.Aid};ltoken={ltoken}");
                    Entity.Ltoken = ltokenCookie;
                }
            }

            string? actionTicket = await scope.ServiceProvider
                .GetRequiredService<AuthClient>()
                .GetActionTicketByStokenAsync("game_role", Entity)
                .ConfigureAwait(false);

            UserGameRoles = await scope.ServiceProvider
                .GetRequiredService<BindingClient>()
                .GetUserGameRolesByActionTicketAsync(actionTicket!, Entity, token)
                .ConfigureAwait(false);

            // 自动填充 CookieToken
            if (CookieToken == null)
            {
                string? cookieToken = await scope.ServiceProvider
                    .GetRequiredService<PassportClient2>()
                    .GetCookieAccountInfoBySTokenAsync(Entity, token)
                    .ConfigureAwait(false);

                if (cookieToken != null)
                {
                    Cookie cookieTokenCookie = Cookie.Parse($"acount_id={Entity.Aid};cookie_token={cookieToken}");
                    Entity.CookieToken = cookieTokenCookie;
                }
            }
        }

        SelectedUserGameRole = UserGameRoles.FirstOrFirstOrDefault(role => role.IsChosen);

        isInitialized = true;

        return UserInfo != null && UserGameRoles.Any();
    }
}
