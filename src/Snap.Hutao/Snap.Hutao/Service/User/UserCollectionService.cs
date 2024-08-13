// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.Database;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using BindingUser = Snap.Hutao.ViewModel.User.User;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Service.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUserCollectionService))]
internal sealed partial class UserCollectionService : IUserCollectionService, IDisposable
{
    private readonly IUserInitializationService userInitializationService;
    private readonly IServiceProvider serviceProvider;
    private readonly IUserRepository userRepository;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private readonly SemaphoreSlim throttler = new(1);

    private AdvancedDbCollectionView<BindingUser, EntityUser>? users;

    public async ValueTask<AdvancedDbCollectionView<BindingUser, EntityUser>> GetUsersAsync()
    {
        // Force run in background thread, otherwise will cause reentrance
        await taskContext.SwitchToBackgroundAsync();
        using (await throttler.EnterAsync().ConfigureAwait(false))
        {
            if (users is null)
            {
                List<EntityUser> entities = userRepository.GetUserList();
                List<BindingUser> users = await entities.SelectListAsync(userInitializationService.ResumeUserAsync).ConfigureAwait(false);

                foreach (BindingUser user in users)
                {
                    if (user.NeedDbUpdateAfterResume)
                    {
                        userRepository.UpdateUser(user.Entity);
                        user.NeedDbUpdateAfterResume = false;
                    }
                }

                await taskContext.SwitchToMainThreadAsync();
                this.users = new(users.ToObservableReorderableDbCollection<BindingUser, EntityUser>(serviceProvider), serviceProvider);
                this.users.CurrentChanged += OnCurrentUserChanged;
            }
        }

        return users;
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

    public async ValueTask<ValueResult<UserOptionResult, string>> TryCreateAndAddUserFromInputCookieAsync(InputCookie inputCookie)
    {
        await taskContext.SwitchToBackgroundAsync();
        BindingUser? newUser = await userInitializationService.CreateUserFromInputCookieOrDefaultAsync(inputCookie).ConfigureAwait(false);

        if (newUser is null)
        {
            return new(UserOptionResult.CookieInvalid, SH.ServiceUserProcessCookieRequestUserInfoFailed);
        }

        await GetUsersAsync().ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(users);

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        users.Add(newUser); // Database synced in the collection

        ArgumentNullException.ThrowIfNull(newUser.UserInfo);
        return new(UserOptionResult.Added, newUser.UserInfo.Uid);
    }

    public void Dispose()
    {
        throttler.Dispose();
    }

    private void OnCurrentUserChanged(object? sender, object? args)
    {
        if (users is null)
        {
            messenger.Send(UserAndUidChangedMessage.Empty);
            return;
        }

        if (users.CurrentItem is null)
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
                users.CurrentItem.UserGameRoles.MoveCurrentToFirst();
            }
        }

        messenger.Send(new UserAndUidChangedMessage(users.CurrentItem));
    }
}