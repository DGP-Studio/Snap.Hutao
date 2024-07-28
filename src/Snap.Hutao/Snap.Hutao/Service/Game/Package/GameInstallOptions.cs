// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Service.Game.Scheme;

namespace Snap.Hutao.Service.Game.Package;

internal readonly struct GameInstallOptions : IDeconstruct<GameFileSystem, LaunchScheme>
{
    public readonly GameFileSystem GameFileSystem;
    public readonly LaunchScheme LaunchScheme;

    public GameInstallOptions(GameFileSystem gameFileSystem, LaunchScheme launchScheme)
    {
        GameFileSystem = gameFileSystem;
        LaunchScheme = launchScheme;
    }

    public void Deconstruct(out GameFileSystem gameFileSystem, out LaunchScheme launchScheme)
    {
        gameFileSystem = GameFileSystem;
        launchScheme = LaunchScheme;
    }
}
