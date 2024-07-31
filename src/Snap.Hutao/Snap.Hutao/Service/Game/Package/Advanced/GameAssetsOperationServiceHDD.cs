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
[SuppressMessage("", "CA2000")]
internal sealed partial class GameAssetsOperationServiceHDD : GameAssetsOperationService
{
    public override async ValueTask InstallAssetsAsync(SophonDecodedBuild remoteBuild, GamePackageServiceContext context)
    {
        foreach (SophonDecodedManifest manifest in remoteBuild.Manifests)
        {
            IEnumerable<SophonAssetOperation> assets = manifest.ManifestProto.Assets.Select(asset => SophonAssetOperation.AddOrRepair(manifest.UrlPrefix, asset));
            foreach (SophonAssetOperation asset in assets)
            {
                await DownloadAndMergeAssetAsync(asset, context).ConfigureAwait(false);
            }
        }
    }

    public override async ValueTask UpdateDiffAssetsAsync(List<SophonAssetOperation> diffAssets, GamePackageServiceContext context)
    {
        foreach (SophonAssetOperation asset in diffAssets)
        {
            ValueTask task = asset.Type switch
            {
                SophonAssetOperationType.AddOrRepair or SophonAssetOperationType.Modify => DownloadAndMergeAssetAsync(asset, context),
                SophonAssetOperationType.Delete => DeleteAssetsAsync(diffAssets.Select(a => a.OldAsset), context),
                _ => ValueTask.CompletedTask,
            };

            await task.ConfigureAwait(false);
        }
    }

    public override async ValueTask PredownloadDiffAssetsAsync(List<SophonAssetOperation> diffAssets, GamePackageServiceContext context)
    {
        foreach (SophonAssetOperation asset in diffAssets)
        {
            IEnumerable<SophonChunk> chunks = asset.Type switch
            {
                SophonAssetOperationType.AddOrRepair => asset.NewAsset.AssetChunks.Select(c => new SophonChunk(asset.UrlPrefix, c)),
                SophonAssetOperationType.Modify => asset.DiffChunks,
                _ => [],
            };

            await DownloadChunksAsync(chunks, context).ConfigureAwait(false);
        }
    }

    protected override async ValueTask VerifyManifestsAsync(SophonDecodedBuild build, List<SophonAssetOperation> conflictedAssets, GamePackageServiceContext context)
    {
        foreach (SophonDecodedManifest manifest in build.Manifests)
        {
            await VerifyManifestAsync(manifest, conflictedAssets, context).ConfigureAwait(false);
        }
    }

    protected override async ValueTask VerifyManifestAsync(SophonDecodedManifest manifest, List<SophonAssetOperation> conflictedAssets, GamePackageServiceContext context)
    {
        foreach (AssetProperty asset in manifest.ManifestProto.Assets)
        {
            await VerifyAssetAsync(new(manifest.UrlPrefix, asset), conflictedAssets, context).ConfigureAwait(false);
        }
    }

    protected override async ValueTask RepairAssetsAsync(GamePackageIntegrityInfo info, GamePackageServiceContext context)
    {
        foreach (SophonAssetOperation asset in info.ConflictedAssets)
        {
            await DownloadAndMergeAssetAsync(asset, context).ConfigureAwait(false);
        }
    }

    protected override async ValueTask DownloadChunksAsync(IEnumerable<SophonChunk> sophonChunks, GamePackageServiceContext context)
    {
        foreach (SophonChunk chunk in sophonChunks)
        {
            await DownloadChunkAsync(chunk, context).ConfigureAwait(false);
        }
    }

    protected override async ValueTask MergeNewAssetAsync(AssetProperty assetProperty, GamePackageServiceContext context)
    {
        CancellationToken token = context.ParallelOptions.CancellationToken;

        string path = Path.Combine(context.Operation.GameFileSystem.GameDirectory, assetProperty.AssetName);
        string? directory = Path.GetDirectoryName(path);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);

        using (SafeFileHandle fileHandle = File.OpenHandle(path, FileMode.Create, FileAccess.Write, FileShare.None, preallocationSize: 32 * 1024))
        {
            using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(81920))
            {
                Memory<byte> buffer = memoryOwner.Memory;

                foreach (AssetChunk chunk in assetProperty.AssetChunks)
                {
                    string chunkPath = Path.Combine(context.Operation.GameFileSystem.ChunksDirectory, chunk.ChunkName);
                    if (!File.Exists(chunkPath))
                    {
                        continue;
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
    }
}