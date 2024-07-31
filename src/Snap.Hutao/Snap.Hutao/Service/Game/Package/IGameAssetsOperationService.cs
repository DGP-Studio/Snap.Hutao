// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Package;

internal interface IGameAssetsOperationService
{
    ValueTask InstallAssetsAsync(SophonDecodedBuild remoteBuild, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions options);

    ValueTask<GamePackageIntegrityInfo> VerifyGamePackageIntegrityAsync(SophonDecodedBuild build, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, CancellationToken token = default);

    ValueTask RepairGamePackageAsync(GamePackageIntegrityInfo info, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, CancellationToken token = default);

    ValueTask UpdateDiffAssetsAsync(List<SophonAssetOperation> diffAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions);

    ValueTask PredownloadDiffAssetsAsync(List<SophonAssetOperation> diffAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions);

    ValueTask ExtractChannelSdkAsync(GamePackageOperationContext context, CancellationToken token = default);
}