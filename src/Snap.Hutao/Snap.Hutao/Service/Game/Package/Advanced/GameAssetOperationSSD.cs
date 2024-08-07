// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Buffers;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Transient)]
internal sealed partial class GameAssetOperationSSD : GameAssetOperation
{
    public override async ValueTask InstallAssetsAsync(GamePackageServiceContext context, SophonDecodedBuild remoteBuild, CancellationToken token = default)
    {
        await Parallel.ForEachAsync(remoteBuild.Manifests, context.ParallelOptions, async (manifest, token) =>
        {
            IEnumerable<SophonAssetOperation> assets = manifest.ManifestProto.Assets.Select(asset => SophonAssetOperation.AddOrRepair(manifest.UrlPrefix, asset));
            await Parallel.ForEachAsync(assets, context.ParallelOptions, (asset, token) => EnsureAssetAsync(context, asset, token)).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public override async ValueTask UpdateDiffAssetsAsync(GamePackageServiceContext context, List<SophonAssetOperation> diffAssets, CancellationToken token = default)
    {
        await Parallel.ForEachAsync(diffAssets, context.ParallelOptions, (asset, token) => asset.Kind switch
        {
            SophonAssetOperationKind.AddOrRepair or SophonAssetOperationKind.Modify => EnsureAssetAsync(context, asset, token),
            SophonAssetOperationKind.Delete => DeleteAssetsAsync(context, diffAssets.Select(a => a.OldAsset), token),
            _ => ValueTask.CompletedTask,
        }).ConfigureAwait(false);
    }

    public override async ValueTask PredownloadDiffAssetsAsync(GamePackageServiceContext context, List<SophonAssetOperation> diffAssets, CancellationToken token = default)
    {
        await Parallel.ForEachAsync(diffAssets, context.ParallelOptions, (asset, token) =>
        {
            IEnumerable<SophonChunk> chunks = asset.Kind switch
            {
                SophonAssetOperationKind.AddOrRepair => asset.NewAsset.AssetChunks.Select(c => new SophonChunk(asset.UrlPrefix, c)),
                SophonAssetOperationKind.Modify => asset.DiffChunks,
                _ => [],
            };

            return DownloadChunksAsync(context, chunks, token);
        }).ConfigureAwait(false);
    }

    protected override async ValueTask VerifyManifestsAsync(GamePackageServiceContext context, SophonDecodedBuild build, Action<SophonAssetOperation> conflictHandler, CancellationToken token = default)
    {
        await Parallel.ForEachAsync(build.Manifests, context.ParallelOptions, (manifest, token) => VerifyManifestAsync(context, manifest, conflictHandler, token)).ConfigureAwait(false);
    }

    protected override async ValueTask VerifyManifestAsync(GamePackageServiceContext context, SophonDecodedManifest manifest, Action<SophonAssetOperation> conflictHandler, CancellationToken token = default)
    {
        await Parallel.ForEachAsync(manifest.ManifestProto.Assets, context.ParallelOptions, (asset, token) => VerifyAssetAsync(context, new(manifest.UrlPrefix, asset), conflictHandler, token)).ConfigureAwait(false);
    }

    protected override async ValueTask RepairAssetsAsync(GamePackageServiceContext context, GamePackageIntegrityInfo info, CancellationToken token = default)
    {
        await Parallel.ForEachAsync(info.ConflictedAssets, context.ParallelOptions, (asset, token) => EnsureAssetAsync(context, asset, token)).ConfigureAwait(false);
    }

    protected override async ValueTask DownloadChunksAsync(GamePackageServiceContext context, IEnumerable<SophonChunk> sophonChunks, CancellationToken token = default)
    {
        await Parallel.ForEachAsync(sophonChunks, context.ParallelOptions, (chunk, token) => DownloadChunkAsync(context, chunk, token)).ConfigureAwait(false);
    }

    protected override async ValueTask MergeNewAssetAsync(GamePackageServiceContext context, AssetProperty assetProperty, CancellationToken token = default)
    {
        string path = Path.Combine(context.Operation.GameFileSystem.GameDirectory, assetProperty.AssetName);
        string? directory = Path.GetDirectoryName(path);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);

        using (SafeFileHandle fileHandle = File.OpenHandle(path, FileMode.Create, FileAccess.Write, FileShare.None, preallocationSize: assetProperty.AssetSize))
        {
            await Parallel.ForEachAsync(assetProperty.AssetChunks, context.ParallelOptions, (chunk, token) => MergeChunkIntoAssetAsync(context, fileHandle, chunk, token)).ConfigureAwait(false);
        }
    }

    private async ValueTask MergeChunkIntoAssetAsync(GamePackageServiceContext context, SafeFileHandle fileHandle, AssetChunk chunk, CancellationToken token = default)
    {
        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(81920))
        {
            Memory<byte> buffer = memoryOwner.Memory;

            string chunkPath = Path.Combine(context.Operation.ProxiedChunksDirectory, chunk.ChunkName);
            if (!File.Exists(chunkPath))
            {
                return;
            }

            TaskCompletionSource tcs = new();
            while (!ProcessingChunks.TryAdd(chunk.ChunkName, tcs.Task))
            {
                if (ProcessingChunks.TryGetValue(chunk.ChunkName, out Task? task))
                {
                    await task.ConfigureAwait(false);
                    token.ThrowIfCancellationRequested();
                }
            }

            try
            {
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
                            context.Progress.Report(new GamePackageOperationReport.Install(bytesRead, 0));
                            offset += bytesRead;
                        }
                        while (true);
                    }
                }
            }
            finally
            {
                tcs.TrySetResult();
                ProcessingChunks.TryRemove(chunk.ChunkName, out _);
                if (!DuplicatingChunkNames.Contains(chunk.ChunkName))
                {
                    FileOperation.Delete(chunkPath);
                }
            }

            context.Progress.Report(new GamePackageOperationReport.Install(0, 1));
        }
    }
}