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
internal sealed partial class GameAssetsOperationServiceSSD : GameAssetsOperationService
{
    public override async ValueTask InstallAssetsAsync(SophonDecodedBuild remoteBuild, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions options)
    {
        await Parallel.ForEachAsync(remoteBuild.Manifests, options, async (manifest, token) =>
        {
            IEnumerable<SophonAsset> assets = manifest.ManifestProto.Assets.Select(asset => new SophonAsset(manifest.UrlPrefix, asset));
            await AddAssetsAsync(assets, context, progress, options).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public override async ValueTask UpdateModifiedAssetsAsync(List<SophonModifiedAsset> modifiedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        await Parallel.ForEachAsync(modifiedAssets, parallelOptions, async (asset, token) =>
        {
            await DownloadChunksAsync(asset.DiffChunks, context, progress, parallelOptions).ConfigureAwait(false);
            await MergeDiffAssetAsync(asset, context, token).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public override async ValueTask PredownloadAddedAssetsAsync(List<SophonAsset> addedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        await Parallel.ForEachAsync(addedAssets, parallelOptions, (asset, token) => DownloadChunksAsync(asset.AssetProperty.AssetChunks.Select(chunk => new SophonChunk(asset.UrlPrefix, chunk)), context, progress, parallelOptions)).ConfigureAwait(false);
    }

    public override async ValueTask PredownloadModifiedAssetsAsync(List<SophonModifiedAsset> modifiedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        await Parallel.ForEachAsync(modifiedAssets, parallelOptions, (asset, token) => DownloadChunksAsync(asset.DiffChunks, context, progress, parallelOptions)).ConfigureAwait(false);
    }

    public override async ValueTask AddAssetsAsync(IEnumerable<SophonAsset> newAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        await Parallel.ForEachAsync(newAssets, parallelOptions, (asset, token) => AddNewAssetAsync(asset, context, progress, parallelOptions)).ConfigureAwait(false);
    }

    protected override async ValueTask VerifyManifestsAsync(SophonDecodedBuild build, List<SophonAsset> conflictedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        await Parallel.ForEachAsync(build.Manifests, parallelOptions, (manifest, token) => VerifyManifestAsync(manifest, conflictedAssets, context, progress, parallelOptions)).ConfigureAwait(false);
    }

    protected override async ValueTask VerifyManifestAsync(SophonDecodedManifest manifest, List<SophonAsset> conflictedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        await Parallel.ForEachAsync(manifest.ManifestProto.Assets, parallelOptions, (asset, token) => VerifyAssetAsync(new(manifest.UrlPrefix, asset), conflictedAssets, context, progress, token)).ConfigureAwait(false);
    }

    protected override async ValueTask RepairAssetsAsync(GamePackageIntegrityInfo info, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        await Parallel.ForEachAsync(info.ConflictedAssets, parallelOptions, async (asset, token) =>
        {
            await DownloadChunksAsync(asset.AssetProperty.AssetChunks.Select(chunk => new SophonChunk(asset.UrlPrefix, chunk)), context, progress, parallelOptions).ConfigureAwait(false);
            await MergeAssetAsync(asset.AssetProperty, context, parallelOptions).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    protected override async ValueTask DownloadChunksAsync(IEnumerable<SophonChunk> sophonChunks, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        await Parallel.ForEachAsync(sophonChunks, parallelOptions, (chunk, token) => DownloadChunkAsync(chunk, context, progress, token)).ConfigureAwait(false);
    }

    protected override async ValueTask MergeAssetAsync(AssetProperty assetProperty, GamePackageOperationContext context, ParallelOptions parallelOptions)
    {
        string path = Path.Combine(context.GameFileSystem.GameDirectory, assetProperty.AssetName);
        string? directory = Path.GetDirectoryName(path);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);

        using (SafeFileHandle fileHandle = File.OpenHandle(path, FileMode.Create, FileAccess.Write, FileShare.None, preallocationSize: 32 * 1024))
        {
            await Parallel.ForEachAsync(assetProperty.AssetChunks, parallelOptions, (chunk, token) => MergeChunkIntoAssetAsync(fileHandle, chunk, context, token)).ConfigureAwait(false);
        }
    }

    private static async ValueTask MergeChunkIntoAssetAsync(SafeFileHandle fileHandle, AssetChunk chunk, GamePackageOperationContext context, CancellationToken token = default)
    {
        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(81920))
        {
            Memory<byte> buffer = memoryOwner.Memory;

            string chunkPath = Path.Combine(context.GameFileSystem.ChunksDirectory, chunk.ChunkName);
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