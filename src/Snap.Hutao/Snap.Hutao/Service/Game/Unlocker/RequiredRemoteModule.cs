// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Unlocker;

internal readonly struct RequiredRemoteModule
{
    public readonly bool HasValue = false;
    public readonly Module Executable;

    public RequiredRemoteModule(in Module executable)
    {
        HasValue = true;
        Executable = executable;
    }
}