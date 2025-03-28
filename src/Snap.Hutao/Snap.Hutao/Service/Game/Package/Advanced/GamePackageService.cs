// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Threading.RateLimiting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.IO;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.Web.Hoyolab.Downloader;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Service.Game.Package.Advanced;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGamePackageService))]
[SuppressMessage("", "CA1001")]
[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1204")]
internal sealed partial class GamePackageService : IGamePackageService
{
    public const string HttpClientName = "SophonChunkRateLimited";

    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IMemoryStreamFactory memoryStreamFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<GamePackageService> logger;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IProgressFactory progressFactory;
    private readonly IServiceProvider serviceProvider;

    private CancellationTokenSource? operationCts;
    private TaskCompletionSource? operationTcs;

    public async ValueTask<bool> ExecuteOperationAsync(GamePackageOperationContext operationContext)
    {
        await CancelOperationAsync().ConfigureAwait(false);

        operationCts = new();
        operationTcs = new();

        ParallelOptions options = new()
        {
            CancellationToken = operationCts.Token,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();

            if (await EnsureAvailableFreeSpaceAndPrepareInformationAsync(operationContext).ConfigureAwait(false) is not { } info)
            {
                return false;
            }

            await taskContext.SwitchToMainThreadAsync();

            GamePackageOperationWindow window = scope.ServiceProvider.GetRequiredService<GamePackageOperationWindow>();
            IProgress<GamePackageOperationReport> progress = progressFactory.CreateForMainThread<GamePackageOperationReport>(window.HandleProgressUpdate);

            await taskContext.SwitchToBackgroundAsync();

            bool result;
            using (HttpClient httpClient = httpClientFactory.CreateClient(HttpClientName))
            {
                using (TokenBucketRateLimiter? limiter = StreamCopyRateLimiter.Create(serviceProvider))
                {
                    Func<GamePackageServiceContext, ValueTask> operation = operationContext.Kind switch
                    {
                        GamePackageOperationKind.Install => InstallAsync,
                        GamePackageOperationKind.Verify => VerifyAndRepairAsync,
                        GamePackageOperationKind.Update => UpdateAsync,
                        GamePackageOperationKind.Predownload => PredownloadAsync,
                        GamePackageOperationKind.ExtractBlk => ExtractAsync,
                        GamePackageOperationKind.ExtractExe => ExtractExeAsync,
                        _ => static context => ValueTask.FromException(HutaoException.NotSupported()),
                    };

                    try
                    {
                        GamePackageServiceContext serviceContext = new(operationContext, info, progress, options, httpClient, limiter);
                        await operation(serviceContext).ConfigureAwait(false);
                        result = true;
                    }
                    catch (OperationCanceledException)
                    {
                        logger.LogDebug("Operation canceled");
                        result = false;
                    }
                    catch (Exception ex)
                    {
                        logger.LogCritical(ex, "Unexpected exception while executing game package operation");
                        result = false;
                    }
                    finally
                    {
                        logger.LogDebug("Operation completed");
                        operationTcs.TrySetResult();
                    }
                }
            }

            await window.CloseTask.ConfigureAwait(false);
            return result;
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

    public async ValueTask<SophonDecodedBuild?> DecodeManifestsAsync(IGameFileSystem gameFileSystem, BranchWrapper? branch, CancellationToken token = default)
    {
        if (branch is null)
        {
            return default;
        }

        SophonBuild? build;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ISophonClient client = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISophonClient>>()
                .Create(gameFileSystem.IsOversea());

            Response<SophonBuild> response = await client.GetBuildAsync(branch, token).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out build))
            {
                return default;
            }
        }

        long downloadTotalBytes = 0L;
        long totalBytes = 0L;
        List<SophonDecodedManifest> decodedManifests = [];
        using (HttpClient httpClient = httpClientFactory.CreateClient(HttpClientName))
        {
            foreach (SophonManifest sophonManifest in build.Manifests)
            {
                bool exclude = sophonManifest.MatchingField switch
                {
                    "game" => false,
                    "zh-cn" => !gameFileSystem.Audio.Chinese,
                    "en-us" => !gameFileSystem.Audio.English,
                    "ja-jp" => !gameFileSystem.Audio.Japanese,
                    "ko-kr" => !gameFileSystem.Audio.Korean,
                    _ => true,
                };

                if (exclude)
                {
                    continue;
                }

                downloadTotalBytes += sophonManifest.Stats.CompressedSize;
                totalBytes += sophonManifest.Stats.UncompressedSize;
                string manifestDownloadUrl = $"{sophonManifest.ManifestDownload.UrlPrefix}/{sophonManifest.Manifest.Id}";

                using (Stream rawManifestStream = await httpClient.GetStreamAsync(manifestDownloadUrl, token).ConfigureAwait(false))
                {
                    using (ZstandardDecompressionStream decompressor = new(rawManifestStream))
                    {
                        using (MemoryStream inMemoryManifestStream = await memoryStreamFactory.GetStreamAsync(decompressor).ConfigureAwait(false))
                        {
                            string manifestMd5 = await Hash.ToHexStringAsync(HashAlgorithmName.MD5, inMemoryManifestStream, token).ConfigureAwait(false);
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

        return new(build.Tag, downloadTotalBytes, totalBytes, decodedManifests);
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

                ImmutableArray<SophonChunk>.Builder diffChunks = ImmutableArray.CreateBuilder<SophonChunk>();
                foreach (AssetChunk chunk in remoteAsset.AssetChunks)
                {
                    if (localAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5.Equals(chunk.ChunkDecompressedHashMd5, StringComparison.OrdinalIgnoreCase)) is null)
                    {
                        diffChunks.Add(new(remoteManifest.UrlPrefix, chunk));
                    }
                }

                yield return SophonAssetOperation.Modify(remoteManifest.UrlPrefix, localAsset, remoteAsset, diffChunks.ToImmutable());
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
        Debug.Assert(context.DuplicatedChunkNames.IsEmpty);
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

    private static async ValueTask PrivateVerifyAndRepairAsync(GamePackageServiceContext context, SophonDecodedBuild build, long totalBytes, int totalBlockCount)
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

        if (Directory.Exists(context.Operation.EffectiveChunksDirectory))
        {
            Directory.Delete(context.Operation.EffectiveChunksDirectory, true);
        }

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind, context.Operation.Kind is GamePackageOperationKind.Verify));
    }

    private static int GetUniqueTotalBlocks(ImmutableArray<SophonAssetOperation> assets)
    {
        HashSet<string> uniqueChunkNames = [];
        foreach (ref readonly SophonAssetOperation asset in assets.AsSpan())
        {
            switch (asset.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair:
                    foreach (ref readonly AssetChunk chunk in CollectionsMarshal.AsSpan(asset.NewAsset.AssetChunks.ToList()))
                    {
                        uniqueChunkNames.Add(chunk.ChunkName);
                    }

                    break;
                case SophonAssetOperationKind.Modify:
                    foreach (ref readonly SophonChunk diffChunk in asset.DiffChunks.AsSpan())
                    {
                        uniqueChunkNames.Add(diffChunk.AssetChunk.ChunkName);
                    }

                    break;
            }
        }

        return uniqueChunkNames.Count;
    }

    private static int GetDownloadTotalBlocks(ImmutableArray<SophonAssetOperation> assets)
    {
        int totalBlocks = 0;
        foreach (ref readonly SophonAssetOperation asset in assets.AsSpan())
        {
            switch (asset.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair:
                    totalBlocks += asset.NewAsset.AssetChunks.Count;
                    break;
                case SophonAssetOperationKind.Modify:
                    totalBlocks += asset.DiffChunks.Length;
                    break;
            }
        }

        return totalBlocks;
    }

    private static int GetInstallTotalBlocks(ImmutableArray<SophonAssetOperation> assets)
    {
        int totalBlocks = 0;
        foreach (ref readonly SophonAssetOperation asset in assets.AsSpan())
        {
            switch (asset.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair or SophonAssetOperationKind.Modify:
                    totalBlocks += asset.NewAsset.AssetChunks.Count;
                    break;
            }
        }

        return totalBlocks;
    }

    private static long GetDownloadTotalBytes(ImmutableArray<SophonAssetOperation> assets)
    {
        long downloadTotalBytes = 0;
        foreach (ref readonly SophonAssetOperation diffAsset in assets.AsSpan())
        {
            switch (diffAsset.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair:
                    downloadTotalBytes += diffAsset.NewAsset.AssetChunks.Sum(c => c.ChunkSize);
                    break;
                case SophonAssetOperationKind.Modify:
                    downloadTotalBytes += diffAsset.DiffChunks.Sum(c => c.AssetChunk.ChunkSize);
                    break;
            }
        }

        return downloadTotalBytes;
    }

    private static long GetTotalBytes(ImmutableArray<SophonAssetOperation> assets, bool isExtractBlk = false)
    {
        long totalBytes = 0;
        foreach (ref readonly SophonAssetOperation diffAsset in assets.AsSpan())
        {
            switch (diffAsset.Kind)
            {
                case SophonAssetOperationKind.AddOrRepair:
                    totalBytes += diffAsset.NewAsset.AssetSize;
                    break;
                case SophonAssetOperationKind.Modify:
                    totalBytes += isExtractBlk ? diffAsset.NewAsset.AssetSize : diffAsset.DiffChunks.Sum(c => c.AssetChunk.ChunkSizeDecompressed);
                    break;
            }
        }

        return totalBytes;
    }

    private static async ValueTask VerifyAndRepairAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild localBuild = context.Operation.LocalBuild;
        await PrivateVerifyAndRepairAsync(context, localBuild, localBuild.TotalBytes, localBuild.TotalChunks).ConfigureAwait(false);
    }

    private async ValueTask<GamePackageOperationInfo?> EnsureAvailableFreeSpaceAndPrepareInformationAsync(GamePackageOperationContext context)
    {
        SophonDecodedBuild localBuild = context.LocalBuild;
        SophonDecodedBuild remoteBuild = context.RemoteBuild;

        if (context.Kind is GamePackageOperationKind.Verify)
        {
            return new(0, localBuild.TotalChunks, 0, localBuild.TotalBytes, default);
        }

        ImmutableArray<SophonAssetOperation> diffAssets = context.Kind switch
        {
            GamePackageOperationKind.Install or GamePackageOperationKind.ExtractExe => default,
            GamePackageOperationKind.Update or GamePackageOperationKind.Predownload or GamePackageOperationKind.ExtractBlk => [.. GetDiffOperations(localBuild, remoteBuild).OrderBy(a => a.Kind)],
            _ => throw HutaoException.NotSupported(),
        };

        (long downloadTotalBytes, long totalBytes) = context.Kind switch
        {
            GamePackageOperationKind.Install or GamePackageOperationKind.ExtractExe => (remoteBuild.DownloadTotalBytes, remoteBuild.TotalBytes),
            GamePackageOperationKind.Update or GamePackageOperationKind.Predownload => (GetDownloadTotalBytes(diffAssets), GetTotalBytes(diffAssets)),
            GamePackageOperationKind.ExtractBlk => (GetTotalBytes(diffAssets, true), GetTotalBytes(diffAssets, true)),
            _ => throw HutaoException.NotSupported(),
        };

        long actualTotalBytes = totalBytes + (1024L * 1024L * 1024L);
        long availableBytes = LogicalDriver.GetAvailableFreeSpace(context.ExtractOrGameDirectory);

        string downloadTotalBytesFormatted = Converters.ToFileSizeString(downloadTotalBytes);
        string totalBytesFormatted = Converters.ToFileSizeString(actualTotalBytes);
        string availableBytesFormatted = Converters.ToFileSizeString(availableBytes);

        string title;
        bool hasAvailableFreeSpace = actualTotalBytes <= availableBytes;
        if (!hasAvailableFreeSpace)
        {
            title = SH.ServiceGamePackageAdvancedDriverNoAvailableFreeSpace;
        }
        else
        {
            title = context.Kind switch
            {
                GamePackageOperationKind.Install => SH.ServiceGamePackageAdvancedConfirmStartInstallTitle,
                GamePackageOperationKind.Update => SH.ServiceGamePackageAdvancedConfirmStartUpdateTitle,
                GamePackageOperationKind.Predownload => SH.ServiceGamePackageAdvancedConfirmStartPredownloadTitle,
                GamePackageOperationKind.ExtractBlk => "Start extracting game blocks?",
                GamePackageOperationKind.ExtractExe => "Start extracting game executable?",
                _ => throw HutaoException.NotSupported(),
            };
        }

        string message = SH.FormatServiceGamePackageAdvancedConfirmMessage(downloadTotalBytesFormatted, totalBytesFormatted, availableBytesFormatted);

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(title, message, ContentDialogButton.Primary, hasAvailableFreeSpace)
            .ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return default;
        }

        (int downloadTotalBlocks, int installTotalBlocks) = context.Kind switch
        {
            GamePackageOperationKind.Install or GamePackageOperationKind.ExtractExe => (remoteBuild.TotalChunks, remoteBuild.TotalChunks),
            GamePackageOperationKind.Update or GamePackageOperationKind.ExtractBlk => (GetDownloadTotalBlocks(diffAssets), GetInstallTotalBlocks(diffAssets)),
            GamePackageOperationKind.Predownload => (GetDownloadTotalBlocks(diffAssets), 0),
            _ => throw HutaoException.NotSupported(),
        };

        return new(downloadTotalBlocks, installTotalBlocks, downloadTotalBytes, totalBytes, diffAssets);
    }

    private static async ValueTask InstallAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild remoteBuild = context.Operation.RemoteBuild;
        int totalChunksCount = remoteBuild.TotalChunks;
        long totalBytes = remoteBuild.TotalBytes;

        InitializeDuplicatedChunkNames(context, remoteBuild.Manifests.SelectMany(m => m.ManifestProto.Assets.SelectMany(a => a.AssetChunks)));

        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedInstalling, totalChunksCount, totalBytes));

        await context.Operation.Asset.InstallAssetsAsync(context, remoteBuild).ConfigureAwait(false);
        await context.Operation.Asset.EnsureChannelSdkAsync(context).ConfigureAwait(false);

        await PrivateVerifyAndRepairAsync(context, remoteBuild, totalBytes, totalChunksCount).ConfigureAwait(false);

        if (Directory.Exists(context.Operation.EffectiveChunksDirectory))
        {
            Directory.Delete(context.Operation.EffectiveChunksDirectory, true);
        }
    }

    private static async ValueTask UpdateAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild remoteBuild = context.Operation.RemoteBuild;
        ImmutableArray<SophonAssetOperation> diffAssets = context.Information.DiffAssetOperations;
        int downloadTotalChunks = context.Information.DownloadTotalChunks;
        int installTotalChunks = context.Information.InstallTotalChunks;
        long totalBytes = context.Information.InstallTotalBytes;

        InitializeDuplicatedChunkNames(context, diffAssets.SelectMany(a => a.DiffChunks.Select(c => c.AssetChunk)));

        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedUpdating, downloadTotalChunks, installTotalChunks, totalBytes));

        await context.Operation.Asset.UpdateDiffAssetsAsync(context, diffAssets).ConfigureAwait(false);
        await context.Operation.Asset.EnsureChannelSdkAsync(context).ConfigureAwait(false);

        await PrivateVerifyAndRepairAsync(context, remoteBuild, remoteBuild.TotalBytes, remoteBuild.TotalChunks).ConfigureAwait(false);

        context.Operation.GameFileSystem.TryUpdateConfigurationFile(remoteBuild.Tag);

        if (Directory.Exists(context.Operation.EffectiveChunksDirectory))
        {
            Directory.Delete(context.Operation.EffectiveChunksDirectory, true);
        }
    }

    private async ValueTask PredownloadAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild remoteBuild = context.Operation.RemoteBuild;
        ImmutableArray<SophonAssetOperation> diffAssets = context.Information.DiffAssetOperations;
        int totalBlocks = context.Information.DownloadTotalChunks;
        long totalBytes = context.Information.InstallTotalBytes;

        int uniqueTotalBlocks = GetUniqueTotalBlocks(diffAssets);

        context.Progress.Report(new GamePackageOperationReport.Reset(SH.ServiceGamePackageAdvancedPredownloading, totalBlocks, 0, totalBytes));

        if (!Directory.Exists(context.Operation.GameFileSystem.GetChunksDirectory()))
        {
            Directory.CreateDirectory(context.Operation.GameFileSystem.GetChunksDirectory());
        }

        PredownloadStatus predownloadStatus = new(remoteBuild.Tag, false, uniqueTotalBlocks);
        using (FileStream predownloadStatusStream = File.Create(context.Operation.GameFileSystem.GetPredownloadStatusPath()))
        {
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions).ConfigureAwait(false);
        }

        await context.Operation.Asset.PredownloadDiffAssetsAsync(context, diffAssets).ConfigureAwait(false);

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind));

        using (FileStream predownloadStatusStream = File.Create(context.Operation.GameFileSystem.GetPredownloadStatusPath()))
        {
            predownloadStatus.Finished = true;
            await JsonSerializer.SerializeAsync(predownloadStatusStream, predownloadStatus, jsonOptions).ConfigureAwait(false);
        }
    }

    #region Dev Only

    private static async ValueTask ExtractAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild localBuild = context.Operation.LocalBuild;
        ImmutableArray<SophonAssetOperation> diffAssets = context.Information.DiffAssetOperations;
        int downloadTotalChunks = context.Information.DownloadTotalChunks;
        int installTotalChunks = context.Information.InstallTotalChunks;
        long totalBytes = context.Information.InstallTotalBytes;

        InitializeDuplicatedChunkNames(context, diffAssets.SelectMany(a => a.DiffChunks.Select(c => c.AssetChunk)));

        context.Progress.Report(new GamePackageOperationReport.Reset("Copying", 0, localBuild.TotalChunks, localBuild.TotalBytes));
        List<string> usefulChunks = diffAssets
            .Where(ao => ao.Kind is SophonAssetOperationKind.Modify)
            .Select(ao => Path.GetFileName(ao.OldAsset.AssetName))
            .ToList();
        string oldBlksDirectory = Path.Combine(context.Operation.GameFileSystem.GetDataDirectory(), @"StreamingAssets\AssetBundles\blocks");
        foreach (string file in Directory.GetFiles(oldBlksDirectory, "*.blk", SearchOption.AllDirectories))
        {
            string fileName = Path.GetFileName(file);
            if (!usefulChunks.Contains(fileName, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            string newFilePath = Path.Combine(context.Operation.ExtractOrGameDirectory, fileName);
            File.Copy(file, newFilePath, true);
            AssetProperty asset = localBuild.Manifests.Single().ManifestProto.Assets.Single(a => a.AssetName.Contains(fileName, StringComparison.OrdinalIgnoreCase));
            context.Progress.Report(new GamePackageOperationReport.Install(asset.AssetSize, asset.AssetChunks.Count));
        }

        context.Progress.Report(new GamePackageOperationReport.Reset("Extracting", downloadTotalChunks, installTotalChunks, totalBytes));
        await context.Operation.Asset.UpdateDiffAssetsAsync(context, diffAssets).ConfigureAwait(false);

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind));
    }

    private static async ValueTask ExtractExeAsync(GamePackageServiceContext context)
    {
        SophonDecodedBuild remoteBuild = context.Operation.RemoteBuild;
        int totalChunks = remoteBuild.TotalChunks;
        long totalBytes = remoteBuild.TotalBytes;

        InitializeDuplicatedChunkNames(context, remoteBuild.Manifests.Single().ManifestProto.Assets.SelectMany(a => a.AssetChunks));

        context.Progress.Report(new GamePackageOperationReport.Reset("Extracting", totalChunks, totalBytes));
        await context.Operation.Asset.InstallAssetsAsync(context, remoteBuild).ConfigureAwait(false);

        context.Progress.Report(new GamePackageOperationReport.Finish(context.Operation.Kind));
    }

    #endregion
}