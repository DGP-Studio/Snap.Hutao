// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.ViewModel.User;

/// <summary>
/// 用于视图绑定的用户
/// </summary>
[HighQuality]
internal sealed class User : ObservableObject, IEntityOnly<EntityUser>, IMappingFrom<User, EntityUser, IServiceProvider>, ISelectable
{
    private readonly EntityUser inner;
    private readonly IMessenger messenger;

    private UserGameRole? selectedUserGameRole;

    /// <summary>
    /// 构造一个新的绑定视图用户
    /// </summary>
    /// <param name="user">用户实体</param>
    private User(EntityUser user, IServiceProvider serviceProvider)
    {
        inner = user;
        messenger = serviceProvider.GetRequiredService<IMessenger>();
    }

    public bool IsInitialized { get; set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    public UserInfo? UserInfo { get; set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    public List<UserGameRole> UserGameRoles { get; set; } = default!;

    /// <summary>
    /// 用户信息
    /// </summary>
    public UserGameRole? SelectedUserGameRole
    {
        get => selectedUserGameRole;
        set => SetSelectedUserGameRole(value);
    }

    public string? Fingerprint { get => inner.Fingerprint; set => inner.Fingerprint = value; }

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

    public bool NeedDbUpdateAfterResume { get; set; }

    public static User From(EntityUser user, IServiceProvider provider)
    {
        return new(user, provider);
    }

    public void SetSelectedUserGameRole(UserGameRole? value, bool raiseMessage = true)
    {
        if (SetProperty(ref selectedUserGameRole, value, nameof(SelectedUserGameRole)) && raiseMessage)
        {
            messenger.Send(Message.UserChangedMessage.CreateOnlyRoleChanged(this));
        }
    }
}
