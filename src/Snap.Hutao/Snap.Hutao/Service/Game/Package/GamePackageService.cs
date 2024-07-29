// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.IO;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using Snap.Hutao.Web.Response;
using System.Buffers;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace Snap.Hutao.Service.Game.Package;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGamePackageService))]
[SuppressMessage("", "CA1001")]
[SuppressMessage("", "SA1204")]
internal sealed partial class GamePackageService : IGamePackageService
{
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IMemoryStreamFactory memoryStreamFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IProgressFactory progressFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly LaunchOptions launchOptions;
    private readonly ITaskContext taskContext;

    private CancellationTokenSource? operationCts;
    private TaskCompletionSource? operationTcs;

    public async ValueTask<bool> StartOperationAsync(GamePackageOperationContext context)
    {
        await CancelOperationAsync().ConfigureAwait(false);

        operationCts = new();
        operationTcs = new();
        ParallelOptions parallelOptions = new()
        {
            CancellationToken = operationCts.Token,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };

        await taskContext.SwitchToMainThreadAsync();
        GamePackageOperationWindow window = serviceProvider.GetRequiredService<GamePackageOperationWindow>();
        IProgress<GamePackageOperationReport> progress = progressFactory.CreateForMainThread<GamePackageOperationReport>(((GamePackageOperationViewModel)window.DataContext).HandleProgressUpdate);

        await taskContext.SwitchToBackgroundAsync();
        ValueTask operation = context.OperationKind switch
        {
            GamePackageOperationKind.Install => InstallAsync(context, progress, parallelOptions),
            GamePackageOperationKind.Verify => VerifyAndRepairAsync(context, progress, parallelOptions),
            GamePackageOperationKind.Update => UpdateAsync(context, progress, parallelOptions),
            GamePackageOperationKind.Predownload => PredownloadAsync(context, progress, parallelOptions),
            _ => ValueTask.CompletedTask,
        };

        try
        {
            await operation.ConfigureAwait(false);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
        finally
        {
            operationTcs.TrySetResult();
        }
    }

    public async ValueTask CancelOperationAsync()
    {
        if (operationCts is null || operationTcs is null)
        {
            return;
        }

        await operationCts.CancelAsync().ConfigureAwait(false);
        await operationTcs.Task.ConfigureAwait(false);
        operationCts.Dispose();
        operationCts = null;
        operationTcs = null;
    }

    // TODO: Check if the block count and byte count are correct
    #region Operation

    private async ValueTask InstallAsync(GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions options)
    {
        if (await DecodeManifestsAsync(context.RemoteBranch, context, options.CancellationToken).ConfigureAwait(false) is not { } decodedBuild)
        {
            progress.Report(new GamePackageOperationReport.Reset("清单数据拉取失败"));
            return;
        }

        long totalBytes = decodedBuild.TotalBytes;
        long availableBytes = LogicalDriver.GetAvailableFreeSpace(context.GameFileSystem.GameDirectory);

        if (totalBytes > availableBytes)
        {
            string title = $"磁盘空间不足，需要 {Converters.ToFileSizeString(totalBytes)}，剩余 {Converters.ToFileSizeString(availableBytes)}";
            progress.Report(new GamePackageOperationReport.Reset(title));
            return;
        }

        int totalBlockCount = decodedBuild.TotalBlockCount;
        progress.Report(new GamePackageOperationReport.Reset("正在安装游戏", totalBlockCount, totalBytes));

        foreach (SophonDecodedManifest manifest in decodedBuild.Manifests)
        {
            IEnumerable<SophonAsset> sophonAssets = manifest.ManifestProto.Assets
                .Select(a => new SophonAsset(manifest.UrlPrefix, a));
            await Parallel.ForEachAsync(sophonAssets, options, (asset, token) => AddNewAssetAsync(asset, context, progress, options)).ConfigureAwait(false);
        }

        await ExtractChannelSdkAsync(context, options.CancellationToken).ConfigureAwait(false);

        // Verify
        progress.Report(new GamePackageOperationReport.Reset("正在验证游戏完整性", totalBlockCount, totalBytes));

        IntegrityInfo info = await VerifyCoreAsync(decodedBuild, context, progress, options).ConfigureAwait(false);

        if (info.NoConflict)
        {
            progress.Report(new GamePackageOperationReport.Finish(context.OperationKind));
            Directory.Delete(context.GameFileSystem.ChunksDirectory, true);
            return;
        }

        (int conflictBlocks, long conflictBytes) = info.GetConflictedBlockCountAndByteCount(context.GameChannelSDK);
        progress.Report(new GamePackageOperationReport.Reset("正在修复游戏完整性", conflictBlocks, conflictBytes));

        await RepairCoreAsync(info, context, progress, options, options.CancellationToken).ConfigureAwait(false);

        Directory.Delete(context.GameFileSystem.ChunksDirectory, true);
        progress.Report(new GamePackageOperationReport.Finish(context.OperationKind));
    }

    private async ValueTask VerifyAndRepairAsync(GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, CancellationToken token = default)
    {
        if (await DecodeManifestsAsync(context.LocalBranch, context, token).ConfigureAwait(false) is not { } sophonDecodedBuild)
        {
            progress.Report(new GamePackageOperationReport.Reset("清单数据拉取失败"));
            return;
        }

        progress.Report(new GamePackageOperationReport.Reset("正在验证游戏完整性", sophonDecodedBuild.TotalBlockCount, sophonDecodedBuild.TotalBytes));

        IntegrityInfo info = await VerifyCoreAsync(sophonDecodedBuild, context, progress, parallelOptions, token).ConfigureAwait(false);

        if (info.NoConflict)
        {
            progress.Report(new GamePackageOperationReport.Finish(context.OperationKind));
            return;
        }

        (int conflictBlocks, long conflictBytes) = info.GetConflictedBlockCountAndByteCount(context.GameChannelSDK);
        progress.Report(new GamePackageOperationReport.Reset("正在修复游戏完整性", conflictBlocks, conflictBytes));

        await RepairCoreAsync(info, context, progress, parallelOptions, token).ConfigureAwait(false);

        Directory.Delete(context.GameFileSystem.ChunksDirectory, true);
        progress.Report(new GamePackageOperationReport.Finish(context.OperationKind, true));
    }

    private async ValueTask UpdateAsync(GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, CancellationToken token = default)
    {
        SophonDecodedBuild? localDecodedBuild = await DecodeManifestsAsync(context.LocalBranch, context, token).ConfigureAwait(false);
        SophonDecodedBuild? remoteDecodedBuild = await DecodeManifestsAsync(context.RemoteBranch, context, token).ConfigureAwait(false);

        if (localDecodedBuild is null || remoteDecodedBuild is null)
        {
            progress.Report(new GamePackageOperationReport.Reset("清单数据拉取失败"));
            return;
        }

        ParseDiff(localDecodedBuild, remoteDecodedBuild, out List<SophonAsset> addedAssets, out Dictionary<AssetProperty, SophonAsset> modifiedAssets, out List<AssetProperty> deletedAssets);

        int totalBlocks = addedAssets.Sum(a => a.AssetProperty.AssetChunks.Count) + modifiedAssets.Sum(a => a.Value.DiffChunks.Count);
        long totalBytes = addedAssets.Sum(a => a.AssetProperty.AssetSize) + modifiedAssets.Sum(a => a.Value.DiffChunks.Sum(c => c.AssetChunk.ChunkSizeDecompressed));

        long availableBytes = LogicalDriver.GetAvailableFreeSpace(context.GameFileSystem.GameDirectory);

        if (totalBytes > availableBytes)
        {
            string title = $"磁盘空间不足，需要 {Converters.ToFileSizeString(totalBytes)}，剩余 {Converters.ToFileSizeString(availableBytes)}";
            progress.Report(new GamePackageOperationReport.Reset(title));
            return;
        }

        progress.Report(new GamePackageOperationReport.Reset("正在更新游戏", totalBlocks, totalBytes));

        // Added
        await Parallel.ForEachAsync(addedAssets, parallelOptions, (asset, token) => AddNewAssetAsync(asset, context, progress, parallelOptions)).ConfigureAwait(false);

        // Modified
        // 内容未发生变化但是偏移量发生变化的块，从旧asset读取并写入新asset流
        // 内容发生变化的块直接读取diff chunk写入新asset流
        await Parallel.ForEachAsync(modifiedAssets, parallelOptions, async (asset, token) =>
        {
            foreach (SophonChunk sophonChunk in asset.Value.DiffChunks)
            {
                await DownloadChunkAsync(sophonChunk, context, progress, token).ConfigureAwait(false);
            }

            await MergeDiffAssetAsync(asset.Key, asset.Value, context, token).ConfigureAwait(false);
        }).ConfigureAwait(false);

        // Deleted
        foreach (AssetProperty asset in deletedAssets)
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

        await ExtractChannelSdkAsync(context, token).ConfigureAwait(false);

        // Verify
        progress.Report(new GamePackageOperationReport.Reset("正在验证游戏完整性", remoteDecodedBuild.Manifests.Sum(m => m.ManifestProto.Assets.Sum(a => a.AssetChunks.Count)), remoteDecodedBuild.TotalBytes));

        IntegrityInfo info = await VerifyCoreAsync(remoteDecodedBuild, context, progress, parallelOptions, token).ConfigureAwait(false);

        if (info.NoConflict)
        {
            Directory.Delete(context.GameFileSystem.ChunksDirectory, true);
            progress.Report(new GamePackageOperationReport.Finish(context.OperationKind));
            return;
        }

        (int conflictBlocks, long conflictBytes) = info.GetConflictedBlockCountAndByteCount(context.GameChannelSDK);
        progress.Report(new GamePackageOperationReport.Reset("正在修复游戏完整性", conflictBlocks, conflictBytes));

        await RepairCoreAsync(info, context, progress, parallelOptions, token).ConfigureAwait(false);

        Directory.Delete(context.GameFileSystem.ChunksDirectory, true);
        progress.Report(new GamePackageOperationReport.Finish(context.OperationKind));
    }

    private async ValueTask PredownloadAsync(GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, CancellationToken token = default)
    {
        SophonDecodedBuild? localDecodedBuild = await DecodeManifestsAsync(context.LocalBranch, context, token).ConfigureAwait(false);
        SophonDecodedBuild? remoteDecodedBuild = await DecodeManifestsAsync(context.RemoteBranch, context, token).ConfigureAwait(false);
        if (localDecodedBuild is null || remoteDecodedBuild is null)
        {
            progress.Report(new GamePackageOperationReport.Reset("清单数据拉取失败"));
            return;
        }

        // 和Update相比不需要处理delete，不需要Combine
        ParseDiff(localDecodedBuild, remoteDecodedBuild, out List<SophonAsset> addedAssets, out Dictionary<AssetProperty, SophonAsset> modifiedAssets, out _);

        // Download
        int totalBlocks = addedAssets.Sum(a => a.AssetProperty.AssetChunks.Count) + modifiedAssets.Sum(a => a.Value.DiffChunks.Count);
        long totalBytes = addedAssets.Sum(a => a.AssetProperty.AssetSize) + modifiedAssets.Sum(a => a.Value.DiffChunks.Sum(c => c.AssetChunk.ChunkSizeDecompressed));

        long availableBytes = LogicalDriver.GetAvailableFreeSpace(context.GameFileSystem.GameDirectory);

        if (totalBytes > availableBytes)
        {
            string title = $"磁盘空间不足，需要 {Converters.ToFileSizeString(totalBytes)}，剩余 {Converters.ToFileSizeString(availableBytes)}";
            progress.Report(new GamePackageOperationReport.Reset(title));
            return;
        }

        progress.Report(new GamePackageOperationReport.Reset("正在预下载游戏", totalBlocks, totalBytes));

        PredownloadStatus predownloadStatus = new(context.RemoteBranch.Tag, false, totalBlocks);
        using (FileStream predownloadStatusStream = File.Create(context.GameFileSystem.PredownloadStatusPath))
        {
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions, token).ConfigureAwait(false);
        }

        // Added
        await Parallel.ForEachAsync(addedAssets, parallelOptions, (asset, token) => DownloadAssetChunksAsync(asset, context, progress, parallelOptions)).ConfigureAwait(false);

        // Modified
        await Parallel.ForEachAsync(modifiedAssets, parallelOptions, async (asset, token) =>
        {
            foreach (SophonChunk sophonChunk in asset.Value.DiffChunks)
            {
                await DownloadChunkAsync(sophonChunk, context, progress, token).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);

        progress.Report(new GamePackageOperationReport.Finish(context.OperationKind));

        using (FileStream predownloadStatusStream = File.Create(context.GameFileSystem.PredownloadStatusPath))
        {
            predownloadStatus.Finished = true;
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions, token).ConfigureAwait(false);
        }

        return;
    }

    #endregion

    #region Parse

    private async ValueTask<SophonDecodedBuild?> DecodeManifestsAsync(BranchWrapper branch, GamePackageOperationContext context, CancellationToken token = default)
    {
        Response<SophonBuild> response;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ISophonClient client = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISophonClient>>()
                .Create(LaunchScheme.ExecutableIsOversea(context.GameFileSystem.GameFileName));

            response = await client.GetBuildAsync(branch, token).ConfigureAwait(false);
        }

        if (!response.IsOk())
        {
            return default!;
        }

        long totalBytes = 0L;
        List<SophonDecodedManifest> decodedManifests = [];
        foreach (SophonManifest sophonManifest in response.Data.Manifests)
        {
            bool exclude = sophonManifest.MatchingField switch
            {
                "game" => false,
                "zh-cn" => !context.GameFileSystem.GameAudioSystem.Chinese,
                "en-us" => !context.GameFileSystem.GameAudioSystem.English,
                "ja-jp" => !context.GameFileSystem.GameAudioSystem.Japanese,
                "ko-kr" => !context.GameFileSystem.GameAudioSystem.Korean,
                _ => true,
            };

            if (exclude)
            {
                continue;
            }

            totalBytes += sophonManifest.Stats.UncompressedSize;
            string manifestDownloadUrl = $"{sophonManifest.ManifestDownload.UrlPrefix}/{sophonManifest.Manifest.Id}";

            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(GamePackageService)))
            {
                using (Stream rawManifestStream = await httpClient.GetStreamAsync(manifestDownloadUrl, token).ConfigureAwait(false))
                {
                    using (ZstandardDecompressionStream decompressor = new(rawManifestStream))
                    {
                        using (MemoryStream inMemoryManifestStream = await memoryStreamFactory.GetStreamAsync(decompressor).ConfigureAwait(false))
                        {
                            string manifestMd5 = await MD5.HashAsync(inMemoryManifestStream, token).ConfigureAwait(false);
                            if (manifestMd5.Equals(sophonManifest.Manifest.Checksum, StringComparison.OrdinalIgnoreCase))
                            {
                                inMemoryManifestStream.Position = 0;
                                decodedManifests.Add(new(sophonManifest.ChunkDownload.UrlPrefix, SophonManifestProto.Parser.ParseFrom(inMemoryManifestStream)));
                            }
                        }
                    }
                }
            }
        }

        return new(totalBytes, decodedManifests);
    }

    private static void ParseDiff(SophonDecodedBuild localDecodedBuild, SophonDecodedBuild remoteDecodedBuild, out List<SophonAsset> addedAssets, out Dictionary<AssetProperty, SophonAsset> modifiedAssets, out List<AssetProperty> deletedAssets)
    {
        addedAssets = [];
        modifiedAssets = [];
        deletedAssets = [];

        // Add
        // 本地没有，远端有
        foreach ((SophonDecodedManifest localManifest, SophonDecodedManifest remoteManifest) in localDecodedBuild.Manifests.Zip(remoteDecodedBuild.Manifests))
        {
            foreach (SophonAsset sophonAsset in remoteManifest.ManifestProto.Assets.Except(localManifest.ManifestProto.Assets, AssetPropertyNameComparer.Shared).Select(ap => new SophonAsset(remoteManifest.UrlPrefix, ap)))
            {
                addedAssets.Add(sophonAsset);
            }
        }

        // Modify
        // 本地有，远端有，但是内容不一致
        foreach ((SophonDecodedManifest localManifest, SophonDecodedManifest remoteManifest) in localDecodedBuild.Manifests.Zip(remoteDecodedBuild.Manifests))
        {
            foreach (AssetProperty asset in remoteManifest.ManifestProto.Assets)
            {
                AssetProperty? localAsset = localManifest.ManifestProto.Assets.FirstOrDefault(a => a.AssetName.Equals(asset.AssetName, StringComparison.OrdinalIgnoreCase));
                if (localAsset is null)
                {
                    continue;
                }

                if (localAsset.AssetHashMd5.Equals(asset.AssetHashMd5, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                modifiedAssets.Add(localAsset, new(remoteManifest.UrlPrefix, asset, asset.AssetChunks.Except(localAsset.AssetChunks, AssetChunkMd5Comparer.Shared).Select(ac => new SophonChunk(remoteManifest.UrlPrefix, ac)).ToList()));
            }
        }

        // Delete
        // 本地有，远端没有
        foreach ((SophonDecodedManifest localManifest, SophonDecodedManifest remoteManifest) in localDecodedBuild.Manifests.Zip(remoteDecodedBuild.Manifests))
        {
            localManifest.ManifestProto.Assets.Except(remoteManifest.ManifestProto.Assets, AssetPropertyNameComparer.Shared).ToList().ForEach(deletedAssets.Add);
        }
    }

    #endregion

    #region Asset Operation

    private async ValueTask<IntegrityInfo> VerifyCoreAsync(SophonDecodedBuild sophonDecodedBuild, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, CancellationToken token = default)
    {
        List<SophonAsset> conflictedAssets = [];
        bool channelSdkConflict = false;

        foreach (SophonDecodedManifest sophonDecodedManifest in sophonDecodedBuild.Manifests)
        {
            await Parallel.ForEachAsync(sophonDecodedManifest.ManifestProto.Assets, parallelOptions, (asset, token) => VerifyAssetAsync(new(sophonDecodedManifest.UrlPrefix, asset), conflictedAssets, context, progress, token)).ConfigureAwait(false);
        }

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
                            string localMd5 = await MD5.HashFileAsync(path, token).ConfigureAwait(false);
                            if (!localMd5.Equals(item.Md5, StringComparison.OrdinalIgnoreCase))
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

        return new IntegrityInfo()
        {
            ConflictedAssets = conflictedAssets,
            ChannelSdkConflicted = channelSdkConflict,
        };
    }

    private async ValueTask VerifyAssetAsync(SophonAsset sophonAsset, List<SophonAsset> conflictAssets, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, CancellationToken token = default)
    {
        string assetPath = Path.Combine(context.GameFileSystem.GameDirectory, sophonAsset.AssetProperty.AssetName);

        if (sophonAsset.AssetProperty.AssetType is 64)
        {
            Directory.CreateDirectory(assetPath);
            return;
        }

        if (!File.Exists(assetPath))
        {
            conflictAssets.Add(sophonAsset);
            for (int i = 0; i < sophonAsset.AssetProperty.AssetChunks.Count; i++)
            {
                progress.Report(new GamePackageOperationReport.Update(0, true));
            }

            return;
        }

        using (FileStream fileStream = File.OpenRead(assetPath))
        {
            for (int i = 0; i < sophonAsset.AssetProperty.AssetChunks.Count; i++)
            {
                AssetChunk chunk = sophonAsset.AssetProperty.AssetChunks[i];
                using (Stream hashStream = await fileStream.CloneSegmentAsync(chunk.ChunkOnFileOffset, chunk.ChunkSizeDecompressed, memoryStreamFactory).ConfigureAwait(false))
                {
                    string chunkMd5 = await MD5.HashAsync(hashStream, token).ConfigureAwait(false);
                    if (!chunkMd5.Equals(chunk.ChunkDecompressedHashMd5, StringComparison.OrdinalIgnoreCase))
                    {
                        conflictAssets.Add(sophonAsset);
                        for (int j = i; j < sophonAsset.AssetProperty.AssetChunks.Count; j++)
                        {
                            progress.Report(new GamePackageOperationReport.Update(0, true));
                        }

                        return;
                    }

                    progress.Report(new GamePackageOperationReport.Update(chunk.ChunkSizeDecompressed, true));
                }
            }
        }
    }

    private async ValueTask RepairCoreAsync(IntegrityInfo info, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, CancellationToken token = default)
    {
        await Parallel.ForEachAsync(info.ConflictedAssets, parallelOptions, async (asset, token) =>
        {
            await DownloadAssetChunksAsync(asset, context, progress, parallelOptions).ConfigureAwait(false);
            await MergeAssetAsync(asset.AssetProperty, context, token).ConfigureAwait(false);
        }).ConfigureAwait(false);

        if (info.ChannelSdkConflicted)
        {
            ArgumentNullException.ThrowIfNull(context.GameChannelSDK);
            await ExtractChannelSdkAsync(context, token).ConfigureAwait(false);

            progress.Report(new GamePackageOperationReport.Update(context.GameChannelSDK.ChannelSdkPackage.Size, true));
        }
    }

    private async ValueTask AddNewAssetAsync(SophonAsset asset, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions options)
    {
        // Folder
        if (asset.AssetProperty.AssetType is 64)
        {
            Directory.CreateDirectory(Path.Combine(context.GameFileSystem.GameDirectory, asset.AssetProperty.AssetName));
            return;
        }

        await DownloadAssetChunksAsync(asset, context, progress, options).ConfigureAwait(false);
        await MergeAssetAsync(asset.AssetProperty, context).ConfigureAwait(false);
    }

    private async ValueTask DownloadAssetChunksAsync(SophonAsset sophonAsset, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions)
    {
        await Parallel.ForEachAsync(sophonAsset.AssetProperty.AssetChunks, parallelOptions, (chunk, token) => DownloadChunkAsync(new(sophonAsset.UrlPrefix, chunk), context, progress, token)).ConfigureAwait(false);
    }

    private async ValueTask DownloadChunkAsync(SophonChunk sophonChunk, GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, CancellationToken token = default)
    {
        Directory.CreateDirectory(context.GameFileSystem.ChunksDirectory);
        string chunkPath = Path.Combine(context.GameFileSystem.ChunksDirectory, sophonChunk.AssetChunk.ChunkName);
        if (File.Exists(chunkPath))
        {
            string chunkXxh64 = await XXH64.HashFileAsync(chunkPath, token).ConfigureAwait(false);
            if (chunkXxh64.Equals(sophonChunk.AssetChunk.ChunkName.Split("_")[0], StringComparison.OrdinalIgnoreCase))
            {
                progress.Report(new GamePackageOperationReport.Update(sophonChunk.AssetChunk.ChunkSize, true));
                return;
            }

            File.Delete(chunkPath);
        }

        using (FileStream fileStream = File.Create(chunkPath))
        {
            fileStream.Position = 0;

            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(GamePackageService)))
            {
                using (HttpResponseMessage responseMessage = await httpClient.GetAsync(sophonChunk.ChunkDownloadUrl, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
                {
                    long totalBytes = responseMessage.Content.Headers.ContentLength ?? 0;
                    using (Stream webStream = await responseMessage.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
                    {
                        StreamCopyWorker<GamePackageOperationReport> worker = new(webStream, fileStream, bytesRead => new GamePackageOperationReport.Update(bytesRead, false));

                        await worker.CopyAsync(progress).ConfigureAwait(false);

                        fileStream.Position = 0;
                        string chunkXxh64 = await XXH64.HashAsync(fileStream, token).ConfigureAwait(false);
                        if (chunkXxh64.Equals(sophonChunk.AssetChunk.ChunkName.Split("_")[0], StringComparison.OrdinalIgnoreCase))
                        {
                            progress.Report(new GamePackageOperationReport.Update(totalBytes, true));
                        }
                    }
                }
            }
        }
    }

    private static async ValueTask MergeAssetAsync(AssetProperty assetProperty, GamePackageOperationContext context, CancellationToken token = default)
    {
        string path = Path.Combine(context.GameFileSystem.GameDirectory, assetProperty.AssetName);
        string? directory = Path.GetDirectoryName(path);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);
        //using (FileStream file = File.Create(path))
        //{
        //    foreach (AssetChunk chunk in assetProperty.AssetChunks)
        //    {
        //        string chunkPath = Path.Combine(context.GameFileSystem.ChunksDirectory, chunk.ChunkName);
        //        using (FileStream chunkFile = File.OpenRead(chunkPath))
        //        {
        //            using (ZstandardDecompressionStream decompressionStream = new(chunkFile))
        //            {
        //                file.Position = chunk.ChunkOnFileOffset;
        //                await decompressionStream.CopyToAsync(file, token).ConfigureAwait(false);
        //            }
        //        }
        //    }
        //}

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

    private async ValueTask MergeDiffAssetAsync(AssetProperty oldAsset, SophonAsset newAsset, GamePackageOperationContext context, CancellationToken token = default)
    {
        using (MemoryStream newAssetStream = memoryStreamFactory.GetStream())
        {
            //using (FileStream oldAssetStream = File.OpenRead(Path.Combine(context.GameFileSystem.GameDirectory, oldAsset.AssetName)))
            //{
            //    foreach (AssetChunk chunk in newAsset.AssetProperty.AssetChunks)
            //    {
            //        newAssetStream.Position = chunk.ChunkOnFileOffset;

            //        AssetChunk? oldChunk = oldAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5 == chunk.ChunkDecompressedHashMd5);
            //        if (oldChunk is null)
            //        {
            //            using (FileStream diffStream = File.OpenRead(Path.Combine(context.GameFileSystem.ChunksDirectory, chunk.ChunkName)))
            //            {
            //                using (ZstandardDecompressionStream decompressionStream = new(diffStream))
            //                {
            //                    await decompressionStream.CopyToAsync(newAssetStream, token).ConfigureAwait(false);
            //                    continue;
            //                }
            //            }
            //        }

            //        oldAssetStream.Position = oldChunk.ChunkOnFileOffset;
            //        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(81920))
            //        {
            //            Memory<byte> buffer = memoryOwner.Memory;
            //            long bytesToCopy = oldChunk.ChunkSizeDecompressed;
            //            while (bytesToCopy > 0)
            //            {
            //                int bytesRead = await oldAssetStream.ReadAsync(buffer[..(int)Math.Min(buffer.Length, bytesToCopy)], token).ConfigureAwait(false);
            //                if (bytesRead <= 0)
            //                {
            //                    break;
            //                }

            //                await newAssetStream.WriteAsync(buffer[..bytesRead], token).ConfigureAwait(false);
            //                bytesToCopy -= bytesRead;
            //            }
            //        }
            //    }

            //    using (FileStream newAssetFileStream = File.Create(Path.Combine(context.GameFileSystem.GameDirectory, newAsset.AssetProperty.AssetName)))
            //    {
            //        newAssetStream.Position = 0;
            //        await newAssetStream.CopyToAsync(newAssetFileStream, token).ConfigureAwait(false);
            //    }
            //}

            using (SafeFileHandle oldAssetHandle = File.OpenHandle(Path.Combine(context.GameFileSystem.GameDirectory, oldAsset.AssetName), FileMode.Open, FileAccess.Read, FileShare.None))
            {
                foreach (AssetChunk chunk in newAsset.AssetProperty.AssetChunks)
                {
                    newAssetStream.Position = chunk.ChunkOnFileOffset;

                    AssetChunk? oldChunk = oldAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5 == chunk.ChunkDecompressedHashMd5);
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

                using (FileStream newAssetFileStream = File.Create(Path.Combine(context.GameFileSystem.GameDirectory, newAsset.AssetProperty.AssetName)))
                {
                    newAssetStream.Position = 0;
                    await newAssetStream.CopyToAsync(newAssetFileStream, token).ConfigureAwait(false);
                }
            }
        }
    }

    private async ValueTask ExtractChannelSdkAsync(GamePackageOperationContext context, CancellationToken token = default)
    {
        if (context.GameChannelSDK is not null)
        {
            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(GamePackageService)))
            {
                using (Stream sdkStream = await httpClient.GetStreamAsync(context.GameChannelSDK.ChannelSdkPackage.Url, token).ConfigureAwait(false))
                {
                    ZipFile.ExtractToDirectory(sdkStream, context.GameFileSystem.GameDirectory, true);
                }
            }
        }
    }

    #endregion

    private sealed class IntegrityInfo
    {
        public required List<SophonAsset> ConflictedAssets { get; init; }

        public required bool ChannelSdkConflicted { get; init; }

        public bool NoConflict { get => ConflictedAssets is [] && !ChannelSdkConflicted; }

        public (int BlockCount, long ByteCount) GetConflictedBlockCountAndByteCount(GameChannelSDK? sdk)
        {
            int conflictBlocks = ConflictedAssets.Sum(a => a.AssetProperty.AssetChunks.Count);
            long conflictBytes = ConflictedAssets.Sum(a => a.AssetProperty.AssetSize);

            if (ChannelSdkConflicted)
            {
                ArgumentNullException.ThrowIfNull(sdk);
                conflictBlocks++;
                conflictBytes += sdk.ChannelSdkPackage.Size;
            }

            return (conflictBlocks, conflictBytes);
        }
    }
}