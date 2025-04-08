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
using Snap.Hutao.Service.Game.Package.Advanced.PackageOperation;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.Web.Hoyolab.Downloader;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;
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
                    IGamePackageOperation operation = scope.ServiceProvider.GetRequiredKeyedService<IGamePackageOperation>(operationContext.Kind);

                    try
                    {
                        GamePackageServiceContext serviceContext = new(operationContext, info, progress, options, httpClient, limiter);
                        await operation.ExecuteAsync(serviceContext).ConfigureAwait(false);
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
            GamePackageOperationKind.Install or GamePackageOperationKind.ExtractExecutable => default,
            GamePackageOperationKind.Update or GamePackageOperationKind.Predownload or GamePackageOperationKind.ExtractBlocks => [.. GetDiffOperations(localBuild, remoteBuild).OrderBy(a => a.Kind)],
            _ => throw HutaoException.NotSupported(),
        };

        (long downloadTotalBytes, long totalBytes) = context.Kind switch
        {
            GamePackageOperationKind.Install or GamePackageOperationKind.ExtractExecutable => (remoteBuild.DownloadTotalBytes, remoteBuild.TotalBytes),
            GamePackageOperationKind.Update or GamePackageOperationKind.Predownload => (GetDownloadTotalBytes(diffAssets), GetTotalBytes(diffAssets)),
            GamePackageOperationKind.ExtractBlocks => (GetTotalBytes(diffAssets, true), GetTotalBytes(diffAssets, true)),
            _ => throw HutaoException.NotSupported(),
        };

        long actualTotalBytes = totalBytes + (1024L * 1024L * 1024L);
        long availableBytes = LogicalDriver.GetAvailableFreeSpace(context.EffectiveGameDirectory);

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
                GamePackageOperationKind.ExtractBlocks => "Start extracting game blocks?",
                GamePackageOperationKind.ExtractExecutable => "Start extracting game executable?",
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
            GamePackageOperationKind.Install or GamePackageOperationKind.ExtractExecutable => (remoteBuild.TotalChunks, remoteBuild.TotalChunks),
            GamePackageOperationKind.Update or GamePackageOperationKind.ExtractBlocks => (GetDownloadTotalBlocks(diffAssets), GetInstallTotalBlocks(diffAssets)),
            GamePackageOperationKind.Predownload => (GetDownloadTotalBlocks(diffAssets), 0),
            _ => throw HutaoException.NotSupported(),
        };

        return new(downloadTotalBlocks, installTotalBlocks, downloadTotalBytes, totalBytes, diffAssets);
    }
}