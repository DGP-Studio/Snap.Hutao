// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Package.Advanced.Model;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced.PackageOperation;

[Service(ServiceLifetime.Transient, typeof(IGamePackageOperation), Key = GamePackageOperationKind.Update)]
internal sealed class GamePackageUpdateOperation : GamePackageOperation
{
    public override async ValueTask ExecuteAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild remoteBuild = context.Operation.RemoteBuild;
        ImmutableArray<SophonAssetOperation> diffAssets = context.Information.DiffAssetOperations;
        int downloadTotalChunks = context.Information.DownloadTotalChunks;
        int installTotalChunks = context.Information.InstallTotalChunks;
        long downloadTotalBytes = context.Information.DownloadTotalBytes;
        long installTotalBytes = context.Information.InstallTotalBytes;

        InitializeDuplicatedChunkNames(context, diffAssets.SelectMany(a => a.DiffChunks.Select(c => c.AssetChunk)));

        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedUpdating, downloadTotalChunks, installTotalChunks, downloadTotalBytes, installTotalBytes));

        if (context.Operation.PatchBuild is { } patchBuild)
        {
            await context.Operation.Asset.InstallOrPatchAssetsAsync(context, patchBuild).ConfigureAwait(false);
            await context.Operation.Asset.DeletePatchDeprecatedFilesAsync(context, patchBuild).ConfigureAwait(false);
        }
        else
        {
            await context.Operation.Asset.UpdateDiffAssetsAsync(context, diffAssets).ConfigureAwait(false);
        }

        await context.Operation.Asset.EnsureChannelSdkAsync(context).ConfigureAwait(false);

        await PrivateVerifyAndRepairAsync(context, remoteBuild, remoteBuild.UncompressedTotalBytes, remoteBuild.TotalChunks).ConfigureAwait(false);

        context.Operation.GameFileSystem.TryUpdateConfigurationFile(remoteBuild.Tag);

        if (Directory.Exists(context.Operation.EffectiveChunksDirectory))
        {
            Directory.Delete(context.Operation.EffectiveChunksDirectory, true);
        }
    }
}