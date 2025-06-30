// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Package.Advanced.Model;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Package.Advanced.AssetOperation;

internal interface IGameAssetOperation
{
    ValueTask InstallAssetsAsync(GamePackageServiceContext context, SophonDecodedBuild remoteBuild);

    ValueTask<GamePackageIntegrityInfo> VerifyGamePackageIntegrityAsync(GamePackageServiceContext context, SophonDecodedBuild build);

    ValueTask RepairGamePackageAsync(GamePackageServiceContext context, GamePackageIntegrityInfo info);

    ValueTask UpdateDiffAssetsAsync(GamePackageServiceContext context, ImmutableArray<SophonAssetOperation> diffAssets);

    ValueTask PredownloadDiffAssetsAsync(GamePackageServiceContext context, ImmutableArray<SophonAssetOperation> diffAssets);

    ValueTask EnsureChannelSdkAsync(GamePackageServiceContext context);

    ValueTask InstallOrPatchAssetsAsync(GamePackageServiceContext context, SophonDecodedPatchBuild patchBuild);

    ValueTask DeletePatchDeprecatedFilesAsync(GamePackageServiceContext context, SophonDecodedPatchBuild patchBuild);

    ValueTask PredownloadPatchesAsync(GamePackageServiceContext context, SophonDecodedPatchBuild patchBuild);
}