// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ComponentModel;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Threading.RateLimiting;
using Snap.Hutao.Factory.IO;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.Web.Hoyolab.Downloader;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using Snap.Hutao.Web.Response;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Service.Game.Package.Advanced;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGamePackageService))]
[SuppressMessage("", "CA1001")]
internal sealed partial class GamePackageService : IGamePackageService
{
    public const string HttpClientName = "SophonChunkRateLimited";

    private readonly IMemoryStreamFactory memoryStreamFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<GamePackageService> logger;
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
        IProgress<GamePackageOperationReport> progress = progressFactory.CreateForMainThread<GamePackageOperationReport>((window.DataContext).HandleProgressUpdate);

        await taskContext.SwitchToBackgroundAsync();

        using (HttpClient httpClient = httpClientFactory.CreateClient(HttpClientName))
        {
            using (NotifyPropertyChangedBox<AppOptions, TokenBucketRateLimiter?> limiterBox = StreamCopyRateLimiter.Create(serviceProvider))
            {
                GamePackageServiceContext serviceContext = new(operationContext, progress, options, httpClient, limiterBox);

                Func<GamePackageServiceContext, ValueTask> operation = operationContext.Kind switch
                {
                    GamePackageOperationKind.Install => InstallAsync,
                    GamePackageOperationKind.Verify => VerifyAndRepairAsync,
                    GamePackageOperationKind.Update => UpdateAsync,
                    GamePackageOperationKind.Predownload => PredownloadAsync,
                    _ => context => ValueTask.FromException(HutaoException.NotSupported()),
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
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Unexpected exception while exeuting game package operation");
                    return false;
                }
                finally
                {
                    logger.LogDebug("Operation completed");
                    operationTcs.TrySetResult();
                }
            }
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

    private static IEnumerable<SophonAssetOperation> GetDiffOperations(SophonDecodedBuild localDecodedBuild, SophonDecodedBuild remoteDecodedBuild)
    {
        foreach ((SophonDecodedManifest localManifest, SophonDecodedManifest remoteManifest) in localDecodedBuild.Manifests.Zip(remoteDecodedBuild.Manifests))
        {
            foreach (AssetProperty remoteAsset in remoteManifest.ManifestProto.Assets)
            {
                if (localManifest.ManifestProto.Assets.FirstOrDefault(localAsset => localAsset.AssetName.Equals(remoteAsset.AssetName, StringComparison.OrdinalIgnoreCase)) is not { } localAsset)
                {
                    yield return SophonAssetOperation.AddOrRepair(remoteManifest.UrlPrefix, remoteAsset);
                    continue;
                }

                if (localAsset.AssetHashMd5.Equals(remoteAsset.AssetHashMd5, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                List<SophonChunk> diffChunks = [];
                foreach (AssetChunk chunk in remoteAsset.AssetChunks)
                {
                    if (localAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5.Equals(chunk.ChunkDecompressedHashMd5, StringComparison.OrdinalIgnoreCase)) is null)
                    {
                        diffChunks.Add(new(remoteManifest.UrlPrefix, chunk));
                    }
                }

                yield return SophonAssetOperation.Modify(remoteManifest.UrlPrefix, localAsset, remoteAsset, diffChunks);
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

    private static void InitializeDuplicatedChunkNames(GamePackageServiceContext context, IEnumerable<AssetChunk> chunks)
    {
        Debug.Assert(context.DuplicatedChunkNames.Count is 0);
        IEnumerable<string> names = chunks
            .GroupBy(chunk => chunk.ChunkName)
            .Where(group => group.Skip(1).Any())
            .Select(group => group.Key)
            .Distinct();

        foreach (string name in names)
        {
            context.DuplicatedChunkNames.TryAdd(name, default);
        }
    }

    private static async ValueTask VerifyAndRepairCoreAsync(GamePackageServiceContext context, SophonDecodedBuild build, long totalBytes, int totalBlockCount)
    {
        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedVerifyingIntegrity, 0, totalBlockCount, totalBytes));
        GamePackageIntegrityInfo info = await context.Operation.Asset.VerifyGamePackageIntegrityAsync(context, build).ConfigureAwait(false);

        if (info.NoConflict)
        {
            context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind));
            return;
        }

        (int conflictedBlocks, long conflictedBytes) = info.GetConflictedBlockCountAndByteCount(context.Operation.GameChannelSDK);
        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedRepairing, conflictedBlocks, conflictedBytes));

        await context.Operation.Asset.RepairGamePackageAsync(context, info).ConfigureAwait(false);

