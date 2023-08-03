// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.ViewModel.User;

/// <summary>
/// 用于视图绑定的用户
/// </summary>
[HighQuality]
internal sealed class User : ObservableObject, IEntityOnly<EntityUser>, ISelectable
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

    public Guid InnerId { get => inner.InnerId; }

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
    /// 是否为国际服
    /// </summary>
    public bool IsOversea { get => Entity.IsOversea; }

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
    internal static async ValueTask<User> ResumeAsync(EntityUser inner, CancellationToken token = default)
    {
        User user = new(inner);

        if (!await user.InitializeCoreAsync(token).ConfigureAwait(false))
        {
            user.UserInfo = new() { Nickname = SH.ModelBindingUserInitializationFailed };
            user.UserGameRoles = new();
        }

        return user;
    }

    /// <summary>
    /// 创建并初始化用户
    /// </summary>
    /// <param name="cookie">cookie</param>
    /// <param name="isOversea">是否为国际服</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户</returns>
    internal static async Task<User?> CreateAsync(Cookie cookie, bool isOversea, CancellationToken token = default)
    {
        // 这里只负责创建实体用户，稍后在用户服务中保存到数据库
        EntityUser entity = EntityUser.From(cookie, isOversea);

        entity.Aid = cookie.GetValueOrDefault(Cookie.STUID);
        entity.Mid = isOversea ? entity.Aid : cookie.GetValueOrDefault(Cookie.MID);
        entity.IsOversea = isOversea;

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
            // Prevent multiple initialization.
            return true;
        }

        if (SToken == null)
        {
            return false;
        }

        using (IServiceScope scope = Ioc.Default.CreateScope())
        {
            bool isOversea = Entity.IsOversea;

            if (!await TrySetLTokenAsync(scope.ServiceProvider, token).ConfigureAwait(false))
            {
                return false;
            }

            if (!await TrySetCookieTokenAsync(scope.ServiceProvider, token).ConfigureAwait(false))
            {
                return false;
            }

            if (!await TrySetUserInfoAsync(scope.ServiceProvider, token).ConfigureAwait(false))
            {
                return false;
            }

            if (!await TrySetUserGameRolesAsync(scope.ServiceProvider, token).ConfigureAwait(false))
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

        Response<LTokenWrapper> lTokenResponse = await provider
            .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
            .Create(Entity.IsOversea)
            .GetLTokenBySTokenAsync(Entity, token)
            .ConfigureAwait(false);

        if (lTokenResponse.IsOk())
        {
            LToken = Cookie.Parse($"{Cookie.LTUID}={Entity.Aid};{Cookie.LTOKEN}={lTokenResponse.Data.LToken}");
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
            .GetRequiredService<IOverseaSupportFactory<IPassportClient>>()
            .Create(Entity.IsOversea)
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
            .GetRequiredService<IOverseaSupportFactory<IUserClient>>()
            .Create(Entity.IsOversea)
            .GetUserFullInfoAsync(Entity, token)
            .ConfigureAwait(false);

        if (response.IsOk())
        {
            UserInfo = response.Data.UserInfo;
            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task<bool> TrySetUserGameRolesAsync(IServiceProvider provider, CancellationToken token)
    {
        Response<ListWrapper<UserGameRole>> userGameRolesResponse = await provider
            .GetRequiredService<BindingClient>()
            .GetUserGameRolesOverseaAwareAsync(Entity, token)
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
}
