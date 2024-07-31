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
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Game.Package.Advanced;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGamePackageService))]
[SuppressMessage("", "CA1001")]
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

    public async ValueTask<bool> StartOperationAsync(GamePackageOperationContext operationContext)
    {
        await CancelOperationAsync().ConfigureAwait(false);

        operationCts = new();
        operationTcs = new();

        ParallelOptions options = new()
        {
            CancellationToken = operationCts.Token,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };

        await taskContext.SwitchToMainThreadAsync();
        GamePackageOperationWindow window = serviceProvider.GetRequiredService<GamePackageOperationWindow>();
        IProgress<GamePackageOperationReport> progress = progressFactory.CreateForMainThread<GamePackageOperationReport>(((GamePackageOperationViewModel)window.DataContext).HandleProgressUpdate);

        GamePackageServiceContext serviceContext = new(operationContext, progress, options);

        await taskContext.SwitchToBackgroundAsync();
        Func<GamePackageServiceContext, ValueTask> operation = operationContext.OperationKind switch
        {
            GamePackageOperationKind.Install => InstallAsync,
            GamePackageOperationKind.Verify => VerifyAndRepairAsync,
            GamePackageOperationKind.Update => UpdateAsync,
            GamePackageOperationKind.Predownload => PredownloadAsync,
            _ => context => ValueTask.CompletedTask,
        };

        try
        {
            await operation(serviceContext).ConfigureAwait(false);
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

    private async ValueTask InstallAsync(GamePackageServiceContext context)
    {
        if (await DecodeManifestsAsync(context.Operation.RemoteBranch, context).ConfigureAwait(false) is not { } remoteBuild)
        {
            context.Progress.Report(new GamePackageOperationReport.Reset("清单数据拉取失败"));
            return;
        }

        long totalBytes = remoteBuild.TotalBytes;
        long availableBytes = LogicalDriver.GetAvailableFreeSpace(context.Operation.GameFileSystem.GameDirectory);

        if (totalBytes > availableBytes)
        {
            string title = $"磁盘空间不足，需要 {Converters.ToFileSizeString(totalBytes)}，剩余 {Converters.ToFileSizeString(availableBytes)}";
            context.Progress.Report(new GamePackageOperationReport.Reset(title));
            return;
        }

        int totalBlockCount = remoteBuild.TotalBlockCount;
        context.Progress.Report(new GamePackageOperationReport.Reset("正在安装游戏", totalBlockCount, totalBytes));

        await context.Operation.GameAssetsOperationService.InstallAssetsAsync(remoteBuild, context).ConfigureAwait(false);

        await context.Operation.GameAssetsOperationService.EnsureChannelSdkAsync(context).ConfigureAwait(false);

        await VerifyAndRepairCoreAsync(context, remoteBuild, totalBytes, totalBlockCount).ConfigureAwait(false);
    }

    private async ValueTask VerifyAndRepairAsync(GamePackageServiceContext context)
    {
        if (await DecodeManifestsAsync(context.Operation.LocalBranch, context).ConfigureAwait(false) is not { } localBuild)
        {
            context.Progress.Report(new GamePackageOperationReport.Reset("清单数据拉取失败"));
            return;
        }

        context.Progress.Report(new GamePackageOperationReport.Reset("正在验证游戏完整性", localBuild.TotalBlockCount, localBuild.TotalBytes));

        GamePackageIntegrityInfo info = await context.Operation.GameAssetsOperationService.VerifyGamePackageIntegrityAsync(localBuild, context).ConfigureAwait(false);

        if (info.NoConflict)
        {
            context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.OperationKind));
            return;
        }

        (int conflictBlocks, long conflictBytes) = info.GetConflictedBlockCountAndByteCount(context.Operation.GameChannelSDK);
        context.Progress.Report(new GamePackageOperationReport.Reset("正在修复游戏完整性", conflictBlocks, conflictBytes));

        await context.Operation.GameAssetsOperationService.RepairGamePackageAsync(info, context).ConfigureAwait(false);

        Directory.Delete(context.Operation.GameFileSystem.ChunksDirectory, true);
        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.OperationKind, true));
    }

    private async ValueTask UpdateAsync(GamePackageServiceContext context)
    {
        if (await DecodeManifestsAsync(context.Operation.LocalBranch, context).ConfigureAwait(false) is not { } localBuild ||
            await DecodeManifestsAsync(context.Operation.RemoteBranch, context).ConfigureAwait(false) is not { } remoteBuild)
        {
            context.Progress.Report(new GamePackageOperationReport.Reset("清单数据拉取失败"));
            return;
        }

        List<SophonAssetOperation> diffAssets = GetSophonDiffAssets(localBuild, remoteBuild).ToList();
        diffAssets.SortBy(a => a.Type);

        int totalBlocks = GetTotalBlocks(diffAssets);
        long totalBytes = GetTotalBytes(diffAssets);

        long availableBytes = LogicalDriver.GetAvailableFreeSpace(context.Operation.GameFileSystem.GameDirectory);
        if (totalBytes > availableBytes)
        {
            string title = $"磁盘空间不足，需要 {Converters.ToFileSizeString(totalBytes)}，剩余 {Converters.ToFileSizeString(availableBytes)}";
            context.Progress.Report(new GamePackageOperationReport.Reset(title));
            return;
        }

        context.Progress.Report(new GamePackageOperationReport.Reset("正在更新游戏", totalBlocks, totalBytes));

        await context.Operation.GameAssetsOperationService.UpdateDiffAssetsAsync(diffAssets, context).ConfigureAwait(false);
        await context.Operation.GameAssetsOperationService.EnsureChannelSdkAsync(context).ConfigureAwait(false);

        // Verify
        context.Progress.Report(new GamePackageOperationReport.Reset("正在验证游戏完整性", remoteBuild.Manifests.Sum(m => m.ManifestProto.Assets.Sum(a => a.AssetChunks.Count)), remoteBuild.TotalBytes));

        GamePackageIntegrityInfo info = await context.Operation.GameAssetsOperationService.VerifyGamePackageIntegrityAsync(remoteBuild, context).ConfigureAwait(false);

        if (info.NoConflict)
        {
            Directory.Delete(context.Operation.GameFileSystem.ChunksDirectory, true);
            context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.OperationKind));
            return;
        }

        (int conflictBlocks, long conflictBytes) = info.GetConflictedBlockCountAndByteCount(context.Operation.GameChannelSDK);
        context.Progress.Report(new GamePackageOperationReport.Reset("正在修复游戏完整性", conflictBlocks, conflictBytes));

        await context.Operation.GameAssetsOperationService.RepairGamePackageAsync(info, context).ConfigureAwait(false);

        Directory.Delete(context.Operation.GameFileSystem.ChunksDirectory, true);
        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.OperationKind));
    }

    private async ValueTask PredownloadAsync(GamePackageServiceContext context)
    {
        if (await DecodeManifestsAsync(context.Operation.LocalBranch, context).ConfigureAwait(false) is not { } localBuild ||
            await DecodeManifestsAsync(context.Operation.RemoteBranch, context).ConfigureAwait(false) is not { } remoteBuild)
        {
            context.Progress.Report(new GamePackageOperationReport.Reset("清单数据拉取失败"));
            return;
        }

        List<SophonAssetOperation> diffAssets = GetSophonDiffAssets(localBuild, remoteBuild).ToList();
        diffAssets.SortBy(a => a.Type);

        // Download
        int totalBlocks = GetTotalBlocks(diffAssets);
        long totalBytes = GetTotalBytes(diffAssets);

        long availableBytes = LogicalDriver.GetAvailableFreeSpace(context.Operation.GameFileSystem.GameDirectory);

        if (totalBytes > availableBytes)
        {
            string title = $"磁盘空间不足，需要 {Converters.ToFileSizeString(totalBytes)}，剩余 {Converters.ToFileSizeString(availableBytes)}";
            context.Progress.Report(new GamePackageOperationReport.Reset(title));
            return;
        }

        context.Progress.Report(new GamePackageOperationReport.Reset("正在预下载资源", totalBlocks, totalBytes));

        PredownloadStatus predownloadStatus = new(context.Operation.RemoteBranch.Tag, false, totalBlocks);
        using (FileStream predownloadStatusStream = File.Create(context.Operation.GameFileSystem.PredownloadStatusPath))
        {
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions, context.ParallelOptions.CancellationToken).ConfigureAwait(false);
        }

        await context.Operation.GameAssetsOperationService.PredownloadDiffAssetsAsync(diffAssets, context).ConfigureAwait(false);

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.OperationKind));

        using (FileStream predownloadStatusStream = File.Create(context.Operation.GameFileSystem.PredownloadStatusPath))
        {
            predownloadStatus.Finished = true;
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions, context.ParallelOptions.CancellationToken).ConfigureAwait(false);
        }
    }

    private async ValueTask<SophonDecodedBuild?> DecodeManifestsAsync(BranchWrapper branch, GamePackageServiceContext context)
    {
        CancellationToken token = context.ParallelOptions.CancellationToken;

        Response<SophonBuild> response;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ISophonClient client = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISophonClient>>()
                .Create(LaunchScheme.ExecutableIsOversea(context.Operation.GameFileSystem.GameFileName));

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
                "zh-cn" => !context.Operation.GameFileSystem.GameAudioSystem.Chinese,
                "en-us" => !context.Operation.GameFileSystem.GameAudioSystem.English,
                "ja-jp" => !context.Operation.GameFileSystem.GameAudioSystem.Japanese,
                "ko-kr" => !context.Operation.GameFileSystem.GameAudioSystem.Korean,
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

    private static IEnumerable<SophonAssetOperation> GetSophonDiffAssets(SophonDecodedBuild localDecodedBuild, SophonDecodedBuild remoteDecodedBuild)
    {
        foreach ((SophonDecodedManifest localManifest, SophonDecodedManifest remoteManifest) in localDecodedBuild.Manifests.Zip(remoteDecodedBuild.Manifests))
        {
            foreach (AssetProperty asset in remoteManifest.ManifestProto.Assets)
            {
                if (localManifest.ManifestProto.Assets.FirstOrDefault(a => a.AssetName.Equals(asset.AssetName, StringComparison.OrdinalIgnoreCase)) is not { } localAsset)
                {
                    yield return SophonAssetOperation.AddOrRepair(remoteManifest.UrlPrefix, asset);
                    continue;
                }

                if (localAsset.AssetHashMd5.Equals(asset.AssetHashMd5, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                List<SophonChunk> diffChunks = [];
                foreach (AssetChunk chunk in asset.AssetChunks)
                {
                    if (localAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5.Equals(chunk.ChunkDecompressedHashMd5, StringComparison.OrdinalIgnoreCase)) is null)
                    {
                        diffChunks.Add(new(remoteManifest.UrlPrefix, chunk));
                    }
                }

                yield return SophonAssetOperation.Modify(remoteManifest.UrlPrefix, localAsset, asset, diffChunks);
            }

            foreach (AssetProperty localAsset in localManifest.ManifestProto.Assets)
            {
                if (remoteManifest.ManifestProto.Assets.FirstOrDefault(a => a.AssetName.Equals(localAsset.AssetName, StringComparison.OrdinalIgnoreCase)) is null)
                {
                    yield return SophonAssetOperation.Delete(localAsset);
                }
            }
        }
    }

    private static async ValueTask VerifyAndRepairCoreAsync(GamePackageServiceContext context, SophonDecodedBuild build, long totalBytes, int totalBlockCount)
    {
        // Verify
        context.Progress.Report(new GamePackageOperationReport.Reset("正在验证游戏完整性", totalBlockCount, totalBytes));
        GamePackageIntegrityInfo info = await context.Operation.GameAssetsOperationService.VerifyGamePackageIntegrityAsync(build, context).ConfigureAwait(false);

        if (info.NoConflict)
        {
            Directory.Delete(context.Operation.GameFileSystem.ChunksDirectory, true);
            context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.OperationKind));
            return;
        }

        (int conflictedBlocks, long conflictedBytes) = info.GetConflictedBlockCountAndByteCount(context.Operation.GameChannelSDK);
        context.Progress.Report(new GamePackageOperationReport.Reset("正在修复游戏完整性", conflictedBlocks, conflictedBytes));

        await context.Operation.GameAssetsOperationService.RepairGamePackageAsync(info, context).ConfigureAwait(false);

        Directory.Delete(context.Operation.GameFileSystem.ChunksDirectory, true);
        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.OperationKind));
    }

    private static int GetTotalBlocks(List<SophonAssetOperation> diffAssets)
    {
        int totalBlocks = 0;
        foreach (ref readonly SophonAssetOperation diffAsset in CollectionsMarshal.AsSpan(diffAssets))
        {
            switch (diffAsset.Type)
            {
                case SophonAssetOperationType.AddOrRepair:
                    totalBlocks += diffAsset.NewAsset.AssetChunks.Count;
                    break;
                case SophonAssetOperationType.Modify:
                    totalBlocks += diffAsset.DiffChunks.Count;
                    break;
                default:
                    break;
            }
        }

        return totalBlocks;
    }

    private static long GetTotalBytes(List<SophonAssetOperation> diffAssets)
    {
        long totalBytes = 0;
        foreach (ref readonly SophonAssetOperation diffAsset in CollectionsMarshal.AsSpan(diffAssets))
        {
            switch (diffAsset.Type)
            {
                case SophonAssetOperationType.AddOrRepair:
                    totalBytes += diffAsset.NewAsset.AssetSize;
                    break;
                case SophonAssetOperationType.Modify:
                    totalBytes += diffAsset.DiffChunks.Sum(c => c.AssetChunk.ChunkSizeDecompressed);
                    break;
                default:
                    break;
            }
        }

        return totalBytes;
    }
}