        if (Directory.Exists(context.Operation.ProxiedChunksDirectory))
        {
            Directory.Delete(context.Operation.ProxiedChunksDirectory, true);
        }

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind, context.Operation.Kind is GamePackageOperationKind.Verify));
    }

    private static int GetTotalBlocks(List<SophonAssetOperation> assets)
    {
        int totalBlocks = 0;
        foreach (ref readonly SophonAssetOperation asset in CollectionsMarshal.AsSpan(assets))
        {
            switch (asset.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair:
                    totalBlocks += asset.NewAsset.AssetChunks.Count;
                    break;
                case SophonAssetOperationKind.Modify:
                    totalBlocks += asset.DiffChunks.Count;
                    break;
                default:
                    break;
            }
        }

        return totalBlocks;
    }

    private static long GetTotalBytes(List<SophonAssetOperation> assets)
    {
        long totalBytes = 0;
        foreach (ref readonly SophonAssetOperation diffAsset in CollectionsMarshal.AsSpan(assets))
        {
            switch (diffAsset.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair:
                    totalBytes += diffAsset.NewAsset.AssetSize;
                    break;
                case SophonAssetOperationKind.Modify:
                    totalBytes += diffAsset.DiffChunks.Sum(c => c.AssetChunk.ChunkSizeDecompressed);
                    break;
            }
        }

        return totalBytes;
    }

    private async ValueTask VerifyAndRepairAsync(GamePackageServiceContext context)
    {
        if (await DecodeManifestsAsync(context, context.Operation.LocalBranch).ConfigureAwait(false) is not { } localBuild)
        {
            context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedDecodeManifestFailed));
            return;
        }

        await VerifyAndRepairCoreAsync(context, localBuild, localBuild.TotalBytes, localBuild.TotalChunks).ConfigureAwait(false);
    }

    private async ValueTask InstallAsync(GamePackageServiceContext context)
    {
        if (await DecodeManifestsAsync(context, context.Operation.RemoteBranch).ConfigureAwait(false) is not { } remoteBuild)
        {
            context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedDecodeManifestFailed));
            return;
        }

        long totalBytes = remoteBuild.TotalBytes;
        if (!context.EnsureAvailableFreeSpace(totalBytes))
        {
            return;
        }

        InitializeDuplicatedChunkNames(context, remoteBuild.Manifests.SelectMany(m => m.ManifestProto.Assets.SelectMany(a => a.AssetChunks)));

        int totalBlockCount = remoteBuild.TotalChunks;
        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedInstalling, totalBlockCount, totalBytes));

        await context.Operation.Asset.InstallAssetsAsync(context, remoteBuild).ConfigureAwait(false);
        await context.Operation.Asset.EnsureChannelSdkAsync(context).ConfigureAwait(false);

        await VerifyAndRepairCoreAsync(context, remoteBuild, totalBytes, totalBlockCount).ConfigureAwait(false);

        if (Directory.Exists(context.Operation.ProxiedChunksDirectory))
        {
            Directory.Delete(context.Operation.ProxiedChunksDirectory, true);
        }
    }

    private async ValueTask UpdateAsync(GamePackageServiceContext context)
    {
        if (await DecodeManifestsAsync(context, context.Operation.LocalBranch).ConfigureAwait(false) is not { } localBuild ||
            await DecodeManifestsAsync(context, context.Operation.RemoteBranch).ConfigureAwait(false) is not { } remoteBuild)
        {
            context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedDecodeManifestFailed));
            return;
        }

        List<SophonAssetOperation> diffAssets = GetDiffOperations(localBuild, remoteBuild).ToList();
        diffAssets.SortBy(a => a.Kind);

        int totalBlocks = GetTotalBlocks(diffAssets);
        long totalBytes = GetTotalBytes(diffAssets);

        if (!context.EnsureAvailableFreeSpace(totalBytes))
        {
            return;
        }

        InitializeDuplicatedChunkNames(context, diffAssets.SelectMany(a => a.DiffChunks.Select(c => c.AssetChunk)));

        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedUpdating, totalBlocks, totalBytes));

        await context.Operation.Asset.UpdateDiffAssetsAsync(context, diffAssets).ConfigureAwait(false);
        await context.Operation.Asset.EnsureChannelSdkAsync(context).ConfigureAwait(false);

        await VerifyAndRepairCoreAsync(context, remoteBuild, remoteBuild.TotalBytes, remoteBuild.TotalChunks).ConfigureAwait(false);

        context.Operation.GameFileSystem.UpdateConfigurationFile(context.Operation.RemoteBranch.Tag);

        if (Directory.Exists(context.Operation.ProxiedChunksDirectory))
        {
            Directory.Delete(context.Operation.ProxiedChunksDirectory, true);
        }
    }

    private async ValueTask PredownloadAsync(GamePackageServiceContext context)
    {
        if (await DecodeManifestsAsync(context, context.Operation.LocalBranch).ConfigureAwait(false) is not { } localBuild ||
            await DecodeManifestsAsync(context, context.Operation.RemoteBranch).ConfigureAwait(false) is not { } remoteBuild)
        {
            context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedDecodeManifestFailed));
            return;
        }

        List<SophonAssetOperation> diffAssets = GetDiffOperations(localBuild, remoteBuild).ToList();
        diffAssets.SortBy(a => a.Kind);

        int totalBlocks = GetTotalBlocks(diffAssets);
        long totalBytes = GetTotalBytes(diffAssets);

        if (!context.EnsureAvailableFreeSpace(totalBytes))
        {
            return;
        }

        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedPredownloading, totalBlocks, totalBytes));

        PredownloadStatus predownloadStatus = new(context.Operation.RemoteBranch.Tag, false, totalBlocks);
        using (FileStream predownloadStatusStream = File.Create(context.Operation.GameFileSystem.PredownloadStatusPath))
        {
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions).ConfigureAwait(false);
        }

        await context.Operation.Asset.PredownloadDiffAssetsAsync(context, diffAssets).ConfigureAwait(false);

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind));

        using (FileStream predownloadStatusStream = File.Create(context.Operation.GameFileSystem.PredownloadStatusPath))
        {
            predownloadStatus.Finished = true;
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions).ConfigureAwait(false);
        }
    }

    private async ValueTask<SophonDecodedBuild?> DecodeManifestsAsync(GamePackageServiceContext context, BranchWrapper branch)
    {
        CancellationToken token = context.CancellationToken;

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

            using (Stream rawManifestStream = await context.HttpClient.GetStreamAsync(manifestDownloadUrl, token).ConfigureAwait(false))
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

        return new(totalBytes, decodedManifests);
    }
}