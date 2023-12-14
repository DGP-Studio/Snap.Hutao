// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Entity.Extension;

internal static class UserExtension
{
    public static bool TryUpdateFingerprint(this User user, string? deviceFp)
    {
        if (string.IsNullOrEmpty(deviceFp))
        {
            return false;
        }

        user.Fingerprint = deviceFp;
        user.FingerprintLastUpdateTime = DateTimeOffset.UtcNow;
        return true;
    }
}