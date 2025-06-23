// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.Downloader;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal readonly struct GameInstallOptions : IDeconstruct<IGameFileSystem, LaunchScheme>, IDeconstruct<IGameFileSystem, LaunchScheme, SophonBuild>
{
    public readonly IGameFileSystem GameFileSystem;
    public readonly LaunchScheme LaunchScheme;

    public readonly bool IsBeta;
    public readonly SophonBuild BetaBuild;

    public GameInstallOptions(IGameFileSystem gameFileSystem, LaunchScheme launchScheme)
    {
        IsBeta = false;
        GameFileSystem = gameFileSystem;
        LaunchScheme = launchScheme;

        BetaBuild = default!;
    }

    public GameInstallOptions(IGameFileSystem gameFileSystem, LaunchScheme launchScheme, SophonBuild betaBuild)
    {
        IsBeta = true;
        GameFileSystem = gameFileSystem;
        LaunchScheme = launchScheme;

        BetaBuild = betaBuild;
    }

    public void Deconstruct(out IGameFileSystem gameFileSystem, out LaunchScheme launchScheme)
    {
        gameFileSystem = GameFileSystem;
        launchScheme = LaunchScheme;
    }

    public void Deconstruct(out IGameFileSystem gameFileSystem, out LaunchScheme launchScheme, out SophonBuild betaBuild)
    {
        gameFileSystem = GameFileSystem;
        launchScheme = LaunchScheme;
        betaBuild = BetaBuild;
    }
}