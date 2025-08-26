// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class LaunchExecutionGameFileSystemExclusiveAccessChangedMessage
{
    public LaunchExecutionGameFileSystemExclusiveAccessChangedMessage(bool canAccess)
    {
        CanAccess = canAccess;
    }

    public bool CanAccess { get; }
}