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

    private readonly ConcurrentDictionary<string, Task> downloadingChunks = [];

    public abstract ValueTask InstallAssetsAsync(SophonDecodedBuild remoteBuild, GamePackageServiceContext context);

    public abstract ValueTask UpdateDiffAssetsAsync(List<SophonAssetOperation> diffAssets, GamePackageServiceContext context);

    public abstract ValueTask PredownloadDiffAssetsAsync(List<SophonAssetOperation> diffAssets, GamePackageServiceContext context);

    public async ValueTask<GamePackageIntegrityInfo> VerifyGamePackageIntegrityAsync(SophonDecodedBuild build, GamePackageServiceContext context)
    {
        CancellationToken token = context.ParallelOptions.CancellationToken;

        List<SophonAssetOperation> conflictedAssets = [];
        bool channelSdkConflicted = false;

        await VerifyManifestsAsync(build, conflictedAssets, context).ConfigureAwait(false);

        if (context.Operation.GameChannelSDK is not null)
        {
            try
            {
                using (StreamReader reader = File.OpenText(Path.Combine(context.Operation.GameFileSystem.GameDirectory, context.Operation.GameChannelSDK.PackageVersionFileName)))
                {
                    while (await reader.ReadLineAsync(token).ConfigureAwait(false) is { Length: > 0 } row)
                    {
                        VersionItem? item = JsonSerializer.Deserialize<VersionItem>(row, jsonOptions);
                        ArgumentNullException.ThrowIfNull(item);

                        string path = Path.Combine(context.Operation.GameFileSystem.GameDirectory, item.RelativePath);
                        if (!item.Md5.Equals(await MD5.HashFileAsync(path, token).ConfigureAwait(false), StringComparison.OrdinalIgnoreCase))
                        {
                            channelSdkConflicted = true;
                            break;
                        }
                    }
                }
            }
            catch (JsonException)
            {
                channelSdkConflicted = true;
            }
        }

        return new()
        {
            ConflictedAssets = conflictedAssets,
            ChannelSdkConflicted = channelSdkConflicted,
        };
    }

    public async ValueTask RepairGamePackageAsync(GamePackageIntegrityInfo info, GamePackageServiceContext context)
    {
        await RepairAssetsAsync(info, context).ConfigureAwait(false);

        if (info.ChannelSdkConflicted)
        {
            ArgumentNullException.ThrowIfNull(context.Operation.GameChannelSDK);
            await EnsureChannelSdkAsync(context).ConfigureAwait(false);

            context.Progress.Report(new GamePackageOperationReport.Update(context.Operation.GameChannelSDK.ChannelSdkPackage.Size, 1));
        }
    }

    public async ValueTask EnsureChannelSdkAsync(GamePackageServiceContext context)
    {
        if (context.Operation.GameChannelSDK is null)
        {
            return;
        }

        using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(GameAssetsOperationService)))
        {
            using (Stream sdkStream = await httpClient.GetStreamAsync(context.Operation.GameChannelSDK.ChannelSdkPackage.Url, context.ParallelOptions.CancellationToken).ConfigureAwait(false))
            {
                ZipFile.ExtractToDirectory(sdkStream, context.Operation.GameFileSystem.GameDirectory, true);
            }
        }
    }

    protected static async ValueTask VerifyAssetAsync(SophonAsset asset, List<SophonAssetOperation> conflictedAssets, GamePackageServiceContext context)
    {
        string assetPath = Path.Combine(context.Operation.GameFileSystem.GameDirectory, asset.AssetProperty.AssetName);

        if (asset.AssetProperty.AssetType is 64)
        {
            Directory.CreateDirectory(assetPath);
            return;
        }

        RepeatedField<AssetChunk> chunks = asset.AssetProperty.AssetChunks;

        if (!File.Exists(assetPath))
        {
            conflictedAssets.Add(SophonAssetOperation.AddOrRepair(asset.UrlPrefix, asset.AssetProperty));
            context.Progress.Report(new GamePackageOperationReport.Update(0, chunks.Count));

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
                    await RandomAccessRead.ExactlyAsync(fileHandle, buffer, chunk.ChunkOnFileOffset, context.ParallelOptions.CancellationToken).ConfigureAwait(false);
                    if (!chunk.ChunkDecompressedHashMd5.Equals(MD5.Hash(buffer.Span), StringComparison.OrdinalIgnoreCase))
                    {
                        conflictedAssets.Add(SophonAssetOperation.AddOrRepair(asset.UrlPrefix, asset.AssetProperty));
                        context.Progress.Report(new GamePackageOperationReport.Update(0, chunks.Count - i));
                        return;
                    }
                }

                context.Progress.Report(new GamePackageOperationReport.Update(chunk.ChunkSizeDecompressed, 1));
            }
        }
    }

    protected static async ValueTask DeleteAssetsAsync(IEnumerable<AssetProperty> deleteAssets, GamePackageServiceContext context)
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        foreach (AssetProperty asset in deleteAssets)
        {
            string assetPath = Path.Combine(context.Operation.GameFileSystem.GameDirectory, asset.AssetName);

            if (asset.AssetType is 64)
            {
                Directory.Delete(assetPath, true);
            }

            if (File.Exists(assetPath))
            {
                File.Delete(assetPath);
            }
        }
    }

    protected abstract ValueTask VerifyManifestsAsync(SophonDecodedBuild build, List<SophonAssetOperation> conflictedAssets, GamePackageServiceContext context);

    protected abstract ValueTask VerifyManifestAsync(SophonDecodedManifest manifest, List<SophonAssetOperation> conflictedAssets, GamePackageServiceContext context);

    protected abstract ValueTask RepairAssetsAsync(GamePackageIntegrityInfo info, GamePackageServiceContext context);

    protected abstract ValueTask DownloadChunksAsync(IEnumerable<SophonChunk> sophonChunks, GamePackageServiceContext context);

    protected abstract ValueTask MergeNewAssetAsync(AssetProperty assetProperty, GamePackageServiceContext context);

    protected async ValueTask DownloadAndMergeAssetAsync(SophonAssetOperation sophonAsset, GamePackageServiceContext context)
    {
        if (sophonAsset.NewAsset.AssetType is 64)
        {
            Directory.CreateDirectory(Path.Combine(context.Operation.GameFileSystem.GameDirectory, sophonAsset.NewAsset.AssetName));
            return;
        }

        IEnumerable<SophonChunk> sophonChunks = sophonAsset.Type switch
        {
            SophonAssetOperationType.AddOrRepair => sophonAsset.NewAsset.AssetChunks.Select(chunk => new SophonChunk(sophonAsset.UrlPrefix, chunk)),
            SophonAssetOperationType.Modify => sophonAsset.DiffChunks,
            _ => [],
        };

        await DownloadChunksAsync(sophonChunks, context).ConfigureAwait(false);
        await MergeAssetAsync(sophonAsset, context).ConfigureAwait(false);
    }

    protected async ValueTask DownloadChunkAsync(SophonChunk sophonChunk, GamePackageServiceContext context)
    {
        CancellationToken token = context.ParallelOptions.CancellationToken;

        Directory.CreateDirectory(context.Operation.GameFileSystem.ChunksDirectory);
        string chunkPath = Path.Combine(context.Operation.GameFileSystem.ChunksDirectory, sophonChunk.AssetChunk.ChunkName);

        TaskCompletionSource downloadTcs = new();
        if (downloadingChunks.TryAdd(sophonChunk.AssetChunk.ChunkName, downloadTcs.Task))
        {
            try
            {
                if (File.Exists(chunkPath))
                {
                    string chunkXxh64 = await XXH64.HashFileAsync(chunkPath, token).ConfigureAwait(false);
                    if (chunkXxh64.Equals(sophonChunk.AssetChunk.ChunkName.Split("_")[0], StringComparison.OrdinalIgnoreCase))
                    {
                        context.Progress.Report(new GamePackageOperationReport.Update(sophonChunk.AssetChunk.ChunkSize, 1));
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

                            await worker.CopyAsync(context.Progress, token).ConfigureAwait(false);

                            fileStream.Position = 0;
                            string chunkXxh64 = await XXH64.HashAsync(fileStream, token).ConfigureAwait(false);
                            if (chunkXxh64.Equals(sophonChunk.AssetChunk.ChunkName.Split("_")[0], StringComparison.OrdinalIgnoreCase))
                            {
                                context.Progress.Report(new GamePackageOperationReport.Update(0, 1));
                            }
                        }
                    }
                }
            }
            finally
            {
                downloadTcs.TrySetResult();
                downloadingChunks.TryRemove(sophonChunk.AssetChunk.ChunkName, out _);
            }
        }
        else if (downloadingChunks.TryGetValue(chunkPath, out Task? task))
        {
            await task.ConfigureAwait(false);
        }
    }

    protected async ValueTask MergeAssetAsync(SophonAssetOperation diffAsset, GamePackageServiceContext context)
    {
        ValueTask task = diffAsset.Type switch
        {
            SophonAssetOperationType.AddOrRepair => MergeNewAssetAsync(diffAsset.NewAsset, context),
            SophonAssetOperationType.Modify => MergeDiffAssetAsync(diffAsset, context),
            _ => ValueTask.CompletedTask,
        };

        await task.ConfigureAwait(false);
    }

    protected async ValueTask MergeDiffAssetAsync(SophonAssetOperation modifiedAsset, GamePackageServiceContext context)
    {
        CancellationToken token = context.ParallelOptions.CancellationToken;

        using (MemoryStream newAssetStream = memoryStreamFactory.GetStream())
        {
            using (SafeFileHandle oldAssetHandle = File.OpenHandle(Path.Combine(context.Operation.GameFileSystem.GameDirectory, modifiedAsset.OldAsset.AssetName), FileMode.Open, FileAccess.Read, FileShare.None))
            {
                foreach (AssetChunk chunk in modifiedAsset.NewAsset.AssetChunks)
                {
                    newAssetStream.Position = chunk.ChunkOnFileOffset;

                    AssetChunk? oldChunk = modifiedAsset.OldAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5 == chunk.ChunkDecompressedHashMd5);
                    if (oldChunk is null)
                    {
                        string chunkPath = Path.Combine(context.Operation.GameFileSystem.ChunksDirectory, chunk.ChunkName);
                        if (!File.Exists(chunkPath))
                        {
                            continue;
                        }

                        using (FileStream diffStream = File.OpenRead(chunkPath))
                        {
                            using (ZstandardDecompressionStream decompressionStream = new(diffStream))
                            {
                                await decompressionStream.CopyToAsync(newAssetStream, token).ConfigureAwait(false);
                            }
                        }

                        continue;
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

                using (FileStream newAssetFileStream = File.Create(Path.Combine(context.Operation.GameFileSystem.GameDirectory, modifiedAsset.NewAsset.AssetName)))
                {
                    newAssetStream.Position = 0;
                    await newAssetStream.CopyToAsync(newAssetFileStream, token).ConfigureAwait(false);
                }
            }
        }
    }
}