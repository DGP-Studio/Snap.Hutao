// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.Threading.RateLimiting;
using Snap.Hutao.Factory.IO;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Package.Advanced.Model;
using Snap.Hutao.Service.Game.Package.Advanced.PackageOperation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.Web.Hoyolab.Downloader;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Service.Game.Package.Advanced;

[GeneratedConstructor]
[Service(ServiceLifetime.Singleton, typeof(IGamePackageService))]
[SuppressMessage("", "CA1001")]
[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1204")]
internal sealed partial class GamePackageService : IGamePackageService
{
    public const string HttpClientName = "SophonChunkRateLimited";

    private readonly GamePackageServiceOperationInformationTraits informationTraits;
    private readonly IMemoryStreamFactory memoryStreamFactory;
    private readonly IHttpClientFactory httpClientFactory;
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

            if (await informationTraits.EnsureAvailableFreeSpaceAndPrepareAsync(operationContext).ConfigureAwait(false) is not { } info)
            {
                return false;
            }

            await taskContext.SwitchToMainThreadAsync();

            // TODO: Move window creation out of this service.
            GamePackageOperationWindow window = scope.ServiceProvider.GetRequiredService<GamePackageOperationWindow>();
            IProgress<GamePackageOperationReport> progress = scope.ServiceProvider
                .GetRequiredService<IProgressFactory>()
                .CreateForMainThread<GamePackageOperationReport>(window.HandleProgressUpdate);

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
                        if (operationCts is { IsCancellationRequested: true })
                        {
                            serviceProvider.GetRequiredService<IMessenger>().Send(InfoBarMessage.Warning(SH.ServicePackageAdvancedExecuteOperationCanceledTitle));
                            await window.CloseTask.ConfigureAwait(false);
                            return false;
                        }

                        throw;
                    }
                    catch (Exception ex)
                    {
                        if (ex is HttpRequestException httpRequestException)
                        {
                            if (HttpRequestExceptionHandling.HttpRequestExceptionToNetworkError(httpRequestException) is Web.NetworkError.NULL)
                            {
                                SentrySdk.CaptureException(ex);
                            }
                        }

                        serviceProvider.GetRequiredService<IMessenger>().Send(InfoBarMessage.Error(SH.ServicePackageAdvancedExecuteOperationFailedTitle, ex));
                        result = false;
                    }
                    finally
                    {
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

    public async ValueTask<SophonDecodedBuild?> DecodeManifestsAsync(IGameFileSystemView gameFileSystem, BranchWrapper? branch, CancellationToken token = default)
    {
        if (branch is null)
        {
            return default;
        }

        SophonBuild? build;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            Response<SophonBuild> response = await scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISophonClient>>()
                .Create(gameFileSystem.IsExecutableOversea())
                .GetBuildAsync(branch, token)
                .ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out build))
            {
                return default;
            }
        }

        return await DecodeManifestsAsync(gameFileSystem, build, token).ConfigureAwait(false);
    }

    public async ValueTask<SophonDecodedBuild?> DecodeManifestsAsync(IGameFileSystemView gameFileSystem, SophonBuild? build, CancellationToken token = default)
    {
        if (build is null)
        {
            return default;
        }

        long downloadTotalBytes = 0L;
        long totalBytes = 0L;
        ImmutableArray<SophonDecodedManifest>.Builder decodedManifests = ImmutableArray.CreateBuilder<SophonDecodedManifest>();
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

                string manifestDownloadUrl = $"{sophonManifest.ManifestDownload.UrlPrefix}/{sophonManifest.Manifest.Id}?{sophonManifest.ManifestDownload.UrlSuffix}";
                try
                {
                    using (Stream rawManifestStream = await httpClient.GetStreamAsync(manifestDownloadUrl, token).ConfigureAwait(false))
                    {
                        using (ZstandardDecompressStream decompressor = new(rawManifestStream))
                        {
                            using (MemoryStream inMemoryManifestStream = await memoryStreamFactory.GetStreamAsync(decompressor).ConfigureAwait(false))
                            {
                                string manifestMd5 = await Hash.ToHexStringAsync(HashAlgorithmName.MD5, inMemoryManifestStream, token).ConfigureAwait(false);
                                if (manifestMd5.Equals(sophonManifest.Manifest.Checksum, StringComparison.OrdinalIgnoreCase))
                                {
                                    inMemoryManifestStream.Position = 0;
                                    decodedManifests.Add(new(sophonManifest.ChunkDownload.UrlPrefix, sophonManifest.ChunkDownload.UrlSuffix, SophonManifestProto.Parser.ParseFrom(inMemoryManifestStream)));
                                }
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    return default;
                }
                catch (Exception ex)
                {
                    StringBuilder messageBuilder = new();
                    if (HttpRequestExceptionHandling.FormatException(messageBuilder, ex, manifestDownloadUrl))
                    {
                        serviceProvider.GetRequiredService<IMessenger>().Send(InfoBarMessage.Error(messageBuilder.ToString(), ex));
                    }
                    else
                    {
                        // IOException: The request was aborted.
                        // + IOException: Unable to read data from the transport connection: 远程主机强迫关闭了一个现有的连接。.
                        //   + SocketException | ConnectionReset: 远程主机强迫关闭了一个现有的连接。
                        SentrySdk.CaptureException(ex);
                    }

                    return default;
                }
            }
        }

        return new(build.Tag, downloadTotalBytes, totalBytes, decodedManifests.ToImmutable());
    }

    public async ValueTask<SophonDecodedPatchBuild?> DecodeDiffManifestsAsync(IGameFileSystemView gameFileSystem, BranchWrapper? branch, CancellationToken token = default)
    {
        if (branch is null)
        {
            return default;
        }

        if (!gameFileSystem.TryGetGameVersion(out string? version))
        {
            return default;
        }

        if (!branch.DiffTags.Contains(version))
        {
            return default;
        }

        SophonPatchBuild? build;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            Response<SophonPatchBuild> response = await scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISophonClient>>()
                .Create(gameFileSystem.IsExecutableOversea())
                .GetPatchBuildAsync(branch, token)
                .ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out build))
            {
                return default;
            }
        }

        return await DecodeDiffManifestsAsync(gameFileSystem, build, token).ConfigureAwait(false);
    }

    public async ValueTask<SophonDecodedPatchBuild?> DecodeDiffManifestsAsync(IGameFileSystemView gameFileSystem, SophonPatchBuild? patchBuild, CancellationToken token = default)
    {
        if (patchBuild is null)
        {
            return default;
        }

        if (!gameFileSystem.TryGetGameVersion(out string? version))
        {
            return default;
        }

        if (patchBuild.Manifests.Any(m => !m.Stats.ContainsKey(version)))
        {
            return default;
        }

        long downloadTotalBytes = 0L;
        long downloadFileCount = 0L;
        long totalBytes = 0L;
        long installFileCount = 0L;
        ImmutableArray<SophonDecodedPatchManifest>.Builder decodedPatchManifests = ImmutableArray.CreateBuilder<SophonDecodedPatchManifest>();
        using (HttpClient httpClient = httpClientFactory.CreateClient(HttpClientName))
        {
            foreach (SophonPatchManifest sophonPatchManifest in patchBuild.Manifests)
            {
                bool exclude = sophonPatchManifest.MatchingField switch
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

                ManifestStats stats = sophonPatchManifest.Stats[version];
                downloadTotalBytes += stats.CompressedSize;
                downloadFileCount += stats.ChunkCount;
                totalBytes += stats.UncompressedSize;
                installFileCount += stats.FileCount;

                string manifestDownloadUrl = $"{sophonPatchManifest.ManifestDownload.UrlPrefix}/{sophonPatchManifest.Manifest.Id}?{sophonPatchManifest.ManifestDownload.UrlSuffix}";
                try
                {
                    using (Stream rawManifestStream = await httpClient.GetStreamAsync(manifestDownloadUrl, token).ConfigureAwait(false))
                    {
                        using (ZstandardDecompressStream decompressor = new(rawManifestStream))
                        {
                            using (MemoryStream inMemoryManifestStream = await memoryStreamFactory.GetStreamAsync(decompressor).ConfigureAwait(false))
                            {
                                string manifestMd5 = await Hash.ToHexStringAsync(HashAlgorithmName.MD5, inMemoryManifestStream, token).ConfigureAwait(false);
                                if (manifestMd5.Equals(sophonPatchManifest.Manifest.Checksum, StringComparison.OrdinalIgnoreCase))
                                {
                                    inMemoryManifestStream.Position = 0;
                                    decodedPatchManifests.Add(new(version, patchBuild.Tag, sophonPatchManifest.DiffDownload.UrlPrefix, sophonPatchManifest.DiffDownload.UrlSuffix, PatchManifest.Parser.ParseFrom(inMemoryManifestStream)));
                                }
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    return default;
                }
            }
        }

        return new(version, patchBuild.Tag, downloadTotalBytes, downloadFileCount, totalBytes, installFileCount, decodedPatchManifests.ToImmutable());
    }
}