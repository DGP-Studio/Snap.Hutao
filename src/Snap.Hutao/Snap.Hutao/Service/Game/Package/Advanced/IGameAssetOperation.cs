// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal interface IGameAssetOperation
{
    HashSet<string> DuplicatingChunkNames { get; set; }

    ValueTask InstallAssetsAsync(GamePackageServiceContext context, SophonDecodedBuild remoteBuild, CancellationToken token = default);

    ValueTask<GamePackageIntegrityInfo> VerifyGamePackageIntegrityAsync(GamePackageServiceContext context, SophonDecodedBuild build, CancellationToken token = default);

    ValueTask RepairGamePackageAsync(GamePackageServiceContext context, GamePackageIntegrityInfo info, CancellationToken token = default);

    ValueTask UpdateDiffAssetsAsync(GamePackageServiceContext context, List<SophonAssetOperation> diffAssets, CancellationToken token = default);

    ValueTask PredownloadDiffAssetsAsync(GamePackageServiceContext context, List<SophonAssetOperation> diffAssets, CancellationToken token = default);

    ValueTask EnsureChannelSdkAsync(GamePackageServiceContext context, CancellationToken token = default);
}