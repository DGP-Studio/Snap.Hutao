// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
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
    ISelectable,
    IAdvancedCollectionViewItem
{
    private readonly IServiceProvider serviceProvider;

    private IAdvancedCollectionView<UserGameRole> userGameRoles = default!;
    private bool isCurrentUserGameRoleChangedMessageSuppressed;

    private User(EntityUser user, IServiceProvider serviceProvider)
    {
        Entity = user;
        this.serviceProvider = serviceProvider;
    }

    public bool IsInitialized { get; set; }

    public UserInfo? UserInfo { get; set; }

    public IAdvancedCollectionView<UserGameRole> UserGameRoles
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

    public string? Fingerprint { get => Entity.Fingerprint; }

    public Guid InnerId { get => Entity.InnerId; }

    public bool IsSelected
    {
        get => Entity.IsSelected;
        set => Entity.IsSelected = value;
    }

    public Cookie? CookieToken
    {
        get => Entity.CookieToken;
        set => Entity.CookieToken = value;
    }

    public Cookie? LToken
    {
        get => Entity.LToken;
        set => Entity.LToken = value;
    }

    public Cookie? SToken
    {
        get => Entity.SToken;
        set => Entity.SToken = value;
    }

    public bool IsOversea { get => Entity.IsOversea; }

    public EntityUser Entity { get; }

    public bool NeedDbUpdateAfterResume { get; set; }

    public string? PreferredUid { get => Entity.PreferredUid; }

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
        if (userGameRoles.CurrentItem is { } item && Entity.PreferredUid != item.GameUid)
        {
            Entity.PreferredUid = item.GameUid;
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<AppDbContext>().Users.UpdateAndSave(Entity);
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