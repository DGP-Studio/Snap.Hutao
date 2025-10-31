// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Package.Advanced.Model;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced.PackageOperation;

[Service(ServiceLifetime.Transient, typeof(IGamePackageOperation), Key = GamePackageOperationKind.Predownload)]
internal sealed partial class GamePackagePredownloadOperation : GamePackageOperation
{
    private readonly JsonSerializerOptions jsonOptions;

    [GeneratedConstructor]
    public partial GamePackagePredownloadOperation(IServiceProvider serviceProvider);

    public override async ValueTask ExecuteAsync(GamePackageServiceContext context)
    {
        int downloadTotalChunks = context.Information.DownloadTotalChunks;
        long downloadTotalBytes = context.Information.DownloadTotalBytes;
        int uniqueTotalBlocks = GetUniqueTotalBlocks(context);

        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedPredownloading, downloadTotalChunks, 0, downloadTotalBytes));

        if (!Directory.Exists(context.Operation.GameFileSystem.GetChunksDirectory()))
        {
            Directory.CreateDirectory(context.Operation.GameFileSystem.GetChunksDirectory());
        }

        SophonDecodedBuild? remoteBuild = context.Operation.RemoteBuild;
        ArgumentNullException.ThrowIfNull(remoteBuild);

        PredownloadStatus predownloadStatus = new(remoteBuild.Tag, false, uniqueTotalBlocks);
        using (FileStream predownloadStatusStream = File.Create(context.Operation.GameFileSystem.GetPredownloadStatusFilePath()))
        {
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions).ConfigureAwait(false);
        }

        if (context.Operation.PatchBuild is { } patchBuild)
        {
            await context.Operation.Asset.PredownloadPatchesAsync(context, patchBuild).ConfigureAwait(false);
        }
        else
        {
            await context.Operation.Asset.PredownloadDiffAssetsAsync(context, context.Information.DiffAssetOperations).ConfigureAwait(false);
        }

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind));

        using (FileStream predownloadStatusStream = File.Create(context.Operation.GameFileSystem.GetPredownloadStatusFilePath()))
        {
            predownloadStatus.Finished = true;
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions).ConfigureAwait(false);
        }
    }

    private static int GetUniqueTotalBlocks(GamePackageServiceContext context)
    {
        if (context.Operation.RemoteBuild is not null)
        {
            return context.Information.DownloadTotalChunks;
        }

        HashSet<string> uniqueChunkNames = [];
        foreach (ref readonly SophonAssetOperation asset in context.Information.DiffAssetOperations.AsSpan())
        {
            switch (asset.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair:
                    foreach (AssetChunk chunk in asset.NewAsset.AssetChunks)
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