// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Google.Protobuf.Collections;
using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.IO.HPatch;
using Snap.Hutao.Factory.IO;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Package.Advanced.Model;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using System.Buffers;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Snap.Hutao.Service.Game.Package.Advanced.AssetOperation;

[SuppressMessage("", "SA1202")]
internal abstract partial class GameAssetOperation : IGameAssetOperation
{
    protected internal const int ChunkBufferSize = 81920;

    private readonly IMemoryStreamFactory memoryStreamFactory;
    private readonly JsonSerializerOptions jsonOptions;

    [GeneratedConstructor]
    public partial GameAssetOperation(IServiceProvider serviceProvider);

    #region Chunk

    public abstract ValueTask InstallAssetsAsync(GamePackageServiceContext context, SophonDecodedBuild remoteBuild);

    public abstract ValueTask UpdateDiffAssetsAsync(GamePackageServiceContext context, ImmutableArray<SophonAssetOperation> diffAssets);

    public abstract ValueTask PredownloadDiffAssetsAsync(GamePackageServiceContext context, ImmutableArray<SophonAssetOperation> diffAssets);

    public async ValueTask<GamePackageIntegrityInfo> VerifyGamePackageIntegrityAsync(GamePackageServiceContext context, SophonDecodedBuild build)
    {
        CancellationToken token = context.CancellationToken;

        ImmutableArray<SophonAssetOperation>.Builder conflictedAssetsBuilder = ImmutableArray.CreateBuilder<SophonAssetOperation>();
        bool channelSdkConflicted = false;

        await VerifyManifestsAsync(context, build, conflictedAssetsBuilder.Add).ConfigureAwait(false);

        if (context.Operation.GameChannelSDK is { } channelSDK)
        {
            string sdkPackageVersionFilePath = Path.Combine(context.Operation.GameFileSystem.GetGameDirectory(), channelSDK.PackageVersionFileName);
            if (!File.Exists(sdkPackageVersionFilePath))
            {
                channelSdkConflicted = true;
            }
            else
            {
                try
                {
                    using (StreamReader reader = File.OpenText(sdkPackageVersionFilePath))
                    {
                        while (await reader.ReadLineAsync(token).ConfigureAwait(false) is { Length: > 0 } row)
                        {
                            VersionItem? item = JsonSerializer.Deserialize<VersionItem>(row, jsonOptions);
                            ArgumentNullException.ThrowIfNull(item);

                            string path = Path.Combine(context.Operation.GameFileSystem.GetGameDirectory(), item.RelativePath);
                            if (!(File.Exists(path) && item.Md5.Equals(await Hash.FileToHexStringAsync(HashAlgorithmName.MD5, path, token).ConfigureAwait(false), StringComparison.OrdinalIgnoreCase)))
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
        }

        return new()
        {
            ConflictedAssets = conflictedAssetsBuilder.ToImmutable(),
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

            context.Progress.Report(new GamePackageOperationReport.Download(context.Operation.GameChannelSDK.ChannelSdkPackage.Size, 1));
            context.Progress.Report(new GamePackageOperationReport.Install(context.Operation.GameChannelSDK.ChannelSdkPackage.DecompressedSize, 1));
        }
    }

    public async ValueTask EnsureChannelSdkAsync(GamePackageServiceContext context)
    {
        CancellationToken token = context.CancellationToken;

        if (context.Operation.GameChannelSDK is null)
        {
            return;
        }

        using (Stream sdkStream = await context.HttpClient.GetStreamAsync(context.Operation.GameChannelSDK.ChannelSdkPackage.Url, token).ConfigureAwait(false))
        {
            ZipFile.ExtractToDirectory(sdkStream, context.Operation.GameFileSystem.GetGameDirectory(), true);
        }
    }

    protected static async ValueTask VerifyAssetAsync(GamePackageServiceContext context, SophonAsset asset, Action<SophonAssetOperation> conflictHandler)
    {
        CancellationToken token = context.CancellationToken;
        token.ThrowIfCancellationRequested();
        string assetPath = Path.Combine(context.Operation.GameFileSystem.GetGameDirectory(), asset.AssetProperty.AssetName);
        string? assetDirectory = Path.GetDirectoryName(assetPath);
        ArgumentNullException.ThrowIfNull(assetDirectory);
        Directory.CreateDirectory(assetDirectory);

        RepeatedField<AssetChunk> chunks = asset.AssetProperty.AssetChunks;

        if (!File.Exists(assetPath))
        {
            conflictHandler(SophonAssetOperation.AddOrRepair(asset.UrlPrefix, asset.UrlSuffix, asset.AssetProperty));
            context.Progress.Report(new GamePackageOperationReport.Install(0, chunks.Count, asset.AssetProperty.AssetName));

            return;
        }

        SafeFileHandle fileHandle;
        try
        {
            fileHandle = File.OpenHandle(assetPath);
        }
        catch (IOException ex)
        {
            switch (ex.HResult)
            {
                // ERROR_FILE_CORRUPT
                case unchecked((int)0x80070570):
                    context.Progress.Report(new GamePackageOperationReport.Abort(SH.ServiceGamePackageAdvancedAssetOperationDiskCorrupted));
                    break;

                // ERROR_NO_SUCH_DEVICE
                case unchecked((int)0x800701B1):
                    context.Progress.Report(new GamePackageOperationReport.Abort(SH.ServiceGamePackageAdvancedAssetOperationNoSuchDevice));
                    break;
            }

            throw;
        }

        using (fileHandle)
        {
            // Reading same file can't be done in parallel
            for (int i = 0; i < chunks.Count; i++)
            {
                AssetChunk chunk = chunks[i];
                using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.RentExactly((int)chunk.ChunkSizeDecompressed))
                {
                    Memory<byte> buffer = memoryOwner.Memory;
                    bool readFailed = false;
                    try
                    {
                        await RandomAccessRead.ExactlyAsync(fileHandle, buffer, chunk.ChunkOnFileOffset, token).ConfigureAwait(false);
                    }
                    catch (IOException)
                    {
                        readFailed = true;
                    }

                    if (readFailed || !chunk.ChunkDecompressedHashMd5.Equals(Hash.ToHexString(HashAlgorithmName.MD5, buffer.Span), StringComparison.OrdinalIgnoreCase))
                    {
                        conflictHandler(SophonAssetOperation.AddOrRepair(asset.UrlPrefix, asset.UrlSuffix, asset.AssetProperty));
                        context.Progress.Report(new GamePackageOperationReport.Install(0, chunks.Count - i, asset.AssetProperty.AssetName));
                        return;
                    }
                }

                context.Progress.Report(new GamePackageOperationReport.Install(chunk.ChunkSizeDecompressed, 1, chunk.ChunkName));
            }
        }
    }

    protected static ValueTask DeleteAssetAsync(GamePackageServiceContext context, AssetProperty asset)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        string assetPath = Path.Combine(context.Operation.EffectiveGameDirectory, asset.AssetName);

        if (File.Exists(assetPath))
        {
            File.Delete(assetPath);
        }

        return ValueTask.CompletedTask;
    }

    protected static async ValueTask DownloadChunkAsync(GamePackageServiceContext context, SophonChunk sophonChunk)
    {
        CancellationToken token = context.CancellationToken;
        token.ThrowIfCancellationRequested();
        Directory.CreateDirectory(context.Operation.EffectiveChunksDirectory);
        string chunkPath = Path.Combine(context.Operation.EffectiveChunksDirectory, sophonChunk.AssetChunk.ChunkName);

        using (await context.ExclusiveProcessChunkAsync(sophonChunk.AssetChunk.ChunkName, token).ConfigureAwait(false))
        {
            if (File.Exists(chunkPath))
            {
                if (ChunkNameMatches(sophonChunk.AssetChunk.ChunkName, await XxHash64.HashFileAsync(chunkPath, token).ConfigureAwait(false)))
                {
                    context.Progress.Report(new GamePackageOperationReport.Download(sophonChunk.AssetChunk.ChunkSize, 1, sophonChunk.AssetChunk.ChunkName));
                    return;
                }

                File.Delete(chunkPath);
            }

            using (FileStream fileStream = File.Create(chunkPath))
            {
                fileStream.Position = 0;

                using (Stream webStream = await context.HttpClient.GetStreamAsync(sophonChunk.ChunkDownloadUrl, token).ConfigureAwait(false))
                {
                    using (StreamCopyWorker<GamePackageOperationReport> worker = new(webStream, fileStream, (bytesRead, _) => new GamePackageOperationReport.Download(bytesRead, 0, sophonChunk.AssetChunk.ChunkName)))
                    {
                        await worker.CopyAsync(context.StreamCopyRateLimiter, context.Progress, token).ConfigureAwait(false);

                        fileStream.Position = 0;
                        if (ChunkNameMatches(sophonChunk.AssetChunk.ChunkName, await XxHash64.HashAsync(fileStream, token).ConfigureAwait(false)))
                        {
                            context.Progress.Report(new GamePackageOperationReport.Download(0, 1, sophonChunk.AssetChunk.ChunkName));
                        }

                        // No need to delete the file if the hash doesn't match, it will be checked when verifying the asset
                    }
                }
            }
        }
    }

    protected abstract ValueTask VerifyManifestsAsync(GamePackageServiceContext context, SophonDecodedBuild build, Action<SophonAssetOperation> conflictHandler);

    protected abstract ValueTask VerifyManifestAsync(GamePackageServiceContext context, SophonDecodedManifest manifest, Action<SophonAssetOperation> conflictHandler);

    protected abstract ValueTask RepairAssetsAsync(GamePackageServiceContext context, GamePackageIntegrityInfo info);

    protected abstract ValueTask DownloadChunksAsync(GamePackageServiceContext context, IReadOnlyList<SophonChunk> sophonChunks);

    protected abstract ValueTask MergeNewAssetAsync(GamePackageServiceContext context, AssetProperty assetProperty);

    protected async ValueTask EnsureAssetAsync(GamePackageServiceContext context, SophonAssetOperation asset)
    {
        CancellationToken token = context.CancellationToken;
        token.ThrowIfCancellationRequested();
        string assetPath = Path.Combine(context.Operation.GameFileSystem.GetGameDirectory(), asset.NewAsset.AssetName);
        string? assetDirectory = Path.GetDirectoryName(assetPath);
        ArgumentNullException.ThrowIfNull(assetDirectory);
        Directory.CreateDirectory(assetDirectory);

        IReadOnlyList<SophonChunk> chunks = asset.Kind switch
        {
            SophonAssetOperationKind.AddOrRepair => [.. asset.NewAsset.AssetChunks.Select(chunk => new SophonChunk(asset.UrlPrefix, asset.UrlSuffix, chunk))],
            SophonAssetOperationKind.Modify => asset.DiffChunks,
            _ => [],
        };

        if (File.Exists(assetPath) && asset.NewAsset.AssetHashMd5.Equals(await Hash.FileToHexStringAsync(HashAlgorithmName.MD5, assetPath, token).ConfigureAwait(false), StringComparison.OrdinalIgnoreCase))
        {
            context.Progress.Report(new GamePackageOperationReport.Download(0, chunks.Count));
            context.Progress.Report(new GamePackageOperationReport.Install(0, chunks.Count));
            return;
        }

        await DownloadChunksAsync(context, chunks).ConfigureAwait(false);
        await MergeAssetAsync(context, asset).ConfigureAwait(false);
    }

    private static bool ChunkNameMatches(string chunkName, string hash)
    {
        return hash.AsSpan().Equals(chunkName.AsSpan().Before('_'), StringComparison.OrdinalIgnoreCase);
    }

    private ValueTask MergeAssetAsync(GamePackageServiceContext context, SophonAssetOperation asset)
    {
        return asset.Kind switch
        {
            SophonAssetOperationKind.AddOrRepair => MergeNewAssetAsync(context, asset.NewAsset),
            SophonAssetOperationKind.Modify => MergeDiffAssetAsync(context, asset),
            _ => ValueTask.CompletedTask,
        };
    }

    private async ValueTask MergeDiffAssetAsync(GamePackageServiceContext context, SophonAssetOperation asset)
    {
        CancellationToken token = context.CancellationToken;

        string oldAssetPath = Path.Combine(context.Operation.GameFileSystem.GetGameDirectory(), asset.OldAsset.AssetName);
        if (!File.Exists(oldAssetPath))
        {
            // File not found, skip this asset and repair later
            return;
        }

        using (MemoryStream newAssetStream = memoryStreamFactory.GetStream())
        {
            using (SafeFileHandle oldAssetHandle = File.OpenHandle(oldAssetPath, options: FileOptions.RandomAccess))
            {
                foreach (AssetChunk chunk in asset.NewAsset.AssetChunks)
                {
                    newAssetStream.Position = chunk.ChunkOnFileOffset;

                    if (asset.OldAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5 == chunk.ChunkDecompressedHashMd5) is not { } oldChunk)
                    {
                        string chunkPath = Path.Combine(context.Operation.EffectiveChunksDirectory, chunk.ChunkName);
                        if (!File.Exists(chunkPath))
                        {
                            // File not found, skip this asset and repair later
                            return;
                        }

                        using (await context.ExclusiveProcessChunkAsync(chunk.ChunkName, token).ConfigureAwait(false))
                        {
                            using (FileStream diffStream = File.OpenRead(chunkPath))
                            {
                                using (ZstandardDecompressStream decompressor = new(diffStream))
                                {
                                    await decompressor.CopyToAsync(newAssetStream, token).ConfigureAwait(false);
                                    context.Progress.Report(new GamePackageOperationReport.Install(chunk.ChunkSizeDecompressed, 0, chunk.ChunkName));
                                }
                            }

                            if (context.Operation.Kind is GamePackageOperationKind.Install or GamePackageOperationKind.Update && !context.DuplicatedChunkNames.ContainsKey(chunk.ChunkName))
                            {
                                FileOperation.Delete(chunkPath);
                            }
                        }
                    }
                    else
                    {
                        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(ChunkBufferSize))
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
                                context.Progress.Report(new GamePackageOperationReport.Install(bytesRead, 0, chunk.ChunkName));
                                offset += bytesRead;
                                bytesToCopy -= bytesRead;
                            }
                        }
                    }

                    context.Progress.Report(new GamePackageOperationReport.Install(0, 1, chunk.ChunkName));
                }
            }

