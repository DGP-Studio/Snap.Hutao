// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity.Extension;

namespace Snap.Hutao.ViewModel.User;

internal static class UserExtension
{
    public static bool TryUpdateFingerprint(this User user, string? deviceFp)
    {
        return user.Entity.TryUpdateFingerprint(deviceFp);
    }
}