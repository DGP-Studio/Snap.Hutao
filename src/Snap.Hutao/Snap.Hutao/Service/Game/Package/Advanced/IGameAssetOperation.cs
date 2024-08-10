// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal interface IGameAssetOperation
{
    HashSet<string> DuplicatingChunkNames { get; set; }

    ValueTask InstallAssetsAsync(GamePackageServiceContext context, SophonDecodedBuild remoteBuild);

    ValueTask<GamePackageIntegrityInfo> VerifyGamePackageIntegrityAsync(GamePackageServiceContext context, SophonDecodedBuild build);

    ValueTask RepairGamePackageAsync(GamePackageServiceContext context, GamePackageIntegrityInfo info);

    ValueTask UpdateDiffAssetsAsync(GamePackageServiceContext context, List<SophonAssetOperation> diffAssets);

    ValueTask PredownloadDiffAssetsAsync(GamePackageServiceContext context, List<SophonAssetOperation> diffAssets);

    ValueTask EnsureChannelSdkAsync(GamePackageServiceContext context);
}