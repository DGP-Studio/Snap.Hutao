// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.Database;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.ObjectModel;
using BindingUser = Snap.Hutao.ViewModel.User.User;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Service.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUserCollectionService))]
internal sealed partial class UserCollectionService : IUserCollectionService, IDisposable
{
    private readonly IUserInitializationService userInitializationService;
    private readonly IServiceProvider serviceProvider;
    private readonly IUserDbService userDbService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private readonly SemaphoreSlim throttler = new(1);

    private AdvancedDbCollectionView<BindingUser, EntityUser>? users;

    public async ValueTask<AdvancedDbCollectionView<BindingUser, EntityUser>> GetUserCollectionAsync()
    {
        // Force run in background thread, otherwise will cause reentrance
        await taskContext.SwitchToBackgroundAsync();
        using (await throttler.EnterAsync().ConfigureAwait(false))
        {
            if (users is null)
            {
                List<EntityUser> entities = await userDbService.GetUserListAsync().ConfigureAwait(false);
                List<BindingUser> users = await entities.SelectListAsync(userInitializationService.ResumeUserAsync).ConfigureAwait(false);

                foreach (BindingUser user in users)
                {
                    if (user.NeedDbUpdateAfterResume)
                    {
                        await userDbService.UpdateUserAsync(user.Entity).ConfigureAwait(false);
                        user.NeedDbUpdateAfterResume = false;
                    }
                }

                await taskContext.SwitchToMainThreadAsync();
                this.users = new(users.ToObservableReorderableDbCollection<BindingUser, EntityUser>(serviceProvider), serviceProvider);
            }
        }

        return users;
    }

    public async ValueTask<ObservableCollection<UserAndUid>> GetUserAndUidCollectionAsync()
    {
        if (userAndUidCollection is null)
        {
            await taskContext.SwitchToBackgroundAsync();
            ObservableCollection<BindingUser> users = await GetUserCollectionAsync().ConfigureAwait(false);
            List<UserAndUid> roles = [];
            uidUserGameRoleMap = [];

            foreach (BindingUser user in users)
            {
                foreach (UserGameRole role in user.UserGameRoles)
                {
                    roles.Add(UserAndUid.From(user.Entity, role));
                    uidUserGameRoleMap[role.GameUid] = role;
                }
            }

            userAndUidCollection = roles.ToObservableCollection();
        }

        return userAndUidCollection;
    }

    public async ValueTask RemoveUserAsync(BindingUser user)
    {
        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        ArgumentNullException.ThrowIfNull(users);
        users.Remove(user);
        userAndUidCollection?.RemoveWhere(r => r.User.Mid == user.Entity.Mid);
        if (user.Entity.Mid is not null)
        {
            midUserMap?.Remove(user.Entity.Mid);
        }

        foreach (UserGameRole role in user.UserGameRoles)
        {
            uidUserGameRoleMap?.Remove(role.GameUid);
        }

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        await userDbService.DeleteUserByIdAsync(user.Entity.InnerId).ConfigureAwait(false);

        messenger.Send(new UserRemovedMessage(user.Entity));
    }

    public UserGameRole? GetUserGameRoleByUid(string uid)
    {
        if (uidUserGameRoleMap is null)
        {
            return default;
        }

        return uidUserGameRoleMap.GetValueOrDefault(uid);
    }

    public bool TryGetUserByMid(string mid, [NotNullWhen(true)] out BindingUser? user)
    {
        ArgumentNullException.ThrowIfNull(midUserMap);
        return midUserMap.TryGetValue(mid, out user);
    }

    public async ValueTask<ValueResult<UserOptionResult, string>> TryCreateAndAddUserFromInputCookieAsync(InputCookie inputCookie)
    {
        await taskContext.SwitchToBackgroundAsync();
        BindingUser? newUser = await userInitializationService.CreateUserFromInputCookieOrDefaultAsync(inputCookie).ConfigureAwait(false);

        if (newUser is null)
        {
            return new(UserOptionResult.CookieInvalid, SH.ServiceUserProcessCookieRequestUserInfoFailed);
        }

        await GetUserCollectionAsync().ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(users);

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        users.Add(newUser); // Database synced in the collection
        if (newUser.Entity.Mid is not null)
        {
            midUserMap?.Add(newUser.Entity.Mid, newUser);
        }

        if (userAndUidCollection is not null)
        {
            foreach (UserGameRole role in newUser.UserGameRoles)
            {
                userAndUidCollection.Add(new(newUser.Entity, role));
                uidUserGameRoleMap?.Add(role.GameUid, role);
            }
        }

        ArgumentNullException.ThrowIfNull(newUser.UserInfo);
        return new(UserOptionResult.Added, newUser.UserInfo.Uid);
    }

    public void Dispose()
    {
        throttler.Dispose();
    }
}