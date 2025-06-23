// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Package.Advanced.Model;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Game.Package.Advanced.PackageOperation;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IGamePackageOperation), Key = GamePackageOperationKind.Predownload)]
internal sealed partial class GamePackagePredownloadOperation : GamePackageOperation
{
    private readonly JsonSerializerOptions jsonOptions;

    public override async ValueTask ExecuteAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild remoteBuild = context.Operation.RemoteBuild;
        ImmutableArray<SophonAssetOperation> diffAssets = context.Information.DiffAssetOperations;
        int totalBlocks = context.Information.DownloadTotalChunks;
        long totalBytes = context.Information.InstallTotalBytes;

        int uniqueTotalBlocks = GetUniqueTotalBlocks(diffAssets);

        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedPredownloading, totalBlocks, 0, totalBytes));

        if (!Directory.Exists(context.Operation.GameFileSystem.GetChunksDirectory()))
        {
            Directory.CreateDirectory(context.Operation.GameFileSystem.GetChunksDirectory());
        }

        PredownloadStatus predownloadStatus = new(remoteBuild.Tag, false, uniqueTotalBlocks);
        using (FileStream predownloadStatusStream = File.Create(context.Operation.GameFileSystem.GetPredownloadStatusPath()))
        {
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions).ConfigureAwait(false);
        }

        await context.Operation.Asset.PredownloadDiffAssetsAsync(context, diffAssets).ConfigureAwait(false);

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind));

        using (FileStream predownloadStatusStream = File.Create(context.Operation.GameFileSystem.GetPredownloadStatusPath()))
        {
            predownloadStatus.Finished = true;
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions).ConfigureAwait(false);
        }
    }

    private static int GetUniqueTotalBlocks(ImmutableArray<SophonAssetOperation> assets)
    {
        HashSet<string> uniqueChunkNames = [];
        foreach (ref readonly SophonAssetOperation asset in assets.AsSpan())
        {
            switch (asset.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair:
                    foreach (ref readonly AssetChunk chunk in CollectionsMarshal.AsSpan(asset.NewAsset.AssetChunks.ToList()))
                    {
                        uniqueChunkNames.Add(chunk.ChunkName);
                    }

                    break;
                case SophonAssetOperationKind.Modify:
                    foreach (ref readonly SophonChunk diffChunk in asset.DiffChunks.AsSpan())
                    {
                        uniqueChunkNames.Add(diffChunk.AssetChunk.ChunkName);
                    }

                    break;
            }
        }

        return uniqueChunkNames.Count;
    }
}