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
internal abstract partial class GameAssetOperation : IGameAssetOperation
{
    private readonly IMemoryStreamFactory memoryStreamFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly JsonSerializerOptions jsonOptions;

    private readonly ConcurrentDictionary<string, Task> downloadingChunks = [];

    public abstract ValueTask InstallAssetsAsync(GamePackageServiceContext context, SophonDecodedBuild remoteBuild);

    public abstract ValueTask UpdateDiffAssetsAsync(GamePackageServiceContext context, List<SophonAssetOperation> diffAssets);

    public abstract ValueTask PredownloadDiffAssetsAsync(GamePackageServiceContext context, List<SophonAssetOperation> diffAssets);

    public async ValueTask<GamePackageIntegrityInfo> VerifyGamePackageIntegrityAsync(GamePackageServiceContext context, SophonDecodedBuild build)
    {
        CancellationToken token = context.CancellationToken;

        List<SophonAssetOperation> conflictedAssets = [];
        bool channelSdkConflicted = false;

        await VerifyManifestsAsync(context, build, conflictedAssets.Add).ConfigureAwait(false);

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

    public async ValueTask RepairGamePackageAsync(GamePackageServiceContext context, GamePackageIntegrityInfo info)
    {
        await RepairAssetsAsync(context, info).ConfigureAwait(false);

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

        using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(GameAssetOperation)))
        {
            using (Stream sdkStream = await httpClient.GetStreamAsync(context.Operation.GameChannelSDK.ChannelSdkPackage.Url, context.ParallelOptions.CancellationToken).ConfigureAwait(false))
            {
                ZipFile.ExtractToDirectory(sdkStream, context.Operation.GameFileSystem.GameDirectory, true);
            }
        }
    }

    protected static async ValueTask VerifyAssetAsync(GamePackageServiceContext context, SophonAsset asset, Action<SophonAssetOperation> conflictHandler)
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
            conflictHandler(SophonAssetOperation.AddOrRepair(asset.UrlPrefix, asset.AssetProperty));
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
                    await RandomAccessRead.ExactlyAsync(fileHandle, buffer, chunk.ChunkOnFileOffset, context.CancellationToken).ConfigureAwait(false);
                    if (!chunk.ChunkDecompressedHashMd5.Equals(MD5.Hash(buffer.Span), StringComparison.OrdinalIgnoreCase))
                    {
                        conflictHandler(SophonAssetOperation.AddOrRepair(asset.UrlPrefix, asset.AssetProperty));
                        context.Progress.Report(new GamePackageOperationReport.Update(0, chunks.Count - i));
                        return;
                    }
                }

                context.Progress.Report(new GamePackageOperationReport.Update(chunk.ChunkSizeDecompressed, 1));
            }
        }
    }

    protected static async ValueTask DeleteAssetsAsync(GamePackageServiceContext context, IEnumerable<AssetProperty> assets)
    {
        await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

        foreach (AssetProperty asset in assets)
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

    protected abstract ValueTask VerifyManifestsAsync(GamePackageServiceContext context, SophonDecodedBuild build, Action<SophonAssetOperation> conflictHandler);

    protected abstract ValueTask VerifyManifestAsync(GamePackageServiceContext context, SophonDecodedManifest manifest, Action<SophonAssetOperation> conflictHandler);

    protected abstract ValueTask RepairAssetsAsync(GamePackageServiceContext context, GamePackageIntegrityInfo info);

    protected abstract ValueTask DownloadChunksAsync(GamePackageServiceContext context, IEnumerable<SophonChunk> sophonChunks);

    protected abstract ValueTask MergeNewAssetAsync(GamePackageServiceContext context, AssetProperty assetProperty);

    protected async ValueTask EnsureAssetAsync(GamePackageServiceContext context, SophonAssetOperation asset)
    {
        if (asset.NewAsset.AssetType is 64)
        {
            Directory.CreateDirectory(Path.Combine(context.Operation.GameFileSystem.GameDirectory, asset.NewAsset.AssetName));
            return;
        }

        IEnumerable<SophonChunk> chunks = asset.Kind switch
        {
            SophonAssetOperationKind.AddOrRepair => asset.NewAsset.AssetChunks.Select(chunk => new SophonChunk(asset.UrlPrefix, chunk)),
            SophonAssetOperationKind.Modify => asset.DiffChunks,
            _ => [],
        };

        await DownloadChunksAsync(context, chunks).ConfigureAwait(false);
        await MergeAssetAsync(context, asset).ConfigureAwait(false);
    }

    protected async ValueTask DownloadChunkAsync(GamePackageServiceContext context, SophonChunk sophonChunk)
    {
        CancellationToken token = context.CancellationToken;

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

                    using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(GameAssetOperation)))
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

    protected async ValueTask MergeAssetAsync(GamePackageServiceContext context, SophonAssetOperation asset)
    {
        ValueTask task = asset.Kind switch
        {
            SophonAssetOperationKind.AddOrRepair => MergeNewAssetAsync(context, asset.NewAsset),
            SophonAssetOperationKind.Modify => MergeDiffAssetAsync(context, asset),
            _ => ValueTask.CompletedTask,
        };

        await task.ConfigureAwait(false);
    }

    protected async ValueTask MergeDiffAssetAsync(GamePackageServiceContext context, SophonAssetOperation asset)
    {
        CancellationToken token = context.CancellationToken;

        using (MemoryStream newAssetStream = memoryStreamFactory.GetStream())
        {
            string oldAssetPath = Path.Combine(context.Operation.GameFileSystem.GameDirectory, asset.OldAsset.AssetName);
            using (SafeFileHandle oldAssetHandle = File.OpenHandle(oldAssetPath, options: FileOptions.RandomAccess))
            {
                foreach (AssetChunk chunk in asset.NewAsset.AssetChunks)
                {
                    newAssetStream.Position = chunk.ChunkOnFileOffset;

                    if (asset.OldAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5 == chunk.ChunkDecompressedHashMd5) is not { } oldChunk)
                    {
                        string chunkPath = Path.Combine(context.Operation.GameFileSystem.ChunksDirectory, chunk.ChunkName);
                        if (!File.Exists(chunkPath))
                        {
                            // File not found, skip this asset and repair later
                            return;
                        }

                        using (FileStream diffStream = File.OpenRead(chunkPath))
                        {
                            using (ZstandardDecompressionStream decompressor = new(diffStream))
                            {
                                await decompressor.CopyToAsync(newAssetStream, token).ConfigureAwait(false);
                            }
                        }
                    }
                    else
                    {
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
                }
            }

            string newAssetPath = Path.Combine(context.Operation.GameFileSystem.GameDirectory, asset.NewAsset.AssetName);
            using (FileStream newAssetFileStream = File.Create(newAssetPath))
            {
                newAssetStream.Position = 0;
                await newAssetStream.CopyToAsync(newAssetFileStream, token).ConfigureAwait(false);
            }
        }
    }
}