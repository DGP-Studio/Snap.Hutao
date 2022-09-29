// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
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
    public Cookie Cookie
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
        bool successful = await user.InitializeCoreAsync(userClient, userGameRoleClient, token).ConfigureAwait(false);
        return successful ? user : null;
    }

    /// <summary>
    /// 创建并初始化用户
    /// </summary>
    /// <param name="cookie">cookie</param>
    /// <param name="userClient">用户客户端</param>
    /// <param name="userGameRoleClient">角色客户端</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户是否初始化完成，若Cookie失效会返回 <see langword="null"/> </returns>
    internal static async Task<User?> CreateAsync(
        Cookie cookie,
        UserClient userClient,
        BindingClient userGameRoleClient,
        CancellationToken token = default)
    {
        User user = new(EntityUser.Create(cookie));
        bool successful = await user.InitializeCoreAsync(userClient, userGameRoleClient, token).ConfigureAwait(false);
        return successful ? user : null;
    }

    private async Task<bool> InitializeCoreAsync(
        UserClient userClient,
        BindingClient userGameRoleClient,
        CancellationToken token = default)
    {
        if (isInitialized)
        {
            return true;
        }

        await InitializeUserInfoAndUserGameRolesAsync(userClient, userGameRoleClient, token).ConfigureAwait(false);

        isInitialized = true;

        return UserInfo != null && UserGameRoles.Any();
    }

    private async Task InitializeUserInfoAndUserGameRolesAsync(UserClient userClient, BindingClient userGameRoleClient, CancellationToken token)
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