// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using System.IO;

namespace Snap.Hutao.Service.Game.Package;

internal readonly struct GamePackageOperationContext
{
    public readonly GamePackageOperationState State;
    public readonly GameFileSystem GameFileSystem;
    public readonly BranchWrapper LocalBranch;
    public readonly BranchWrapper RemoteBranch;

    public GamePackageOperationContext(
        GamePackageOperationState state,
        GameFileSystem gameFileSystem,
        BranchWrapper localBranch,
        BranchWrapper remoteBranch)
    {
        State = state;
        GameFileSystem = gameFileSystem;
        LocalBranch = localBranch;
        RemoteBranch = remoteBranch;
    }
}
