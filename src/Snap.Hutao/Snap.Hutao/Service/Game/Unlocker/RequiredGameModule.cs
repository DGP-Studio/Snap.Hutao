// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Unlocker;

internal readonly struct RequiredGameModule
{
    public readonly bool HasValue = false;
    public readonly Module UnityPlayer;
    public readonly Module UserAssembly;

    public RequiredGameModule(in Module unityPlayer, in Module userAssembly)
    {
        HasValue = true;
        UnityPlayer = unityPlayer;
        UserAssembly = userAssembly;
    }
}