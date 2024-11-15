// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AvatarInfo;

internal enum RefreshResultKind
{
    Ok,
    MetadataNotInitialized,
    APIUnavailable,
    StatusCodeNotSucceed,
    ShowcaseNotOpen,
}