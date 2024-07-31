// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf.Collections;
using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Factory.IO;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace Snap.Hutao.Service.Game.Package.Advanced;

[ConstructorGenerated]
internal abstract partial class GameAssetsOperationService : IGameAssetsOperationService
{
    private readonly IMemoryStreamFactory memoryStreamFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly JsonSerializerOptions jsonOptions;

    private readonly ConcurrentDictionary<string, Task> downloadTasks = [];

    public abstract ValueTask InstallAssetsAsync(SophonDecodedBuild remoteBuild, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions options);

    public abstract ValueTask UpdateDiffAssetsAsync(List<SophonAssetOperation> diffAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions);

    public abstract ValueTask PredownloadDiffAssetsAsync(List<SophonAssetOperation> diffAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions);

    public async ValueTask<GamePackageIntegrityInfo> VerifyGamePackageIntegrityAsync(SophonDecodedBuild build, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, CancellationToken token = default)
    {
        List<SophonAssetOperation> conflictedAssets = [];
        bool channelSdkConflict = false;

        await VerifyManifestsAsync(build, conflictedAssets, context, progress, parallelOptions).ConfigureAwait(false);

        if (context.GameChannelSDK is not null)
        {
            try
            {
                using (FileStream sdkManifestStream = File.OpenRead(Path.Combine(context.GameFileSystem.GameDirectory, context.GameChannelSDK.PackageVersionFileName)))
                {
                    using (StreamReader reader = new(sdkManifestStream))
                    {
                        while (await reader.ReadLineAsync(token).ConfigureAwait(false) is { Length: > 0 } row)
                        {
                            VersionItem? item = JsonSerializer.Deserialize<VersionItem>(row, jsonOptions);
                            ArgumentNullException.ThrowIfNull(item);

                            string path = Path.Combine(context.GameFileSystem.GameDirectory, item.RelativePath);
                            if (!item.Md5.Equals(await MD5.HashFileAsync(path, token).ConfigureAwait(false), StringComparison.OrdinalIgnoreCase))
                            {
                                channelSdkConflict = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (JsonException)
            {
                channelSdkConflict = true;
            }
        }

        return new()
        {
            ConflictedAssets = conflictedAssets,
            ChannelSdkConflicted = channelSdkConflict,
        };
    }

    public async ValueTask RepairGamePackageAsync(GamePackageIntegrityInfo info, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, CancellationToken token = default)
    {
        await RepairAssetsAsync(info, context, progress, parallelOptions).ConfigureAwait(false);

        if (info.ChannelSdkConflicted)
        {
            ArgumentNullException.ThrowIfNull(context.GameChannelSDK);
            await ExtractChannelSdkAsync(context, token).ConfigureAwait(false);

            progress.Report(new GamePackageOperationReport.Update(context.GameChannelSDK.ChannelSdkPackage.Size, 1));
        }
    }

    public async ValueTask ExtractChannelSdkAsync(GamePackageOperationContext context, CancellationToken token = default)
    {
        if (context.GameChannelSDK is null)
        {
            return;
        }

        using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(GameAssetsOperationService)))
        {
            using (Stream sdkStream = await httpClient.GetStreamAsync(context.GameChannelSDK.ChannelSdkPackage.Url, token).ConfigureAwait(false))
            {
                ZipFile.ExtractToDirectory(sdkStream, context.GameFileSystem.GameDirectory, true);
            }
        }
    }

    protected static async ValueTask VerifyAssetAsync(SophonAsset asset, List<SophonAssetOperation> conflictedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, CancellationToken token = default)
    {
        string assetPath = Path.Combine(context.GameFileSystem.GameDirectory, asset.AssetProperty.AssetName);

        if (asset.AssetProperty.AssetType is 64)
        {
            Directory.CreateDirectory(assetPath);
            return;
        }

        RepeatedField<AssetChunk> chunks = asset.AssetProperty.AssetChunks;

        if (!File.Exists(assetPath))
        {
            conflictedAssets.Add(SophonAssetOperation.AddOrRepair(asset.UrlPrefix, asset.AssetProperty));
            progress.Report(new GamePackageOperationReport.Update(0, chunks.Count));

            return;
        }

        using (SafeFileHandle fileHandle = File.OpenHandle(assetPath))
        {
            for (int i = 0; i < chunks.Count; i++)
            {
                AssetChunk chunk = chunks[i];
                using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent((int)chunk.ChunkSizeDecompressed))
                {
                    Memory<byte> buffer = memoryOwner.Memory[..(int)chunk.ChunkSizeDecompressed];
                    await RandomAccessRead.ExactlyAsync(fileHandle, buffer, chunk.ChunkOnFileOffset, token).ConfigureAwait(false);
                    if (!chunk.ChunkDecompressedHashMd5.Equals(MD5.Hash(buffer.Span), StringComparison.OrdinalIgnoreCase))
                    {
                        conflictedAssets.Add(SophonAssetOperation.AddOrRepair(asset.UrlPrefix, asset.AssetProperty));
                        progress.Report(new GamePackageOperationReport.Update(0, chunks.Count - i));
                        return;
                    }
                }

                progress.Report(new GamePackageOperationReport.Update(chunk.ChunkSizeDecompressed, 1));
            }
        }
    }

    protected static async ValueTask DeleteAssetsAsync(IEnumerable<AssetProperty> deleteAssets, GamePackageOperationContext context)
    {
        foreach (AssetProperty asset in deleteAssets)
        {
            string assetPath = Path.Combine(context.GameFileSystem.GameDirectory, asset.AssetName);

            if (asset.AssetType is 64)
            {
                Directory.Delete(assetPath, true);
            }

            if (File.Exists(assetPath))
            {
                File.Delete(assetPath);
            }
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    protected abstract ValueTask VerifyManifestsAsync(SophonDecodedBuild build, List<SophonAssetOperation> conflictedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions);

    protected abstract ValueTask VerifyManifestAsync(SophonDecodedManifest manifest, List<SophonAssetOperation> conflictedAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions);

    protected abstract ValueTask RepairAssetsAsync(GamePackageIntegrityInfo info, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions);

    protected abstract ValueTask DownloadChunksAsync(IEnumerable<SophonChunk> sophonChunks, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions);

    protected abstract ValueTask MergeNewAssetAsync(AssetProperty assetProperty, GamePackageOperationContext context, ParallelOptions parallelOptions);

    protected async ValueTask DownloadAndMergeAssetAsync(SophonAssetOperation sophonAsset, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        if (sophonAsset.NewAsset.AssetType is 64)
        {
            Directory.CreateDirectory(Path.Combine(context.GameFileSystem.GameDirectory, sophonAsset.NewAsset.AssetName));
            return;
        }

        IEnumerable<SophonChunk> sophonChunks = sophonAsset.Type switch
        {
            SophonAssetOperationType.AddOrRepair => sophonAsset.NewAsset.AssetChunks.Select(chunk => new SophonChunk(sophonAsset.UrlPrefix, chunk)),
            SophonAssetOperationType.Modify => sophonAsset.DiffChunks,
            _ => [],
        };

        await DownloadChunksAsync(sophonChunks, context, progress, parallelOptions).ConfigureAwait(false);
        await MergeAssetAsync(sophonAsset, context, parallelOptions).ConfigureAwait(false);
    }

    protected async ValueTask DownloadChunkAsync(SophonChunk sophonChunk, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, CancellationToken token = default)
    {
        Directory.CreateDirectory(context.GameFileSystem.ChunksDirectory);
        string chunkPath = Path.Combine(context.GameFileSystem.ChunksDirectory, sophonChunk.AssetChunk.ChunkName);

        TaskCompletionSource downloadTcs = new();
        if (downloadTasks.TryAdd(sophonChunk.AssetChunk.ChunkName, downloadTcs.Task))
        {
            try
            {
                if (File.Exists(chunkPath))
                {
                    string chunkXxh64 = await XXH64.HashFileAsync(chunkPath, token).ConfigureAwait(false);
                    if (chunkXxh64.Equals(sophonChunk.AssetChunk.ChunkName.Split("_")[0], StringComparison.OrdinalIgnoreCase))
                    {
                        progress.Report(new GamePackageOperationReport.Update(sophonChunk.AssetChunk.ChunkSize, 1));
                        return;
                    }

                    File.Delete(chunkPath);
                }

                using (FileStream fileStream = File.Create(chunkPath))
                {
                    fileStream.Position = 0;

                    using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(GameAssetsOperationService)))
                    {
                        using (Stream webStream = await httpClient.GetStreamAsync(sophonChunk.ChunkDownloadUrl, token).ConfigureAwait(false))
                        {
                            StreamCopyWorker<GamePackageOperationReport> worker = new(webStream, fileStream, (bytesRead, _) => new GamePackageOperationReport.Update(bytesRead, 0));

                            await worker.CopyAsync(progress).ConfigureAwait(false);

                            fileStream.Position = 0;
                            string chunkXxh64 = await XXH64.HashAsync(fileStream, token).ConfigureAwait(false);
                            if (chunkXxh64.Equals(sophonChunk.AssetChunk.ChunkName.Split("_")[0], StringComparison.OrdinalIgnoreCase))
                            {
                                progress.Report(new GamePackageOperationReport.Update(0, 1));
                            }
                        }
                    }
                }
            }
            finally
            {
                downloadTcs.TrySetResult();
                downloadTasks.TryRemove(sophonChunk.AssetChunk.ChunkName, out _);
            }
        }
        else if (downloadTasks.TryGetValue(chunkPath, out Task? task))
        {
            await task.ConfigureAwait(false);
        }
    }

    protected async ValueTask MergeAssetAsync(SophonAssetOperation diffAsset, GamePackageOperationContext context, ParallelOptions parallelOptions)
    {
        ValueTask task = diffAsset.Type switch
        {
            SophonAssetOperationType.AddOrRepair => MergeNewAssetAsync(diffAsset.NewAsset, context, parallelOptions),
            SophonAssetOperationType.Modify => MergeDiffAssetAsync(diffAsset, context, parallelOptions.CancellationToken),
            _ => ValueTask.CompletedTask,
        };

        await task.ConfigureAwait(false);
    }

    protected async ValueTask MergeDiffAssetAsync(SophonAssetOperation modifiedAsset, GamePackageOperationContext context, CancellationToken token = default)
    {
        using (MemoryStream newAssetStream = memoryStreamFactory.GetStream())
        {
            using (SafeFileHandle oldAssetHandle = File.OpenHandle(Path.Combine(context.GameFileSystem.GameDirectory, modifiedAsset.OldAsset.AssetName), FileMode.Open, FileAccess.Read, FileShare.None))
            {
                foreach (AssetChunk chunk in modifiedAsset.NewAsset.AssetChunks)
                {
                    newAssetStream.Position = chunk.ChunkOnFileOffset;

                    AssetChunk? oldChunk = modifiedAsset.OldAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5 == chunk.ChunkDecompressedHashMd5);
                    if (oldChunk is null)
                    {
                        using (FileStream diffStream = File.OpenRead(Path.Combine(context.GameFileSystem.ChunksDirectory, chunk.ChunkName)))
                        {
                            using (ZstandardDecompressionStream decompressionStream = new(diffStream))
                            {
                                await decompressionStream.CopyToAsync(newAssetStream, token).ConfigureAwait(false);
                                continue;
                            }
                        }
                    }

                    using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(81920))
                    {
                        Memory<byte> buffer = memoryOwner.Memory;
                        long offset = oldChunk.ChunkOnFileOffset;
                        long bytesToCopy = oldChunk.ChunkSizeDecompressed;
                        while (bytesToCopy > 0)
                        {
                            int bytesRead = await RandomAccess.ReadAsync(oldAssetHandle, buffer[..(int)Math.Min(buffer.Length, bytesToCopy)], offset, token).ConfigureAwait(false);
                            if (bytesRead <= 0)
                            {
                                break;
                            }

                            await newAssetStream.WriteAsync(buffer[..bytesRead], token).ConfigureAwait(false);
                            offset += bytesRead;
                            bytesToCopy -= bytesRead;
                        }
                    }
                }

                using (FileStream newAssetFileStream = File.Create(Path.Combine(context.GameFileSystem.GameDirectory, modifiedAsset.NewAsset.AssetName)))
                {
                    newAssetStream.Position = 0;
                    await newAssetStream.CopyToAsync(newAssetFileStream, token).ConfigureAwait(false);
                }
            }
        }
    }
}