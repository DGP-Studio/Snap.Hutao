// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Factory.IO;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Service.Game.Package;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGamePackageService))]
[SuppressMessage("", "CA1001")]
[SuppressMessage("", "SA1204")]
internal sealed partial class GamePackageService : IGamePackageService
{
    private readonly IMemoryStreamFactory memoryStreamFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IProgressFactory progressFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private CancellationTokenSource? operationCts;
    private TaskCompletionSource? operationTcs;
    private IGameAssetsOperationService? gamePackageOperationService;

    public async ValueTask<bool> StartOperationAsync(GamePackageOperationContext context)
    {
        await CancelOperationAsync().ConfigureAwait(false);

        operationCts = new();
        operationTcs = new();
        gamePackageOperationService = serviceProvider
            .GetRequiredService<ISolidStateDriveServiceFactory<IGameAssetsOperationService>>()
            .Create(context.GameFileSystem.GameDirectory);

        ParallelOptions options = new()
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
            GamePackageOperationKind.Install => InstallAsync(context, progress, options),
            GamePackageOperationKind.Verify => VerifyAndRepairAsync(context, progress, options),
            GamePackageOperationKind.Update => UpdateAsync(context, progress, options),
            GamePackageOperationKind.Predownload => PredownloadAsync(context, progress, options),
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
        gamePackageOperationService = null;
    }

    #region Operation

    private async ValueTask InstallAsync(GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions options)
    {
        ArgumentNullException.ThrowIfNull(gamePackageOperationService);

        if (await DecodeManifestsAsync(context.RemoteBranch, context, options.CancellationToken).ConfigureAwait(false) is not { } remoteBuild)
        {
            progress.Report(new GamePackageOperationReport.Reset("清单数据拉取失败"));
            return;
        }

        long totalBytes = remoteBuild.TotalBytes;
        long availableBytes = LogicalDriver.GetAvailableFreeSpace(context.GameFileSystem.GameDirectory);

        if (totalBytes > availableBytes)
        {
            string title = $"磁盘空间不足，需要 {Converters.ToFileSizeString(totalBytes)}，剩余 {Converters.ToFileSizeString(availableBytes)}";
            progress.Report(new GamePackageOperationReport.Reset(title));
            return;
        }

        int totalBlockCount = remoteBuild.TotalBlockCount;
        progress.Report(new GamePackageOperationReport.Reset("正在安装游戏", totalBlockCount, totalBytes));

        await gamePackageOperationService.InstallAssetsAsync(remoteBuild, context, progress, options).ConfigureAwait(false);

        await gamePackageOperationService.ExtractChannelSdkAsync(context, options.CancellationToken).ConfigureAwait(false);

        // Verify
        progress.Report(new GamePackageOperationReport.Reset("正在验证游戏完整性", totalBlockCount, totalBytes));
        GamePackageIntegrityInfo info = await gamePackageOperationService.VerifyGamePackageIntegrityAsync(remoteBuild, context, progress, options).ConfigureAwait(false);

        if (info.NoConflict)
        {
            Directory.Delete(context.GameFileSystem.ChunksDirectory, true);
            progress.Report(new GamePackageOperationReport.Finish(context.OperationKind));
            return;
        }

        (int conflictedBlocks, long conflictedBytes) = info.GetConflictedBlockCountAndByteCount(context.GameChannelSDK);
        progress.Report(new GamePackageOperationReport.Reset("正在修复游戏完整性", conflictedBlocks, conflictedBytes));

        await gamePackageOperationService.RepairGamePackageAsync(info, context, progress, options, options.CancellationToken).ConfigureAwait(false);

        Directory.Delete(context.GameFileSystem.ChunksDirectory, true);
        progress.Report(new GamePackageOperationReport.Finish(context.OperationKind));
    }

    private async ValueTask VerifyAndRepairAsync(GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(gamePackageOperationService);

        if (await DecodeManifestsAsync(context.LocalBranch, context, token).ConfigureAwait(false) is not { } localBuild)
        {
            progress.Report(new GamePackageOperationReport.Reset("清单数据拉取失败"));
            return;
        }

        progress.Report(new GamePackageOperationReport.Reset("正在验证游戏完整性", localBuild.TotalBlockCount, localBuild.TotalBytes));

        GamePackageIntegrityInfo info = await gamePackageOperationService.VerifyGamePackageIntegrityAsync(localBuild, context, progress, parallelOptions, token).ConfigureAwait(false);

        if (info.NoConflict)
        {
            progress.Report(new GamePackageOperationReport.Finish(context.OperationKind));
            return;
        }

        (int conflictBlocks, long conflictBytes) = info.GetConflictedBlockCountAndByteCount(context.GameChannelSDK);
        progress.Report(new GamePackageOperationReport.Reset("正在修复游戏完整性", conflictBlocks, conflictBytes));

        await gamePackageOperationService.RepairGamePackageAsync(info, context, progress, parallelOptions, token).ConfigureAwait(false);

        Directory.Delete(context.GameFileSystem.ChunksDirectory, true);
        progress.Report(new GamePackageOperationReport.Finish(context.OperationKind, true));
    }

