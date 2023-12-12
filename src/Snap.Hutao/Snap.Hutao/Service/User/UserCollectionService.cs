// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Message;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.ObjectModel;
using BindingUser = Snap.Hutao.ViewModel.User.User;

namespace Snap.Hutao.Service.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUserCollectionService))]
internal sealed partial class UserCollectionService : IUserCollectionService
{
    private readonly ScopedDbCurrent<BindingUser, Model.Entity.User, UserChangedMessage> dbCurrent;
    private readonly IUserInitializationService userInitializationService;
    private readonly IUserDbService userDbService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private readonly Throttler throttler = new();

    private ObservableCollection<BindingUser>? userCollection;
    private Dictionary<string, BindingUser>? midUserMap;

    private ObservableCollection<UserAndUid>? userAndUidCollection;
    private Dictionary<string, UserGameRole>? uidUserGameRoleMap;

    public BindingUser? CurrentUser
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    public async ValueTask<ObservableCollection<BindingUser>> GetUserCollectionAsync()
    {
        using (await throttler.ThrottleAsync().ConfigureAwait(false))
        {
            if (userCollection is null)
            {
                List<Model.Entity.User> entities = await userDbService.GetUserListAsync().ConfigureAwait(false);
                List<BindingUser> users = await entities.SelectListAsync(userInitializationService.ResumeUserAsync).ConfigureAwait(false);

                midUserMap = [];
                foreach (BindingUser user in users)
                {
                    if (user.Entity.Mid is not null)
                    {
                        midUserMap[user.Entity.Mid] = user;
                    }

                    if (user.NeedDbUpdateAfterResume)
                    {
                        await userDbService.UpdateUserAsync(user.Entity).ConfigureAwait(false);
                        user.NeedDbUpdateAfterResume = false;
                    }
                }

                userCollection = users.ToObservableCollection();

                try
                {
                    CurrentUser = users.SelectedOrDefault();
                }
                catch (InvalidOperationException ex)
                {
                    ThrowHelper.UserdataCorrupted(SH.ServiceUserCurrentMultiMatched, ex);
                }
            }
        }

        return userCollection;
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
        ArgumentNullException.ThrowIfNull(userCollection);
        userCollection.Remove(user);
        userAndUidCollection?.RemoveWhere(r => r.User.Mid == user.Entity.Mid);
        if (user.Entity.Mid is not null)
        {
            midUserMap?.Remove(user.Entity.Mid);
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

        try
        {
            return uidUserGameRoleMap[uid];
        }
        catch (InvalidOperationException)
        {
            // Sequence contains more than one matching element
            // TODO: return a specialize UserGameRole to indicate error
        }

        return default;
    }

    public bool TryGetUserByMid(string mid, [NotNullWhen(true)] out BindingUser? user)
    {
        ArgumentNullException.ThrowIfNull(midUserMap);
        return midUserMap.TryGetValue(mid, out user);
    }

    public async ValueTask<ValueResult<UserOptionResult, string>> TryCreateAndAddUserFromCookieAsync(Cookie cookie, bool isOversea)
    {
        await taskContext.SwitchToBackgroundAsync();
        BindingUser? newUser = await userInitializationService.CreateUserFromCookieOrDefaultAsync(cookie, isOversea).ConfigureAwait(false);

        if (newUser is null)
        {
            return new(UserOptionResult.Invalid, SH.ServiceUserProcessCookieRequestUserInfoFailed);
        }

        await GetUserCollectionAsync().ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(userCollection);

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        userCollection.Add(newUser);
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

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        await userDbService.AddUserAsync(newUser.Entity).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(newUser.UserInfo);
        return new(UserOptionResult.Added, newUser.UserInfo.Uid);
    }
}