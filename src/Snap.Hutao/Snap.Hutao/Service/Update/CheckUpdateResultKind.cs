// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Update;

internal enum CheckUpdateResultKind
{
    None = 0,
    VersionApiInvalidResponse = 1,
    VersionApiInvalidSha256 = 2,
    AlreayUpdated = 3,

    NeedDownload = 4,
    NeedInstall = 5,
}