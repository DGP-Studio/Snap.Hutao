// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using BindingUser = Snap.Hutao.ViewModel.User.User;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Service.User;

internal static class UserServiceExtension
{
    public static ValueTask<bool> RefreshCookieTokenAsync(this IUserService userService, BindingUser user)
    {
        return userService.RefreshCookieTokenAsync(user.Entity);
    }

    public static async ValueTask<UserGameRole?> GetUserGameRoleByUidAsync(this IUserService userService, string uid)
    {
        AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
        return users.SourceCollection.SelectMany(user => user.UserGameRoles.SourceCollection).FirstOrDefault(role => role.GameUid == uid);
    }

    public static async ValueTask<string?> GetCurrentUidAsync(this IUserService userService)
    {
        AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
        return users.CurrentItem?.UserGameRoles?.CurrentItem?.GameUid;
    }

    public static async ValueTask<UserAndUid?> GetCurrentUserAndUidAsync(this IUserService userService)
    {
        AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
        UserAndUid.TryFromUser(users.CurrentItem, out UserAndUid? userAndUid);
        return userAndUid;
    }

    public static async ValueTask<BindingUser?> GetUserByMidAsync(this IUserService userService, string mid)
    {
        AdvancedDbCollectionView<BindingUser, EntityUser> users = await userService.GetUsersAsync().ConfigureAwait(false);
        return users.SourceCollection.SingleOrDefault(u => u.Entity.Mid == mid);
    }
}