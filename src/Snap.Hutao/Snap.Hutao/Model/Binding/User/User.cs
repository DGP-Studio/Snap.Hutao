// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Extension;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Passport;
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
    /// 用户信息
    /// </summary>
    public UserGameRole? SelectedUserGameRole
    {
        get => selectedUserGameRole;
        private set => SetProperty(ref selectedUserGameRole, value);
    }

    /// <inheritdoc cref="EntityUser.IsSelected"/>
    public bool IsSelected
    {
        get => inner.IsSelected;
        set => inner.IsSelected = value;
    }

    /// <inheritdoc cref="EntityUser.Cookie"/>
    public Cookie? Cookie
    {
        get => inner.Cookie;
        set => inner.Cookie = value;
    }

    /// <inheritdoc cref="EntityUser.Stoken"/>
    public Cookie? Stoken
    {
        get => inner.Cookie;
        set
        {
            inner.Stoken = value;
            OnPropertyChanged(nameof(HasStoken));
        }
    }

    /// <summary>
    /// 是否拥有 SToken
    /// </summary>
    public bool HasStoken
    {
        get => inner.Stoken != null;
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
        EntityUser entity = EntityUser.Create(cookie);

        UserInformation? userInfo = await Ioc.Default
            .GetRequiredService<PassportClient>()
            .VerifyLtokenAsync(cookie, CancellationToken.None)
            .ConfigureAwait(false);

        entity.Aid = userInfo?.Aid;
        entity.Mid = userInfo?.Mid;

        if (entity.Aid == null && entity.Mid == null)
        {
            return null;
        }
        else
        {
            User user = new(entity);
            bool initialized = await user.InitializeCoreAsync(token).ConfigureAwait(false);
            return initialized ? user : null;
        }
    }

    /// <summary>
    /// 更新SToken
    /// </summary>
    /// <param name="stoken">cookie</param>
    internal void UpdateSToken(Cookie stoken)
    {
        Stoken = stoken;
        OnPropertyChanged(nameof(HasStoken));
    }

    private async Task<bool> InitializeCoreAsync(CancellationToken token = default)
    {
        if (isInitialized)
        {
            return true;
        }

        UserInfo = await Ioc.Default.GetRequiredService<UserClient>()
            .GetUserFullInfoAsync(Entity, token)
            .ConfigureAwait(false);

        UserGameRoles = await Ioc.Default.GetRequiredService<BindingClient>()
            .GetUserGameRolesByCookieAsync(Entity, token)
            .ConfigureAwait(false);

        SelectedUserGameRole = UserGameRoles.FirstOrFirstOrDefault(role => role.IsChosen);

        isInitialized = true;

        return UserInfo != null && UserGameRoles.Any();
    }
}
