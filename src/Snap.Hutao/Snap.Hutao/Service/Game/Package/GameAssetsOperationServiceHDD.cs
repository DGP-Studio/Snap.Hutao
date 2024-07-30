// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Buffers;
using System.IO;

namespace Snap.Hutao.Service.Game.Package;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Transient)]
[SuppressMessage("", "CA2000")]
internal sealed partial class GameAssetsOperationServiceHDD : GameAssetsOperationService
{
    public override async ValueTask InstallAssetsAsync(SophonDecodedBuild remoteBuild, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions options)
    {
        foreach (SophonDecodedManifest manifest in remoteBuild.Manifests)
        {
            IEnumerable<SophonAsset> assets = manifest.ManifestProto.Assets.Select(asset => new SophonAsset(manifest.UrlPrefix, asset));
            await AddAssetsAsync(assets, context, progress, options).ConfigureAwait(false);
        }
    }

    public override async ValueTask UpdateModifiedAssetsAsync(List<SophonModifiedAsset> modifiedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        foreach (SophonModifiedAsset asset in modifiedAssets)
        {
            await DownloadChunksAsync(asset.DiffChunks, context, progress, parallelOptions).ConfigureAwait(false);
            await MergeDiffAssetAsync(asset, context, parallelOptions.CancellationToken).ConfigureAwait(false);
        }
    }

    public override async ValueTask PredownloadAddedAssetsAsync(List<SophonAsset> addedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        foreach (SophonAsset asset in addedAssets)
        {
            await DownloadChunksAsync(asset.AssetProperty.AssetChunks.Select(chunk => new SophonChunk(asset.UrlPrefix, chunk)), context, progress, parallelOptions).ConfigureAwait(false);
        }
    }

    public override async ValueTask PredownloadModifiedAssetsAsync(List<SophonModifiedAsset> modifiedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        foreach (SophonModifiedAsset asset in modifiedAssets)
        {
            await DownloadChunksAsync(asset.DiffChunks, context, progress, parallelOptions).ConfigureAwait(false);
        }
    }

    public override async ValueTask AddAssetsAsync(IEnumerable<SophonAsset> newAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        foreach (SophonAsset asset in newAssets)
        {
            await AddNewAssetAsync(asset, context, progress, parallelOptions).ConfigureAwait(false);
        }
    }

    protected override async ValueTask VerifyManifestsAsync(SophonDecodedBuild build, List<SophonAsset> conflictedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        foreach (SophonDecodedManifest manifest in build.Manifests)
        {
            await VerifyManifestAsync(manifest, conflictedAssets, context, progress, parallelOptions).ConfigureAwait(false);
        }
    }

    protected override async ValueTask VerifyManifestAsync(SophonDecodedManifest manifest, List<SophonAsset> conflictedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        foreach (AssetProperty asset in manifest.ManifestProto.Assets)
        {
            await VerifyAssetAsync(new(manifest.UrlPrefix, asset), conflictedAssets, context, progress, parallelOptions.CancellationToken).ConfigureAwait(false);
        }
    }

    protected override async ValueTask RepairAssetsAsync(GamePackageIntegrityInfo info, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        foreach (SophonAsset asset in info.ConflictedAssets)
        {
            await DownloadChunksAsync(asset.AssetProperty.AssetChunks.Select(chunk => new SophonChunk(asset.UrlPrefix, chunk)), context, progress, parallelOptions).ConfigureAwait(false);
            await MergeAssetAsync(asset.AssetProperty, context, parallelOptions).ConfigureAwait(false);
        }
    }

    protected override async ValueTask DownloadChunksAsync(IEnumerable<SophonChunk> sophonChunks, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        foreach (SophonChunk chunk in sophonChunks)
        {
            await DownloadChunkAsync(chunk, context, progress, parallelOptions.CancellationToken).ConfigureAwait(false);
        }
    }

    protected override async ValueTask MergeAssetAsync(AssetProperty assetProperty, GamePackageOperationContext context, ParallelOptions parallelOptions)
    {
        string path = Path.Combine(context.GameFileSystem.GameDirectory, assetProperty.AssetName);
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
                    string chunkPath = Path.Combine(context.GameFileSystem.ChunksDirectory, chunk.ChunkName);
                    using (FileStream chunkFile = File.OpenRead(chunkPath))
                    {
                        using (ZstandardDecompressionStream decompressionStream = new(chunkFile))
                        {
                            long offset = chunk.ChunkOnFileOffset;
                            do
                            {
                                int bytesRead = await decompressionStream.ReadAsync(buffer, parallelOptions.CancellationToken).ConfigureAwait(false);
                                if (bytesRead <= 0)
                                {
                                    break;
                                }

                                await RandomAccess.WriteAsync(fileHandle, buffer[..bytesRead], offset, parallelOptions.CancellationToken).ConfigureAwait(false);
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