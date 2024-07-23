// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using System.IO;

namespace Snap.Hutao.Service.Game.Package;

internal readonly struct GamePackageOperationContext
{
    public readonly GamePackageOperationState State;
    public readonly bool IsOversea;
    public readonly string GameDirectory;
    public readonly GameAudioSystem GameAudioSystem;
    public readonly string ChunkDirectory;
    public readonly BranchWrapper LocalBranch;
    public readonly BranchWrapper RemoteBranch;

    public GamePackageOperationContext(
        GamePackageOperationState state,
        bool isOversea,
        string gameDirectory,
        GameAudioSystem gameAudioSystem,
        BranchWrapper localBranch,
        BranchWrapper remoteBranch)
    {
        State = state;
        IsOversea = isOversea;
        GameDirectory = gameDirectory;
        GameAudioSystem = gameAudioSystem;
        ChunkDirectory = Path.Combine(GameDirectory, "chunks");
        LocalBranch = localBranch;
        RemoteBranch = remoteBranch;
    }
}
