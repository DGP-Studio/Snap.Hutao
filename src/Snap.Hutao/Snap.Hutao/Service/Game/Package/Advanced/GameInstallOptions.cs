// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.Downloader;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed class GameInstallOptions : IDeconstruct<IGameFileSystem, LaunchScheme>, IDeconstruct<IGameFileSystem, LaunchScheme, SophonBuild>
{
    private readonly IGameFileSystem gameFileSystem;
    private readonly LaunchScheme launchScheme;
    private readonly SophonBuild? betaBuild;

    public GameInstallOptions(IGameFileSystem gameFileSystem, LaunchScheme launchScheme)
    {
        IsBeta = false;
        this.gameFileSystem = gameFileSystem;
        this.launchScheme = launchScheme;

        betaBuild = default!;
    }

    public GameInstallOptions(IGameFileSystem gameFileSystem, LaunchScheme launchScheme, SophonBuild betaBuild)
    {
        IsBeta = true;
        this.gameFileSystem = gameFileSystem;
        this.launchScheme = launchScheme;

        this.betaBuild = betaBuild;
    }

    [MemberNotNullWhen(true, nameof(betaBuild))]
    public bool IsBeta { get; }

    public void Deconstruct(out IGameFileSystem gameFileSystem, out LaunchScheme launchScheme)
    {
        gameFileSystem = this.gameFileSystem;
        launchScheme = this.launchScheme;
    }

    public void Deconstruct(out IGameFileSystem gameFileSystem, out LaunchScheme launchScheme, out SophonBuild betaBuild)
    {
        gameFileSystem = this.gameFileSystem;
        launchScheme = this.launchScheme;
        HutaoException.ThrowIfNot(IsBeta, "Deconstruct to beta build when not a beta install");
        betaBuild = this.betaBuild;
    }
}