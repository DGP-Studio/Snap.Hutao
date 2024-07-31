// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal interface IGameAssetsOperationService
{
    ValueTask InstallAssetsAsync(SophonDecodedBuild remoteBuild, GamePackageServiceContext context);

    ValueTask<GamePackageIntegrityInfo> VerifyGamePackageIntegrityAsync(SophonDecodedBuild build, GamePackageServiceContext context);

    ValueTask RepairGamePackageAsync(GamePackageIntegrityInfo info, GamePackageServiceContext context);

    ValueTask UpdateDiffAssetsAsync(List<SophonAssetOperation> diffAssets, GamePackageServiceContext context);

    ValueTask PredownloadDiffAssetsAsync(List<SophonAssetOperation> diffAssets, GamePackageServiceContext context);

    ValueTask EnsureChannelSdkAsync(GamePackageServiceContext context);
}