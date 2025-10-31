// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Service.Game.Package.Advanced.Model;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Buffers;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.Service.Game.Package.Advanced.AssetOperation;

[SuppressMessage("", "SA1202")]
[Service(ServiceLifetime.Transient)]
internal sealed partial class GameAssetOperationSSD : GameAssetOperation
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial GameAssetOperationSSD(IServiceProvider serviceProvider);

    #region Chunk

    public override async ValueTask InstallAssetsAsync(GamePackageServiceContext context, SophonDecodedBuild remoteBuild)
    {
        await Parallel.ForEachAsync(remoteBuild.Manifests, context.ParallelOptions, async (manifest, token) =>
        {
            token.ThrowIfCancellationRequested();
            IEnumerable<SophonAssetOperation> assets = manifest.Data.Assets.Select(asset => SophonAssetOperation.AddOrRepair(manifest.UrlPrefix, manifest.UrlSuffix, asset));
            await Parallel.ForEachAsync(assets, context.ParallelOptions, (asset, token) => EnsureAssetAsync(context, asset)).ConfigureAwait(true);
        }).ConfigureAwait(false);
    }

    public override async ValueTask UpdateDiffAssetsAsync(GamePackageServiceContext context, ImmutableArray<SophonAssetOperation> diffAssets)
    {
        await Parallel.ForEachAsync(diffAssets, context.ParallelOptions, (asset, token) => asset.Kind switch
        {
            SophonAssetOperationKind.AddOrRepair or SophonAssetOperationKind.Modify => EnsureAssetAsync(context, asset),
            SophonAssetOperationKind.Delete => DeleteAssetAsync(context, asset.OldAsset),
            _ => ValueTask.CompletedTask,
        }).ConfigureAwait(false);
    }

    public override async ValueTask PredownloadDiffAssetsAsync(GamePackageServiceContext context, ImmutableArray<SophonAssetOperation> diffAssets)
    {
        await Parallel.ForEachAsync(diffAssets, context.ParallelOptions, (asset, token) =>
        {
            token.ThrowIfCancellationRequested();
            IReadOnlyList<SophonChunk> chunks = asset.Kind switch
            {
                SophonAssetOperationKind.AddOrRepair => [.. asset.NewAsset.AssetChunks.Select(c => new SophonChunk(asset.UrlPrefix, asset.UrlSuffix, c))],
                SophonAssetOperationKind.Modify => asset.DiffChunks,
                _ => [],
            };

            return DownloadChunksAsync(context, chunks);
        }).ConfigureAwait(false);
    }

    protected override async ValueTask VerifyManifestsAsync(GamePackageServiceContext context, SophonDecodedBuild build, Action<SophonAssetOperation> conflictHandler)
    {
        await Parallel.ForEachAsync(build.Manifests, context.ParallelOptions, (manifest, token) => VerifyManifestAsync(context, manifest, conflictHandler)).ConfigureAwait(false);
    }

    protected override async ValueTask VerifyManifestAsync(GamePackageServiceContext context, SophonDecodedManifest manifest, Action<SophonAssetOperation> conflictHandler)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        await Parallel.ForEachAsync(manifest.Data.Assets, context.ParallelOptions, (asset, token) => VerifyAssetAsync(context, new(manifest.UrlPrefix, manifest.UrlSuffix, asset), conflictHandler)).ConfigureAwait(false);
    }

    protected override async ValueTask RepairAssetsAsync(GamePackageServiceContext context, GamePackageIntegrityInfo info)
    {
        await Parallel.ForEachAsync(info.ConflictedAssets, context.ParallelOptions, (asset, token) => EnsureAssetAsync(context, asset)).ConfigureAwait(false);
    }

    protected override async ValueTask DownloadChunksAsync(GamePackageServiceContext context, IReadOnlyList<SophonChunk> sophonChunks)
    {
        await Parallel.ForEachAsync(sophonChunks, context.ParallelOptions, (chunk, token) => DownloadChunkAsync(context, chunk)).ConfigureAwait(false);
    }

    protected override async ValueTask MergeNewAssetAsync(GamePackageServiceContext context, AssetProperty assetProperty)
    {
        string path = context.EnsureAssetTargetDirectoryExists(assetProperty.AssetName);
        using (SafeFileHandle fileHandle = File.OpenHandle(path, FileMode.Create, FileAccess.Write, FileShare.None, preallocationSize: assetProperty.AssetSize))
        {
            // ReSharper disable once AccessToDisposedClosure
            await Parallel.ForEachAsync(assetProperty.AssetChunks, context.ParallelOptions, (chunk, token) => MergeChunkIntoAssetAsync(context, fileHandle, chunk)).ConfigureAwait(false);
        }
    }

    private static async ValueTask MergeChunkIntoAssetAsync(GamePackageServiceContext context, SafeFileHandle fileHandle, AssetChunk chunk)
    {
        CancellationToken token = context.CancellationToken;
        token.ThrowIfCancellationRequested();
        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(ChunkBufferSize))
        {
            Memory<byte> buffer = memoryOwner.Memory;

            string chunkPath = Path.Combine(context.Operation.EffectiveChunksDirectory, chunk.ChunkName);
            if (!File.Exists(chunkPath))
            {
                return;
            }

            using (await context.ExclusiveProcessChunkAsync(chunk.ChunkName, token).ConfigureAwait(true))
            {
                using (FileStream chunkFile = File.OpenRead(chunkPath))
                {
                    using (ZstandardDecompressStream decompressStream = new(chunkFile))
                    {
                        long offset = chunk.ChunkOnFileOffset;
                        do
                        {
                            int bytesRead = await decompressStream.ReadAsync(buffer, token).ConfigureAwait(true);
                            if (bytesRead <= 0)
                            {
                                break;
                            }

                            await RandomAccess.WriteAsync(fileHandle, buffer[..bytesRead], offset, token).ConfigureAwait(true);
                            context.Progress.Report(new GamePackageOperationReport.Install(bytesRead, 0, chunk.ChunkName));
                            offset += bytesRead;
                        }
                        while (true);
                    }
                }

                if (context.Operation.Kind is GamePackageOperationKind.Install or GamePackageOperationKind.Update && !context.DuplicatedChunkNames.ContainsKey(chunk.ChunkName))
                {
                    FileOperation.Delete(chunkPath);
                }
            }

            context.Progress.Report(new GamePackageOperationReport.Install(0, 1, chunk.ChunkName));
        }
    }

    #endregion

    #region LDiff

    public override async ValueTask InstallOrPatchAssetsAsync(GamePackageServiceContext context, SophonDecodedPatchBuild patchBuild)
    {
        await Parallel.ForEachAsync(patchBuild.Manifests, context.ParallelOptions, async (manifest, token) =>
        {
            token.ThrowIfCancellationRequested();
            IEnumerable<SophonPatchAsset> assets = manifest.Data.FileDatas
                .Where(fd => fd.PatchesEntries.SingleOrDefault(pe => pe.Key == manifest.OriginalTag) is not null)
                .Select(fd => new SophonPatchAsset(manifest.UrlPrefix, manifest.UrlSuffix, fd, fd.PatchesEntries.Single(pe => pe.Key == manifest.OriginalTag).PatchInfo));
            await Parallel.ForEachAsync(assets, context.ParallelOptions, (patchAsset, token) => InstallOrPatchAssetAsync(context, patchAsset)).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public override async ValueTask DeletePatchDeprecatedFilesAsync(GamePackageServiceContext context, SophonDecodedPatchBuild patchBuild)
    {
        await Parallel.ForEachAsync(patchBuild.Manifests, context.ParallelOptions, async (manifest, token) =>
        {
            token.ThrowIfCancellationRequested();
            IEnumerable<string> assetNames = manifest.Data.DeleteFilesEntries.SingleOrDefault(fd => fd.Key == manifest.OriginalTag)?.DeleteFiles.Infos.Select(i => i.Name) ?? [];
            await Parallel.ForEachAsync(assetNames, context.ParallelOptions, (assetName, token) =>
            {
                token.ThrowIfCancellationRequested();
                string assetPath = Path.Combine(context.Operation.EffectiveGameDirectory, assetName);
                if (File.Exists(assetPath))
                {
                    File.Delete(assetPath);
                }

                return ValueTask.CompletedTask;
            }).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    public override async ValueTask PredownloadPatchesAsync(GamePackageServiceContext context, SophonDecodedPatchBuild patchBuild)
    {
        await Parallel.ForEachAsync(patchBuild.Manifests, context.ParallelOptions, async (manifest, token) =>
        {
            token.ThrowIfCancellationRequested();
            IEnumerable<SophonPatchAsset> assets = manifest.Data.FileDatas
                .Where(fd => fd.PatchesEntries.SingleOrDefault(pe => pe.Key == manifest.OriginalTag) is not null)
                .Select(fd => new SophonPatchAsset(manifest.UrlPrefix, manifest.UrlSuffix, fd, fd.PatchesEntries.Single(pe => pe.Key == manifest.OriginalTag).PatchInfo));
            await Parallel.ForEachAsync(assets, context.ParallelOptions, (patchAsset, token) => DownloadPatchAsync(context, patchAsset)).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    #endregion
}