            string path = context.EnsureAssetTargetDirectoryExists(asset.NewAsset.AssetName);
            using (FileStream newAssetFileStream = File.Create(path))
            {
                newAssetStream.Position = 0;
                await newAssetStream.CopyToAsync(newAssetFileStream, token).ConfigureAwait(false);
            }
        }
    }

    #endregion

    #region LDiff

    public abstract ValueTask InstallOrPatchAssetsAsync(GamePackageServiceContext context, SophonDecodedPatchBuild patchBuild);

    public abstract ValueTask DeletePatchDeprecatedFilesAsync(GamePackageServiceContext context, SophonDecodedPatchBuild patchBuild);

    public abstract ValueTask PredownloadPatchesAsync(GamePackageServiceContext context, SophonDecodedPatchBuild patchBuild);

    protected static async ValueTask DownloadPatchAsync(GamePackageServiceContext context, SophonPatchAsset asset)
    {
        CancellationToken token = context.CancellationToken;
        token.ThrowIfCancellationRequested();
        Directory.CreateDirectory(context.Operation.EffectiveChunksDirectory);
        string patchPath = Path.Combine(context.Operation.EffectiveChunksDirectory, asset.PatchInfo.Id);

        using (await context.ExclusiveProcessChunkAsync(asset.PatchInfo.Id, token).ConfigureAwait(false))
        {
            if (context.DownloadedPatches.ContainsKey(asset.PatchInfo.Id))
            {
                return;
            }

            if (File.Exists(patchPath))
            {
                if (ChunkNameMatches(asset.PatchInfo.Id, await XxHash64.HashFileAsync(patchPath, token).ConfigureAwait(false)))
                {
                    context.DownloadedPatches.TryAdd(asset.PatchInfo.Id, default);
                    context.Progress.Report(new GamePackageOperationReport.Download(asset.PatchInfo.PatchFileSize, 1, asset.PatchInfo.Id));
                    return;
                }

                File.Delete(patchPath);
            }

            using (FileStream fileStream = File.Create(patchPath))
            {
                fileStream.Position = 0;

                using (Stream webStream = await context.HttpClient.GetStreamAsync(asset.PatchDownloadUrl, token).ConfigureAwait(false))
                {
                    using (StreamCopyWorker<GamePackageOperationReport> worker = new(webStream, fileStream, (bytesRead, _) => new GamePackageOperationReport.Download(bytesRead, 0, asset.PatchInfo.Id)))
                    {
                        await worker.CopyAsync(context.StreamCopyRateLimiter, context.Progress, token).ConfigureAwait(false);

                        fileStream.Position = 0;
                        if (ChunkNameMatches(asset.PatchInfo.Id, await XxHash64.HashAsync(fileStream, token).ConfigureAwait(false)))
                        {
                            context.DownloadedPatches.TryAdd(asset.PatchInfo.Id, default);
                            context.Progress.Report(new GamePackageOperationReport.Download(0, 1, asset.PatchInfo.Id));
                            return;
                        }

                        // We should delete patch file if the hash doesn't match. Retry until all assets rely on this patch file all failed.
                        // Failed assets will be repaired later.
                        File.Delete(patchPath);
                    }
                }
            }
        }
    }

    protected async ValueTask InstallOrPatchAssetAsync(GamePackageServiceContext context, SophonPatchAsset asset)
    {
        CancellationToken token = context.CancellationToken;

        FileData fileData = asset.FileData;
        PatchInfo patchInfo = asset.PatchInfo;

        string assetPath = Path.Combine(context.Operation.GameFileSystem.GetGameDirectory(), fileData.FileName);
        if (File.Exists(assetPath) && fileData.FileHash.Equals(await Hash.FileToHexStringAsync(HashAlgorithmName.MD5, assetPath, token).ConfigureAwait(false), StringComparison.OrdinalIgnoreCase))
        {
            context.Progress.Report(new GamePackageOperationReport.Install(fileData.FileSize, 1, fileData.FileName));
            return;
        }

        long patchStartOffset = patchInfo.PatchStartOffset;
        long patchLength = patchInfo.PatchLength;

        string patchFilePath = Path.Combine(context.Operation.EffectiveChunksDirectory, patchInfo.Id);

        if (!context.DownloadedPatches.ContainsKey(patchInfo.Id))
        {
            await DownloadPatchAsync(context, asset).ConfigureAwait(false);
            if (!File.Exists(patchFilePath))
            {
                // Patch file hash not match, skip this asset and repair later
                return;
            }
        }

        using (SafeFileHandle patchFileHandle = File.OpenHandle(patchFilePath, options: FileOptions.RandomAccess))
        {
            if (string.IsNullOrEmpty(patchInfo.OriginalFileName))
            {
                using (FileStream newAssetStream = File.Create(context.EnsureAssetTargetDirectoryExists(fileData.FileName)))
                {
                    newAssetStream.Position = 0;
                    using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(ChunkBufferSize))
                    {
                        Memory<byte> buffer = memoryOwner.Memory;
                        while (patchLength > 0)
                        {
                            int bytesRead = await RandomAccess.ReadAsync(patchFileHandle, buffer[..(int)Math.Min(buffer.Length, patchLength)], patchStartOffset, token).ConfigureAwait(false);
                            if (bytesRead <= 0)
                            {
                                break;
                            }

                            await newAssetStream.WriteAsync(buffer[..bytesRead], token).ConfigureAwait(false);
                            context.Progress.Report(new GamePackageOperationReport.Install(bytesRead, 0, fileData.FileName));
                            patchStartOffset += bytesRead;
                            patchLength -= bytesRead;
                        }
                    }
                }
            }
            else
            {
                string oldAssetPath = Path.Combine(context.Operation.GameFileSystem.GetGameDirectory(), fileData.FileName);
                if (!File.Exists(oldAssetPath))
                {
                    // File not found, skip this asset and repair later
                    return;
                }

                using (MemoryStream newAssetStream = memoryStreamFactory.GetStreamExactly(fileData.FileSize))
                {
                    SafeFileHandle oldAssetHandle = File.OpenHandle(oldAssetPath, options: FileOptions.RandomAccess);
                    using (FileSegment oldFileSegment = new(oldAssetHandle))
                    {
                        using (FileSegment patchFileSegment = new(patchFileHandle, patchStartOffset, patchLength, false))
                        {
                            if (!HPatch.PatchZstandard(oldFileSegment, patchFileSegment, newAssetStream))
                            {
                                // HPatch failed, skip this asset and repair later
                                return;
                            }
                        }
                    }

                    using (FileStream newAssetFileStream = File.Create(context.EnsureAssetTargetDirectoryExists(fileData.FileName)))
                    {
                        newAssetStream.Position = 0;
                        newAssetFileStream.Position = 0;
                        await newAssetStream.CopyToAsync(newAssetFileStream, token).ConfigureAwait(false);
                    }
                }
            }

            context.Progress.Report(new GamePackageOperationReport.Install(fileData.FileSize, 1, fileData.FileName));
        }
    }

    #endregion
}