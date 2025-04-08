// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced.PackageOperation;

[Injection(InjectAs.Transient, typeof(IGamePackageOperation), Key = GamePackageOperationKind.Update)]
internal sealed class GamePackageUpdateOperation : GamePackageOperation
{
    public override async ValueTask ExecuteAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild remoteBuild = context.Operation.RemoteBuild;
        ImmutableArray<SophonAssetOperation> diffAssets = context.Information.DiffAssetOperations;
        int downloadTotalChunks = context.Information.DownloadTotalChunks;
        int installTotalChunks = context.Information.InstallTotalChunks;
        long totalBytes = context.Information.InstallTotalBytes;

        InitializeDuplicatedChunkNames(context, diffAssets.SelectMany(a => a.DiffChunks.Select(c => c.AssetChunk)));

        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedUpdating, downloadTotalChunks, installTotalChunks, totalBytes));

        await context.Operation.Asset.UpdateDiffAssetsAsync(context, diffAssets).ConfigureAwait(false);
        await context.Operation.Asset.EnsureChannelSdkAsync(context).ConfigureAwait(false);

        await PrivateVerifyAndRepairAsync(context, remoteBuild, remoteBuild.TotalBytes, remoteBuild.TotalChunks).ConfigureAwait(false);

        context.Operation.GameFileSystem.TryUpdateConfigurationFile(remoteBuild.Tag);

        if (Directory.Exists(context.Operation.EffectiveChunksDirectory))
        {
            Directory.Delete(context.Operation.EffectiveChunksDirectory, true);
        }
    }
}