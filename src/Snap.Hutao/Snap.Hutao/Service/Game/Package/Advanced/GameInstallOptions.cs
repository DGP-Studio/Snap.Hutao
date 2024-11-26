// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Service.Game.Scheme;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal readonly struct GameInstallOptions : IDeconstruct<IGameFileSystem, LaunchScheme>
{
    public readonly IGameFileSystem GameFileSystem;
    public readonly LaunchScheme LaunchScheme;

    public GameInstallOptions(IGameFileSystem gameFileSystem, LaunchScheme launchScheme)
    {
        GameFileSystem = gameFileSystem;
        LaunchScheme = launchScheme;
    }

    public void Deconstruct(out IGameFileSystem gameFileSystem, out LaunchScheme launchScheme)
    {
        gameFileSystem = GameFileSystem;
        launchScheme = LaunchScheme;
    }
}