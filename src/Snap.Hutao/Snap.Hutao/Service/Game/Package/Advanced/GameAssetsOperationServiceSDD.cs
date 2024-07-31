// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Buffers;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Transient)]
internal sealed partial class GameAssetsOperationServiceSSD : GameAssetsOperationService
{
    public override async ValueTask InstallAssetsAsync(SophonDecodedBuild remoteBuild, GamePackageServiceContext context)
    {
        await Parallel.ForEachAsync(remoteBuild.Manifests, context.ParallelOptions, async (manifest, token) =>
        {
            IEnumerable<SophonAssetOperation> assets = manifest.ManifestProto.Assets.Select(asset => SophonAssetOperation.AddOrRepair(manifest.UrlPrefix, asset));
            await Parallel.ForEachAsync(assets, context.ParallelOptions, (asset, token) => DownloadAndMergeAssetAsync(asset, context)).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public override async ValueTask UpdateDiffAssetsAsync(List<SophonAssetOperation> diffAssets, GamePackageServiceContext context)
    {
        await Parallel.ForEachAsync(diffAssets, context.ParallelOptions, (asset, token) => asset.Type switch
        {
            SophonAssetOperationType.AddOrRepair or SophonAssetOperationType.Modify => DownloadAndMergeAssetAsync(asset, context),
            SophonAssetOperationType.Delete => DeleteAssetsAsync(diffAssets.Select(a => a.OldAsset), context),
            _ => ValueTask.CompletedTask,
        }).ConfigureAwait(false);
    }

    public override async ValueTask PredownloadDiffAssetsAsync(List<SophonAssetOperation> diffAssets, GamePackageServiceContext context)
    {
        await Parallel.ForEachAsync(diffAssets, context.ParallelOptions, (asset, token) =>
        {
            IEnumerable<SophonChunk> chunks = asset.Type switch
            {
                SophonAssetOperationType.AddOrRepair => asset.NewAsset.AssetChunks.Select(c => new SophonChunk(asset.UrlPrefix, c)),
                SophonAssetOperationType.Modify => asset.DiffChunks,
                _ => [],
            };

            return DownloadChunksAsync(chunks, context);
        }).ConfigureAwait(false);
    }

    protected override async ValueTask VerifyManifestsAsync(SophonDecodedBuild build, List<SophonAssetOperation> conflictedAssets, GamePackageServiceContext context)
    {
        await Parallel.ForEachAsync(build.Manifests, context.ParallelOptions, (manifest, token) => VerifyManifestAsync(manifest, conflictedAssets, context)).ConfigureAwait(false);
    }

    protected override async ValueTask VerifyManifestAsync(SophonDecodedManifest manifest, List<SophonAssetOperation> conflictedAssets, GamePackageServiceContext context)
    {
        await Parallel.ForEachAsync(manifest.ManifestProto.Assets, context.ParallelOptions, (asset, token) => VerifyAssetAsync(new(manifest.UrlPrefix, asset), conflictedAssets, context)).ConfigureAwait(false);
    }

    protected override async ValueTask RepairAssetsAsync(GamePackageIntegrityInfo info, GamePackageServiceContext context)
    {
        await Parallel.ForEachAsync(info.ConflictedAssets, context.ParallelOptions, (asset, token) => DownloadAndMergeAssetAsync(asset, context)).ConfigureAwait(false);
    }

    protected override async ValueTask DownloadChunksAsync(IEnumerable<SophonChunk> sophonChunks, GamePackageServiceContext context)
    {
        await Parallel.ForEachAsync(sophonChunks, context.ParallelOptions, (chunk, token) => DownloadChunkAsync(chunk, context)).ConfigureAwait(false);
    }

    protected override async ValueTask MergeNewAssetAsync(AssetProperty assetProperty, GamePackageServiceContext context)
    {
        string path = Path.Combine(context.Operation.GameFileSystem.GameDirectory, assetProperty.AssetName);
        string? directory = Path.GetDirectoryName(path);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);

        using (SafeFileHandle fileHandle = File.OpenHandle(path, FileMode.Create, FileAccess.Write, FileShare.None, preallocationSize: 32 * 1024))
        {
            await Parallel.ForEachAsync(assetProperty.AssetChunks, context.ParallelOptions, (chunk, token) => MergeChunkIntoAssetAsync(fileHandle, chunk, context)).ConfigureAwait(false);
        }
    }

    private async ValueTask MergeChunkIntoAssetAsync(SafeFileHandle fileHandle, AssetChunk chunk, GamePackageServiceContext context)
    {
        CancellationToken token = context.ParallelOptions.CancellationToken;

        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(81920))
        {
            Memory<byte> buffer = memoryOwner.Memory;

            string chunkPath = Path.Combine(context.Operation.GameFileSystem.ChunksDirectory, chunk.ChunkName);
            if (!File.Exists(chunkPath))
            {
                return;
            }

            using (FileStream chunkFile = File.OpenRead(chunkPath))
            {
                using (ZstandardDecompressionStream decompressionStream = new(chunkFile))
                {
                    long offset = chunk.ChunkOnFileOffset;
                    do
                    {
                        int bytesRead = await decompressionStream.ReadAsync(buffer, token).ConfigureAwait(false);
                        if (bytesRead <= 0)
                        {
                            break;
                        }

                        await RandomAccess.WriteAsync(fileHandle, buffer[..bytesRead], offset, token).ConfigureAwait(false);
                        offset += bytesRead;
                    }
                    while (true);
                }
            }
        }
    }
}