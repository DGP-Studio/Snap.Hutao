// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.ExceptionService;

internal enum HutaoExceptionKind
{
    None,

    // Foundation
    ImageCacheInvalidUri,

    // IO
    FileSystemCreateFileInsufficientPermissions,
    PrivateNamedPipeContentHashIncorrect,

    // Service
    GachaStatisticsInvalidItemId,
    GameFpsUnlockingFailed,
}