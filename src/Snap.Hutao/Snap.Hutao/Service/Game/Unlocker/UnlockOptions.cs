// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Unlocker;

internal readonly struct UnlockOptions
{
    public readonly GameFileSystem GameFileSystem;
    public readonly TimeSpan FindModuleDelay;
    public readonly TimeSpan FindModuleLimit;
    public readonly TimeSpan AdjustFpsDelay;

    public UnlockOptions(GameFileSystem gameFileSystem, int findModuleDelayMilliseconds, int findModuleLimitMilliseconds, int adjustFpsDelayMilliseconds)
    {
        GameFileSystem = gameFileSystem;
        FindModuleDelay = TimeSpan.FromMilliseconds(findModuleDelayMilliseconds);
        FindModuleLimit = TimeSpan.FromMilliseconds(findModuleLimitMilliseconds);
        AdjustFpsDelay = TimeSpan.FromMilliseconds(adjustFpsDelayMilliseconds);
    }
}