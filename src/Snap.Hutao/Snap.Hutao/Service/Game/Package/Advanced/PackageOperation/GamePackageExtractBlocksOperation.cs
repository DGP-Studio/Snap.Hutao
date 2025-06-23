// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Service.Game.Package.Advanced.Model;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced.PackageOperation;

[Injection(InjectAs.Transient, typeof(IGamePackageOperation), Key = GamePackageOperationKind.ExtractBlocks)]
internal sealed class GamePackageExtractBlocksOperation : GamePackageOperation
{
    public override async ValueTask ExecuteAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild localBuild = context.Operation.LocalBuild;
        ImmutableArray<SophonAssetOperation> diffAssets = context.Information.DiffAssetOperations;
        int downloadTotalChunks = context.Information.DownloadTotalChunks;
        int installTotalChunks = context.Information.InstallTotalChunks;
        long totalBytes = context.Information.InstallTotalBytes;

        InitializeDuplicatedChunkNames(context, diffAssets.SelectMany(a => a.DiffChunks.Select(c => c.AssetChunk)));

        context.Progress.Report(new GamePackageOperationReport.Reset("Copying", 0, localBuild.TotalChunks, localBuild.UncompressedTotalBytes));
        List<string> usefulChunks = diffAssets
            .Where(ao => ao.Kind is SophonAssetOperationKind.Modify)
            .Select(ao => Path.GetFileName(ao.OldAsset.AssetName))
            .ToList();
        string oldBlksDirectory = Path.Combine(context.Operation.GameFileSystem.GetDataDirectory(), @"StreamingAssets\AssetBundles\blocks");
        foreach (string file in Directory.GetFiles(oldBlksDirectory, "*.blk", SearchOption.AllDirectories))
        {
            string fileName = Path.GetFileName(file);
            if (!usefulChunks.Contains(fileName, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            string newFilePath = Path.Combine(context.Operation.EffectiveGameDirectory, fileName);
            FileOperation.Copy(file, newFilePath, true);
            AssetProperty asset = localBuild.Manifests.Single().Data.Assets.Single(a => a.AssetName.Contains(fileName, StringComparison.OrdinalIgnoreCase));
            context.Progress.Report(new GamePackageOperationReport.Install(asset.AssetSize, asset.AssetChunks.Count));
        }

        context.Progress.Report(new GamePackageOperationReport.Reset("Extracting", downloadTotalChunks, installTotalChunks, totalBytes));
        await context.Operation.Asset.UpdateDiffAssetsAsync(context, diffAssets).ConfigureAwait(false);

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind));
    }
}