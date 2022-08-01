// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.Generic;
using System.Linq;

namespace Snap.Hutao.Model.Binding;

/// <summary>
/// 用于视图绑定的用户
/// </summary>
public class User : Observable
{
    private readonly Entity.User inner;

    private UserGameRole? selectedUserGameRole;
    private bool isInitialized = false;

    /// <summary>
    /// 构造一个新的绑定视图用户
    /// </summary>
    /// <param name="user">用户实体</param>
    private User(Entity.User user)
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

    /// <inheritdoc cref="Entity.User.IsSelected"/>
    public bool IsSelected
    {
        get => inner.IsSelected;
        set => inner.IsSelected = value;
    }

    /// <inheritdoc cref="Entity.User.Cookie"/>
    public string? Cookie
    {
        get => inner.Cookie;
        set => inner.Cookie = value;
    }

    /// <summary>
    /// 内部的用户实体
    /// </summary>
    public Entity.User Entity { get => inner; }

    /// <summary>
    /// 是否初始化完成
    /// </summary>
    public bool IsInitialized { get => isInitialized; }

    /// <summary>
    /// 初始化用户
    /// </summary>
    /// <param name="inner">用户实体</param>
    /// <param name="userClient">用户客户端</param>
    /// <param name="userGameRoleClient">角色客户端</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户是否初始化完成，若Cookie失效会返回 <see langword="false"/> </returns>
    internal static async Task<User?> CreateAsync(Entity.User inner, UserClient userClient, UserGameRoleClient userGameRoleClient, CancellationToken token = default)
    {
        User user = new(inner);
        bool successful = await user.InitializeAsync(userClient, userGameRoleClient, token).ConfigureAwait(false);
        return successful ? user : null;
    }

    private async Task<bool> InitializeAsync(UserClient userClient, UserGameRoleClient userGameRoleClient, CancellationToken token = default)
    {
        if (isInitialized)
        {
            return true;
        }

        UserInfo = await userClient
            .GetUserFullInfoAsync(this, token)
            .ConfigureAwait(false);

        UserGameRoles = await userGameRoleClient
            .GetUserGameRolesAsync(this, token)
            .ConfigureAwait(false);

        SelectedUserGameRole = UserGameRoles.FirstOrFirstOrDefault(role => role.IsChosen);

        isInitialized = true;

        return UserInfo != null && UserGameRoles.Any();
    }
}
