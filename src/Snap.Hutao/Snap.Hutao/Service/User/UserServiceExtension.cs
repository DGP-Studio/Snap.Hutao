// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using BindingUser = Snap.Hutao.ViewModel.User.User;

namespace Snap.Hutao.Service.User;

internal static class UserServiceExtension
{
    public static ValueTask<bool> RefreshCookieTokenAsync(this IUserService userService, BindingUser user)
    {
        return userService.RefreshCookieTokenAsync(user.Entity);
    }
}