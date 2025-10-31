// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.Immutable;
using BindingUser = Snap.Hutao.ViewModel.User.User;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Service.User;

[Service(ServiceLifetime.Singleton, typeof(IUserCollectionService))]
internal sealed partial class UserCollectionService : IUserCollectionService, IDisposable
{
    private readonly IUserInitializationService userInitializationService;
    private readonly IServiceProvider serviceProvider;
    private readonly IUserRepository userRepository;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private readonly AsyncLock collectionLocker = new();

    private AdvancedDbCollectionView<BindingUser, EntityUser>? users;

    [GeneratedConstructor]
    public partial UserCollectionService(IServiceProvider serviceProvider);

    public async ValueTask<AdvancedDbCollectionView<BindingUser, EntityUser>> GetUsersAsync()
    {
        // Force run in background thread, otherwise will cause re-entrance
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
        using (await collectionLocker.LockAsync().ConfigureAwait(false))
        {
            if (users is null)
            {
                ImmutableArray<EntityUser> entityUsers = userRepository.GetUserImmutableArray();
                List<BindingUser> bindingUsers = new(entityUsers.Length);
                foreach (EntityUser entity in entityUsers)
                {
                    BindingUser user = await userInitializationService.ResumeUserAsync(entity).ConfigureAwait(false);
                    if (user.NeedDbUpdateAfterResume)
                    {
                        userRepository.UpdateUser(user.Entity);
                        user.NeedDbUpdateAfterResume = false;
                    }

                    bindingUsers.Add(user);
                }

                users = bindingUsers.ToAdvancedDbCollectionViewWrappedObservableReorderableDbCollection<BindingUser, EntityUser>(serviceProvider);

                // Since this service is singleton, we can safely subscribe to the event
                users.CurrentChanged += OnCurrentUserChanged;

                await taskContext.SwitchToMainThreadAsync();
                users.MoveCurrentTo(users.Source.SelectedOrFirstOrDefault());
            }

            return users;
        }
    }

    public async ValueTask RemoveUserAsync(BindingUser user)
    {
        ArgumentNullException.ThrowIfNull(users);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        userRepository.DeleteUserById(user.Entity.InnerId);

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        users.Remove(user);

        messenger.Send(new UserRemovedMessage(user));
    }

    public async ValueTask<ValueResult<UserOptionResultKind, string?>> TryCreateAndAddUserFromInputCookieAsync(InputCookie inputCookie)
    {
        await taskContext.SwitchToBackgroundAsync();
        BindingUser? newUser = await userInitializationService.CreateUserFromInputCookieOrDefaultAsync(inputCookie).ConfigureAwait(false);

        if (newUser is null)
        {
            return new(UserOptionResultKind.CookieInvalid, SH.ServiceUserProcessCookieRequestUserInfoFailed);
        }

        if (newUser.UserGameRoles.Count is 0)
        {
            return new(UserOptionResultKind.GameRoleNotFound, SH.ServiceUserUserInfoContainsNoGameRole);
        }

        await GetUsersAsync().ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(users);

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        users.Add(newUser); // Database synced in the collection

        ArgumentNullException.ThrowIfNull(newUser.UserInfo);
        return new(UserOptionResultKind.Added, newUser.UserInfo.Uid);
    }

    public void Dispose()
    {
        if (users is not null)
        {
            users.CurrentChanged -= OnCurrentUserChanged;
        }
    }

    private void OnCurrentUserChanged(object? sender, object? args)
    {
        if (users?.CurrentItem is null)
        {
            messenger.Send(UserAndUidChangedMessage.Empty);
            return;
        }

        // Suppress the BindingUser itself to raise the message
        // This is to avoid the message being raised in the
        // BindingUser.OnCurrentUserGameRoleChanged.
        using (users.CurrentItem.SuppressCurrentUserGameRoleChangedMessage())
        {
            foreach (UserGameRole role in users.CurrentItem.UserGameRoles)
            {
                if (role.GameUid == users.CurrentItem.PreferredUid)
                {
                    users.CurrentItem.UserGameRoles.MoveCurrentTo(role);
                    break;
                }
            }

            if (users.CurrentItem.UserGameRoles.CurrentItem is null)
            {
                if (users.CurrentItem.UserGameRoles.Source.SingleOrDefault(role => role.IsChosen) is { } chosenRole)
                {
                    users.CurrentItem.UserGameRoles.MoveCurrentTo(chosenRole);
                }
                else
                {
                    users.CurrentItem.UserGameRoles.MoveCurrentToFirst();
                }
            }
        }

        messenger.Send(new UserAndUidChangedMessage(users.CurrentItem));
    }
}