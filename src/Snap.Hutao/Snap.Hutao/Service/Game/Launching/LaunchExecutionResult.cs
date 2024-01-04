// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class LaunchExecutionResult
{
    public LaunchExecutionResultKind Kind { get; set; }

    public string ErrorMessage { get; set; } = default!;
}

internal enum LaunchExecutionResultKind
{
    Ok,
    NoActiveScheme,
    NoActiveGamePath,
    GameConfigFileNotFound,
    GameConfigDirectoryNotFound,
    GameConfigUnauthorizedAccess,
}