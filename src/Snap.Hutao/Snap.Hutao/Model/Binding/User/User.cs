// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Migrations;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Model.Binding.User;

/// <summary>
/// 用于视图绑定的用户
/// </summary>
[HighQuality]
internal sealed class User : ObservableObject
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
        set
        {
            if (SetProperty(ref selectedUserGameRole, value))
            {
                Ioc.Default
                    .GetRequiredService<IMessenger>()
                    .Send(new Message.UserChangedMessage() { OldValue = this, NewValue = this });
            }
        }
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

    /// <inheritdoc cref="EntityUser.LToken"/>
    public Cookie? LToken
    {
        get => inner.LToken;
        set => inner.LToken = value;
    }

    /// <inheritdoc cref="EntityUser.SToken"/>
    public Cookie? SToken
    {
        get => inner.SToken;
        set => inner.SToken = value;
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
    /// <returns>用户</returns>
    internal static async Task<User> ResumeAsync(EntityUser inner, CancellationToken token = default)
    {
        User user = new(inner);
        bool isOk = false;

        if (!user.Entity.IsOversea)
        {
            isOk = await user.InitializeCoreAsync(token).ConfigureAwait(false);
        }
        else
        {
            isOk = await user.InitializeCoreOsAsync(token).ConfigureAwait(false);
        }

        if (!isOk)
        {
            user.UserInfo = new UserInfo() { Nickname = "网络异常" };
            user.UserGameRoles = new();
        }

        return user;
    }

    /// <summary>
    /// 创建并初始化用户
    /// </summary>
    /// <param name="cookie">cookie</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户</returns>
    internal static async Task<User?> CreateAsync(Cookie cookie, CancellationToken token = default)
    {
        // 这里只负责创建实体用户，稍后在用户服务中保存到数据库
        EntityUser entity = EntityUser.Create(cookie);

        entity.Aid = cookie.GetValueOrDefault(Cookie.STUID);
        entity.Mid = cookie.GetValueOrDefault(Cookie.MID);
        entity.IsOversea = false;

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

    /// <summary>
    /// 创建并初始化国际服用户（临时）
    /// </summary>
    /// <param name="cookie">cookie</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户</returns>
    internal static async Task<User?> CreateOsUserAsync(Cookie cookie, CancellationToken token = default)
    {
        // 这里只负责创建实体用户，稍后在用户服务中保存到数据库
        EntityUser entity = EntityUser.CreateOs(cookie);

        entity.Aid = cookie.GetValueOrDefault(Cookie.STUID);

        // Note: Currently we dont know how to get "mid" for hoyolab user,
        // mid is set as the same value of ltuid(stuid/user id)
        entity.Mid = entity.Aid;

        entity.IsOversea = true;

        if (entity.Aid != null)
        {
            User user = new(entity);
            bool initialized = await user.InitializeCoreOsAsync(token).ConfigureAwait(false);

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
            // Prevent multiple initialization.
            return true;
        }

        if (SToken == null)
        {
            return false;
        }

        using (IServiceScope scope = Ioc.Default.CreateScope())
        {
            if (!await TrySetUserInfoAsync(scope.ServiceProvider, token).ConfigureAwait(false))
            {
                return false;
            }

            if (!await TrySetLTokenAsync(scope.ServiceProvider, token).ConfigureAwait(false))
            {
                return false;
            }

            if (!await TrySetUserGameRolesAsync(scope.ServiceProvider, token).ConfigureAwait(false))
            {
                return false;
            }

            if (await TrySetCookieTokenAsync(scope.ServiceProvider, token).ConfigureAwait(false))
            {
                return false;
            }
        }

        SelectedUserGameRole = UserGameRoles.FirstOrFirstOrDefault(role => role.IsChosen);
        return isInitialized = true;
    }

    private async Task<bool> TrySetLTokenAsync(IServiceProvider provider, CancellationToken token)
    {
        if (LToken != null)
        {
            return true;
        }

        Response<LtokenWrapper> lTokenResponse = await provider
            .GetRequiredService<PassportClient2>()
            .GetLTokenBySTokenAsync(Entity, token)
            .ConfigureAwait(false);

        if (lTokenResponse.IsOk())
        {
            LToken = Cookie.Parse($"{Cookie.LTUID}={Entity.Aid};{Cookie.LTOKEN}={lTokenResponse.Data.Ltoken}");
            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task<bool> TrySetCookieTokenAsync(IServiceProvider provider, CancellationToken token)
    {
        if (CookieToken != null)
        {
            return true;
        }

        Response<UidCookieToken> cookieTokenResponse = await provider
            .GetRequiredService<PassportClient2>()
            .GetCookieAccountInfoBySTokenAsync(Entity, token)
            .ConfigureAwait(false);

        if (cookieTokenResponse.IsOk())
        {
            CookieToken = Cookie.Parse($"{Cookie.ACCOUNT_ID}={Entity.Aid};{Cookie.COOKIE_TOKEN}={cookieTokenResponse.Data.CookieToken}");
            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task<bool> TrySetUserInfoAsync(IServiceProvider provider, CancellationToken token)
    {
        Response<UserFullInfoWrapper> response = await provider
            .GetRequiredService<UserClient>()
            .GetUserFullInfoAsync(Entity, token)
            .ConfigureAwait(false);
        UserInfo = response.Data?.UserInfo;
        return UserInfo != null;
    }

    private async Task<bool> TrySetUserGameRolesAsync(IServiceProvider provider, CancellationToken token)
    {
        Response<ActionTicketWrapper> actionTicketResponse = await provider
                .GetRequiredService<AuthClient>()
                .GetActionTicketByStokenAsync("game_role", Entity)
                .ConfigureAwait(false);

        if (actionTicketResponse.IsOk())
        {
            string actionTicket = actionTicketResponse.Data.Ticket;

            Response<ListWrapper<UserGameRole>> userGameRolesResponse = await provider
                .GetRequiredService<BindingClient>()
                .GetUserGameRolesByActionTicketAsync(actionTicket, Entity, token)
                .ConfigureAwait(false);

            if (userGameRolesResponse.IsOk())
            {
                UserGameRoles = userGameRolesResponse.Data.List;
                return UserGameRoles.Any();
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private async Task<bool> InitializeCoreOsAsync(CancellationToken token = default)
    {
        if (isInitialized)
        {
            return true;
        }

        if (SToken == null)
        {
            return false;
        }

        using (IServiceScope scope = Ioc.Default.CreateScope())
        {

            // 自动填充 Ltoken
            if (LToken == null)
            {
                Response<LtokenWrapper> ltokenResponse = await scope.ServiceProvider
                    .GetRequiredService<PassportClientOs>()
                    .GetLtokenBySTokenAsync(Entity, token)
                    .ConfigureAwait(false);

                if (ltokenResponse.IsOk())
                {
                    Cookie ltokenCookie = Cookie.Parse($"ltuid={Entity.Aid};ltoken={ltokenResponse.Data.Ltoken}");
                    Entity.LToken = ltokenCookie;
                }
                else
                {
                    return false;
                }
            }

            // Fetch user info
            Response<UserFullInfoWrapper> response = await scope.ServiceProvider
                .GetRequiredService<UserClient>()
                .GetOsUserFullInfoAsync(Entity, token)
                .ConfigureAwait(false);
            UserInfo = response.Data?.UserInfo;

            // 自动填充 CookieToken
            if (CookieToken == null)
            {
                Response<UidCookieToken> cookieTokenResponse = await scope.ServiceProvider
                    .GetRequiredService<PassportClientOs>()
                    .GetCookieAccountInfoBySTokenAsync(Entity, token)
                    .ConfigureAwait(false);

                if (cookieTokenResponse.IsOk())
                {
                    Cookie cookieTokenCookie = Cookie.Parse($"account_id={Entity.Aid};cookie_token={cookieTokenResponse.Data.CookieToken}");
                    Entity.CookieToken = cookieTokenCookie;
                }
                else
                {
                    return false;
                }
            }

            // 获取游戏角色
            Response<ListWrapper<UserGameRole>> userGameRolesResponse = await scope.ServiceProvider
                .GetRequiredService<BindingClient>()
                .GetOsUserGameRolesByCookieAsync(Entity, token)
                .ConfigureAwait(false);

            if (userGameRolesResponse.IsOk())
            {
                UserGameRoles = userGameRolesResponse.Data.List;
            }
            else
            {
                return false;
            }
        }

        SelectedUserGameRole = UserGameRoles.FirstOrFirstOrDefault(role => role.IsChosen);

        isInitialized = true;

        return UserInfo != null && UserGameRoles.Any();
    }
}
