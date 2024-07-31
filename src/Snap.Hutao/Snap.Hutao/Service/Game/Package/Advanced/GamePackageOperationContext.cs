// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal readonly struct GamePackageOperationContext
{
    public readonly GamePackageOperationKind OperationKind;
    public readonly IGameAssetsOperationService GameAssetsOperationService;
    public readonly GameFileSystem GameFileSystem;
    public readonly BranchWrapper LocalBranch;
    public readonly BranchWrapper RemoteBranch;
    public readonly GameChannelSDK? GameChannelSDK;

    public GamePackageOperationContext(
        IServiceProvider serviceProvider,
        GamePackageOperationKind kind,
        GameFileSystem gameFileSystem,
        BranchWrapper localBranch,
        BranchWrapper remoteBranch,
        GameChannelSDK? gameChannelSDK)
    {
        OperationKind = kind;
        GameAssetsOperationService = serviceProvider.GetRequiredService<IDriverMediaTypeAwareFactory<IGameAssetsOperationService>>().Create(gameFileSystem.GameDirectory);
        GameFileSystem = gameFileSystem;
        LocalBranch = localBranch;
        RemoteBranch = remoteBranch;
        GameChannelSDK = gameChannelSDK;
    }
}