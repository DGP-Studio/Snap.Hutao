// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;

namespace Snap.Hutao.Service.Game.Package;

internal readonly struct GamePackageOperationContext
{
    public readonly GamePackageOperationState State;
    public readonly GameFileSystem GameFileSystem;
    public readonly BranchWrapper LocalBranch;
    public readonly BranchWrapper RemoteBranch;
    public readonly GameChannelSDK? GameChannelSDK;

    public GamePackageOperationContext(
        GamePackageOperationState state,
        GameFileSystem gameFileSystem,
        BranchWrapper localBranch,
        BranchWrapper remoteBranch,
        GameChannelSDK? gameChannelSDK)
    {
        State = state;
        GameFileSystem = gameFileSystem;
        LocalBranch = localBranch;
        RemoteBranch = remoteBranch;
        GameChannelSDK = gameChannelSDK;
    }
}