    private async ValueTask UpdateAsync(GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(gamePackageOperationService);

        if (await DecodeManifestsAsync(context.LocalBranch, context, token).ConfigureAwait(false) is not { } localBuild ||
            await DecodeManifestsAsync(context.RemoteBranch, context, token).ConfigureAwait(false) is not { } remoteBuild)
        {
            progress.Report(new GamePackageOperationReport.Reset("清单数据拉取失败"));
            return;
        }

        ParseDiff(localBuild, remoteBuild, out List<SophonAsset> addedAssets, out List<SophonModifiedAsset> modifiedAssets, out List<AssetProperty> deletedAssets);

        int totalBlocks = addedAssets.Sum(a => a.AssetProperty.AssetChunks.Count) + modifiedAssets.Sum(a => a.DiffChunks.Count);
        long totalBytes = addedAssets.Sum(a => a.AssetProperty.AssetSize) + modifiedAssets.Sum(a => a.DiffChunks.Sum(c => c.AssetChunk.ChunkSizeDecompressed));

        long availableBytes = LogicalDriver.GetAvailableFreeSpace(context.GameFileSystem.GameDirectory);

        if (totalBytes > availableBytes)
        {
            string title = $"磁盘空间不足，需要 {Converters.ToFileSizeString(totalBytes)}，剩余 {Converters.ToFileSizeString(availableBytes)}";
            progress.Report(new GamePackageOperationReport.Reset(title));
            return;
        }

        progress.Report(new GamePackageOperationReport.Reset("正在更新游戏", totalBlocks, totalBytes));

        // Added
        await gamePackageOperationService.AddAssetsAsync(addedAssets, context, progress, parallelOptions).ConfigureAwait(false);

        // Modified
        // 内容未发生变化但是偏移量发生变化的块，从旧asset读取并写入新asset流
        // 内容发生变化的块直接读取diff chunk写入新asset流
        await gamePackageOperationService.UpdateModifiedAssetsAsync(modifiedAssets, context, progress, parallelOptions).ConfigureAwait(false);

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

        await gamePackageOperationService.ExtractChannelSdkAsync(context, token).ConfigureAwait(false);

        // Verify
        progress.Report(new GamePackageOperationReport.Reset("正在验证游戏完整性", remoteBuild.Manifests.Sum(m => m.ManifestProto.Assets.Sum(a => a.AssetChunks.Count)), remoteBuild.TotalBytes));

        GamePackageIntegrityInfo info = await gamePackageOperationService.VerifyGamePackageIntegrityAsync(remoteBuild, context, progress, parallelOptions, token).ConfigureAwait(false);

        if (info.NoConflict)
        {
            Directory.Delete(context.GameFileSystem.ChunksDirectory, true);
            progress.Report(new GamePackageOperationReport.Finish(context.OperationKind));
            return;
        }

        (int conflictBlocks, long conflictBytes) = info.GetConflictedBlockCountAndByteCount(context.GameChannelSDK);
        progress.Report(new GamePackageOperationReport.Reset("正在修复游戏完整性", conflictBlocks, conflictBytes));

        await gamePackageOperationService.RepairGamePackageAsync(info, context, progress, parallelOptions, token).ConfigureAwait(false);

        Directory.Delete(context.GameFileSystem.ChunksDirectory, true);
        progress.Report(new GamePackageOperationReport.Finish(context.OperationKind));
    }

    private async ValueTask PredownloadAsync(GamePackageOperationContext context, IProgress<GamePackageOperationReport> progress, ParallelOptions parallelOptions, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(gamePackageOperationService);

        if (await DecodeManifestsAsync(context.LocalBranch, context, token).ConfigureAwait(false) is not { } localBuild ||
            await DecodeManifestsAsync(context.RemoteBranch, context, token).ConfigureAwait(false) is not { } remoteBuild)
        {
            progress.Report(new GamePackageOperationReport.Reset("清单数据拉取失败"));
            return;
        }

        // 和Update相比不需要处理delete，不需要Combine
        ParseDiff(localBuild, remoteBuild, out List<SophonAsset> addedAssets, out List<SophonModifiedAsset> modifiedAssets, out _);

        // Download
        int totalBlocks = addedAssets.Sum(a => a.AssetProperty.AssetChunks.Count) + modifiedAssets.Sum(a => a.DiffChunks.Count);
        long totalBytes = addedAssets.Sum(a => a.AssetProperty.AssetSize) + modifiedAssets.Sum(a => a.DiffChunks.Sum(c => c.AssetChunk.ChunkSizeDecompressed));

        long availableBytes = LogicalDriver.GetAvailableFreeSpace(context.GameFileSystem.GameDirectory);

        if (totalBytes > availableBytes)
        {
            string title = $"磁盘空间不足，需要 {Converters.ToFileSizeString(totalBytes)}，剩余 {Converters.ToFileSizeString(availableBytes)}";
            progress.Report(new GamePackageOperationReport.Reset(title));
            return;
        }

        progress.Report(new GamePackageOperationReport.Reset("正在预下载资源", totalBlocks, totalBytes));

        PredownloadStatus predownloadStatus = new(context.RemoteBranch.Tag, false, totalBlocks);
        using (FileStream predownloadStatusStream = File.Create(context.GameFileSystem.PredownloadStatusPath))
        {
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions, token).ConfigureAwait(false);
        }

        // Added
        await gamePackageOperationService.PredownloadAddedAssetsAsync(addedAssets, context, progress, parallelOptions).ConfigureAwait(false);

        // Modified
        await gamePackageOperationService.PredownloadModifiedAssetsAsync(modifiedAssets, context, progress, parallelOptions).ConfigureAwait(false);

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

            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(GameAssetsOperationServiceSSD)))
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

    private static void ParseDiff(SophonDecodedBuild localDecodedBuild, SophonDecodedBuild remoteDecodedBuild, out List<SophonAsset> addedAssets, out List<SophonModifiedAsset> modifiedAssets, out List<AssetProperty> deletedAssets)
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

                modifiedAssets.Add(new(
                    remoteManifest.UrlPrefix,
                    localAsset,
                    asset,
                    asset.AssetChunks.Except(localAsset.AssetChunks, AssetChunkMd5Comparer.Shared).Select(ac => new SophonChunk(remoteManifest.UrlPrefix, ac)).ToList()));
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
}