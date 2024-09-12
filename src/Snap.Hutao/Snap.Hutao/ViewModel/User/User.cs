// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Database.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.ViewModel.User;

internal sealed partial class User : IEntityAccess<EntityUser>,
    IMappingFrom<User, EntityUser, IServiceProvider>,
    ISelectable,
    IAdvancedCollectionViewItem
{
    private readonly EntityUser inner;
    private readonly IServiceProvider serviceProvider;

    private AdvancedCollectionView<UserGameRole> userGameRoles = default!;
    private bool isCurrentUserGameRoleChangedMessageSuppressed;

    private User(EntityUser user, IServiceProvider serviceProvider)
    {
        inner = user;
        this.serviceProvider = serviceProvider;
    }

    public bool IsInitialized { get; set; }

    public UserInfo? UserInfo { get; set; }

    public AdvancedCollectionView<UserGameRole> UserGameRoles
    {
        get => userGameRoles;
        set
        {
            if (userGameRoles is not null)
            {
                userGameRoles.CurrentChanged -= OnCurrentUserGameRoleChanged;
            }

            userGameRoles = value;

            if (value is not null)
            {
                value.CurrentChanged += OnCurrentUserGameRoleChanged;
            }
        }
    }

    public string? Fingerprint { get => inner.Fingerprint; }

    public Guid InnerId { get => inner.InnerId; }

    public bool IsSelected
    {
        get => inner.IsSelected;
        set => inner.IsSelected = value;
    }

    public Cookie? CookieToken
    {
        get => inner.CookieToken;
        set => inner.CookieToken = value;
    }

    public Cookie? LToken
    {
        get => inner.LToken;
        set => inner.LToken = value;
    }

    public Cookie? SToken
    {
        get => inner.SToken;
        set => inner.SToken = value;
    }

    public bool IsOversea { get => Entity.IsOversea; }

    public EntityUser Entity { get => inner; }

    public bool NeedDbUpdateAfterResume { get; set; }

    public string? PreferredUid { get => inner.PreferredUid; }

    public static User From(EntityUser user, IServiceProvider provider)
    {
        return new(user, provider);
    }

    public IDisposable SuppressCurrentUserGameRoleChangedMessage()
    {
        return new CurrentUserGameRoleChangedSuppression(this);
    }

    private void OnCurrentUserGameRoleChanged(object? sender, object? e)
    {
        if (userGameRoles.CurrentItem is { } item && inner.PreferredUid != item.GameUid)
        {
            inner.PreferredUid = item.GameUid;
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<AppDbContext>().Users.UpdateAndSave(inner);
            }
        }

        if (!isCurrentUserGameRoleChangedMessageSuppressed)
        {
            serviceProvider.GetRequiredService<IMessenger>().Send(new UserAndUidChangedMessage(this));
        }
    }

    private sealed partial class CurrentUserGameRoleChangedSuppression : IDisposable
    {
        private readonly User reference;

        public CurrentUserGameRoleChangedSuppression(User reference)
        {
            this.reference = reference;
            reference.isCurrentUserGameRoleChangedMessageSuppressed = true;
        }

        public void Dispose()
        {
            reference.isCurrentUserGameRoleChangedMessageSuppressed = false;
        }
    }
}