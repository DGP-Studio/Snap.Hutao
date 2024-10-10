// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching;

internal enum LaunchExecutionResultKind
{
    Ok,
    NoActiveScheme,
    NoActiveGamePath,
    GameProcessRunning,
    GameConfigFileNotFound,
    GameConfigDirectoryNotFound,
    GameConfigInsufficientPermissions,
    GameDirectoryInsufficientPermissions,
    GameResourceIndexQueryInvalidResponse,
    GameResourcePackageConvertInternalError,
    GameAccountRegistryWriteResultNotMatch,
    GameAccountCreateAuthTicketFailed,
    GameAccountUserAndUidAndServerNotMatch,
    GameFpsUnlockingFailed,
}