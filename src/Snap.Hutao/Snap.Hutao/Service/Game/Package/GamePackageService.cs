// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using Snap.Hutao.Web.Response;
using System.Buffers;
using System.IO;
using System.Net.Http;
using ZstdNet;

namespace Snap.Hutao.Service.Game.Package;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGamePackageService))]
[SuppressMessage("", "CA1001")]
internal sealed partial class GamePackageService : IGamePackageService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IProgressFactory progressFactory;
    private readonly LaunchOptions launchOptions;
    private readonly ITaskContext taskContext;

    private CancellationTokenSource? workingTokenSource;

    public async ValueTask StartOperationAsync(GamePackageOperationContext context)
    {
        await CancelOperationAsync().ConfigureAwait(false);

        workingTokenSource = new();
        GamePackageOperationWindow window = Ioc.Default.GetRequiredService<GamePackageOperationWindow>();
        switch (context.State)
        {
            case GamePackageOperationState.Verify:
                await VerifyAsync(window, context, workingTokenSource.Token).ConfigureAwait(false);
                break;
            case GamePackageOperationState.Update:
                await UpdateAsync(window, context, workingTokenSource.Token).ConfigureAwait(false);
                break;
            case GamePackageOperationState.Predownload:
                await PredownloadAsync(window, context, workingTokenSource.Token).ConfigureAwait(false);
                break;
            default:
                break;
        }
    }

    public async ValueTask CancelOperationAsync()
    {
        if (workingTokenSource is null)
        {
            return;
        }

        await workingTokenSource.CancelAsync().ConfigureAwait(false);
        workingTokenSource.Dispose();
        workingTokenSource = null;
    }

    private async ValueTask VerifyAsync(GamePackageOperationWindow window, GamePackageOperationContext context, CancellationToken token = default)
    {
        try
        {
            GamePackageOperationViewModel viewModel = new("正在验证游戏完整性");
            window.InitializeDataContext(viewModel);

            IProgress<SophonChunkDownloadStatus> progress = progressFactory.CreateForMainThread<SophonChunkDownloadStatus>(viewModel.Report);

            Response<SophonBuild> sophonBuildResp;
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                ISophonClient client = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<ISophonClient>>()
                    .Create(context.IsOversea);

                sophonBuildResp = await client.GetBuildAsync(context.LocalBranch, token).ConfigureAwait(false);
            }

            if (!sophonBuildResp.IsOk())
            {
                await taskContext.SwitchToMainThreadAsync();
                viewModel.Title = "清单数据拉取失败";
                viewModel.ResetProgress(0, 0);
                viewModel.Report(new(0, true));
                return;
            }

            SophonDecodedBuild sophonDecodedBuild = await DecodeManifestsAsync(sophonBuildResp.Data, context, token).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            viewModel.ResetProgress(sophonDecodedBuild.Manifests.Sum(m => m.ManifestProto.Assets.Sum(a => a.AssetChunks.Count)), sophonDecodedBuild.TotalBytes);
            await taskContext.SwitchToBackgroundAsync();

            List<SophonAsset> conflictAssets = [];

            ParallelOptions parallelOptions = new()
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            };

            foreach (SophonDecodedManifest sophonDecodedManifest in sophonDecodedBuild.Manifests)
            {
                await Parallel.ForEachAsync(sophonDecodedManifest.ManifestProto.Assets, parallelOptions, (asset, token) => VerifyAssetAsync(new(sophonDecodedManifest.UrlPrefix, asset), conflictAssets, context, progress, token)).ConfigureAwait(false);
            }

            await taskContext.SwitchToMainThreadAsync();
            viewModel.RefreshUI();
            await taskContext.SwitchToBackgroundAsync();

            if (conflictAssets.IsNullOrEmpty())
            {
                await taskContext.SwitchToMainThreadAsync();
                viewModel.Title = "游戏完整，无需修复";
                return;
            }

            await taskContext.SwitchToMainThreadAsync();
            viewModel.Title = "正在修复游戏完整性";
            viewModel.ResetProgress(conflictAssets.Sum(a => a.AssetProperty.AssetChunks.Count), conflictAssets.Sum(a => a.AssetProperty.AssetSize));
            await taskContext.SwitchToBackgroundAsync();

            await Parallel.ForEachAsync(conflictAssets, parallelOptions, async (asset, token) =>
            {
                await DownloadFileChunksAsync(asset, context, progress, token).ConfigureAwait(false);
                await CombineFileAsync(asset.AssetProperty, context, token).ConfigureAwait(false);
            }).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            viewModel.Title = "修复完成";
            viewModel.RefreshUI();
            await taskContext.SwitchToBackgroundAsync();

            Directory.Delete(context.ChunkDirectory, true);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async ValueTask UpdateAsync(GamePackageOperationWindow window, GamePackageOperationContext context, CancellationToken token = default)
    {
        try
        {
            GamePackageOperationViewModel viewModel = new("正在更新游戏");
            window.InitializeDataContext(viewModel);

            IProgress<SophonChunkDownloadStatus> progress = progressFactory.CreateForMainThread<SophonChunkDownloadStatus>(viewModel.Report);

            SophonBuild localBuild;
            SophonBuild remoteBuild;

            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                ISophonClient client = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<ISophonClient>>()
                    .Create(context.IsOversea);

                Response<SophonBuild> sophonBuildResp = await client.GetBuildAsync(context.LocalBranch, token).ConfigureAwait(false);
                if (!sophonBuildResp.IsOk())
                {
                    await taskContext.SwitchToMainThreadAsync();
                    viewModel.Title = "清单数据拉取失败";
                    viewModel.ResetProgress(0, 0);
                    viewModel.Report(new(0, true));
                    return;
                }

                localBuild = sophonBuildResp.Data;

                sophonBuildResp = await client.GetBuildAsync(context.RemoteBranch, token).ConfigureAwait(false);
                if (!sophonBuildResp.IsOk())
                {
                    await taskContext.SwitchToMainThreadAsync();
                    viewModel.Title = "清单数据拉取失败";
                    viewModel.ResetProgress(0, 0);
                    viewModel.Report(new(0, true));
                    return;
                }

                remoteBuild = sophonBuildResp.Data;
            }

            SophonDecodedBuild localDecodedBuild = await DecodeManifestsAsync(localBuild, context, token).ConfigureAwait(false);
            SophonDecodedBuild remoteDecodedBuild = await DecodeManifestsAsync(remoteBuild, context, token).ConfigureAwait(false);

            // Add
            // 本地没有，远端有
            List<SophonAsset> addedAssets = [];
            foreach ((SophonDecodedManifest localManifest, SophonDecodedManifest remoteManifest) in localDecodedBuild.Manifests.Zip(remoteDecodedBuild.Manifests))
            {
                foreach (SophonAsset sophonAsset in remoteManifest.ManifestProto.Assets.Except(localManifest.ManifestProto.Assets, new AssetPropertyNameComparer()).Select(ap => new SophonAsset(remoteManifest.UrlPrefix, ap)))
                {
                    addedAssets.Add(sophonAsset);
                }
            }

            // Modify
            // 本地有，远端有，但是内容不一致
            Dictionary<AssetProperty, SophonAsset> modifiedAssets = [];
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

                    modifiedAssets.Add(localAsset, new(remoteManifest.UrlPrefix, asset, asset.AssetChunks.Except(localAsset.AssetChunks, new AssetChunkMd5Comparer()).Select(ac => new SophonChunk(remoteManifest.UrlPrefix, ac)).ToList()));
                }
            }

            // Delete
            // 本地有，远端没有
            List<AssetProperty> deletedAssets = [];
            foreach ((SophonDecodedManifest localManifest, SophonDecodedManifest remoteManifest) in localDecodedBuild.Manifests.Zip(remoteDecodedBuild.Manifests))
            {
                localManifest.ManifestProto.Assets.Except(remoteManifest.ManifestProto.Assets, new AssetPropertyNameComparer()).ToList().ForEach(deletedAssets.Add);
            }

            // Patch
            await taskContext.SwitchToMainThreadAsync();
            viewModel.ResetProgress(addedAssets.Sum(a => a.AssetProperty.AssetChunks.Count) + modifiedAssets.Sum(a => a.Value.AssetProperty.AssetChunks.Count), addedAssets.Sum(a => a.AssetProperty.AssetSize) + modifiedAssets.Sum(a => a.Value.AssetProperty.AssetSize));
            await taskContext.SwitchToBackgroundAsync();

            ParallelOptions parallelOptions = new()
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            };

            // Added
            await Parallel.ForEachAsync(addedAssets, parallelOptions, async (asset, token) =>
            {
                if (asset.AssetProperty.AssetType is 64)
                {
                    Directory.CreateDirectory(Path.Combine(context.GameDirectory, asset.AssetProperty.AssetName));
                    return;
                }

                await DownloadFileChunksAsync(asset, context, progress, token).ConfigureAwait(false);
                await CombineFileAsync(asset.AssetProperty, context, token).ConfigureAwait(false);
            }).ConfigureAwait(false);

            // Modified
            // 内容未发生变化但是偏移量发生变化的块，从旧asset读取并写入新asset流
            // 内容发生变化的块直接读取diff chunk写入新asset流
            await Parallel.ForEachAsync(modifiedAssets, parallelOptions, async (asset, token) =>
            {
                foreach (SophonChunk sophonChunk in asset.Value.DiffChunks)
                {
                    await DownloadChunkAsync(sophonChunk, context, progress, token).ConfigureAwait(false);
                }

                await CombineDiffFileAsync(asset.Key, asset.Value, context, token).ConfigureAwait(false);
            }).ConfigureAwait(false);

            // Deleted
            foreach (AssetProperty asset in deletedAssets)
            {
                File.Delete(Path.Combine(context.GameDirectory, asset.AssetName));
            }

            // Verify
            await taskContext.SwitchToMainThreadAsync();
            viewModel.Title = "正在验证游戏完整性";
            viewModel.ResetProgress(remoteDecodedBuild.Manifests.Sum(m => m.ManifestProto.Assets.Sum(a => a.AssetChunks.Count)), remoteDecodedBuild.TotalBytes);
            await taskContext.SwitchToBackgroundAsync();

            List<SophonAsset> conflictAssets = [];

            foreach (SophonDecodedManifest sophonDecodedManifest in remoteDecodedBuild.Manifests)
            {
                await Parallel.ForEachAsync(sophonDecodedManifest.ManifestProto.Assets, parallelOptions, (asset, token) => VerifyAssetAsync(new(sophonDecodedManifest.UrlPrefix, asset), conflictAssets, context, progress, token)).ConfigureAwait(false);
            }

            await taskContext.SwitchToMainThreadAsync();
            viewModel.RefreshUI();
            await taskContext.SwitchToBackgroundAsync();

            if (conflictAssets.IsNullOrEmpty())
            {
                await taskContext.SwitchToMainThreadAsync();
                viewModel.Title = "更新完成";
                File.Delete(context.ChunkDirectory);
                return;
            }

            await taskContext.SwitchToMainThreadAsync();
            viewModel.Title = "正在修复游戏完整性";
            viewModel.ResetProgress(conflictAssets.Sum(a => a.AssetProperty.AssetChunks.Count), conflictAssets.Sum(a => a.AssetProperty.AssetSize));
            await taskContext.SwitchToBackgroundAsync();

            await Parallel.ForEachAsync(conflictAssets, parallelOptions, async (asset, token) =>
            {
                await DownloadFileChunksAsync(asset, context, progress, token).ConfigureAwait(false);
                await CombineFileAsync(asset.AssetProperty, context, token).ConfigureAwait(false);
            }).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            viewModel.Title = "更新完成";
            viewModel.RefreshUI();
            File.Delete(context.ChunkDirectory);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async ValueTask PredownloadAsync(GamePackageOperationWindow window, GamePackageOperationContext context, CancellationToken token = default)
    {
        try
        {
            GamePackageOperationViewModel viewModel = new("正在预下载资源");
            window.InitializeDataContext(viewModel);

            IProgress<SophonChunkDownloadStatus> progress = progressFactory.CreateForMainThread<SophonChunkDownloadStatus>(viewModel.Report);

            SophonBuild localBuild;
            SophonBuild remoteBuild;

            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                ISophonClient client = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<ISophonClient>>()
                    .Create(context.IsOversea);

                Response<SophonBuild> sophonBuildResp = await client.GetBuildAsync(context.LocalBranch, token).ConfigureAwait(false);
                if (!sophonBuildResp.IsOk())
                {
                    await taskContext.SwitchToMainThreadAsync();
                    viewModel.Title = "清单数据拉取失败";
                    viewModel.ResetProgress(0, 0);
                    viewModel.Report(new(0, true));
                    return;
                }

                localBuild = sophonBuildResp.Data;

                sophonBuildResp = await client.GetBuildAsync(context.RemoteBranch, token).ConfigureAwait(false);
                if (!sophonBuildResp.IsOk())
                {
                    await taskContext.SwitchToMainThreadAsync();
                    viewModel.Title = "清单数据拉取失败";
                    viewModel.ResetProgress(0, 0);
                    viewModel.Report(new(0, true));
                    return;
                }

                remoteBuild = sophonBuildResp.Data;
            }

            SophonDecodedBuild localDecodedBuild = await DecodeManifestsAsync(localBuild, context, token).ConfigureAwait(false);
            SophonDecodedBuild remoteDecodedBuild = await DecodeManifestsAsync(remoteBuild, context, token).ConfigureAwait(false);

            // 和Update相比不需要处理delete，不需要Combine

            // Add
            // 本地没有，远端有
            List<SophonAsset> addedAssets = [];
            foreach ((SophonDecodedManifest localManifest, SophonDecodedManifest remoteManifest) in localDecodedBuild.Manifests.Zip(remoteDecodedBuild.Manifests))
            {
                foreach (SophonAsset sophonAsset in remoteManifest.ManifestProto.Assets.Except(localManifest.ManifestProto.Assets, new AssetPropertyNameComparer()).Select(ap => new SophonAsset(remoteManifest.UrlPrefix, ap)))
                {
                    addedAssets.Add(sophonAsset);
                }
            }

            // Modify
            // 本地有，远端有，但是内容不一致
            Dictionary<AssetProperty, SophonAsset> modifiedAssets = [];
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

                    modifiedAssets.Add(localAsset, new(remoteManifest.UrlPrefix, asset, asset.AssetChunks.Except(localAsset.AssetChunks, new AssetChunkMd5Comparer()).Select(ac => new SophonChunk(remoteManifest.UrlPrefix, ac)).ToList()));
                }
            }

            // Download
            await taskContext.SwitchToMainThreadAsync();
            viewModel.ResetProgress(addedAssets.Sum(a => a.AssetProperty.AssetChunks.Count) + modifiedAssets.Sum(a => a.Value.AssetProperty.AssetChunks.Count), addedAssets.Sum(a => a.AssetProperty.AssetSize) + modifiedAssets.Sum(a => a.Value.AssetProperty.AssetSize));
            await taskContext.SwitchToBackgroundAsync();

            ParallelOptions parallelOptions = new()
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            };

            // Added
            await Parallel.ForEachAsync(addedAssets, parallelOptions, (asset, token) => DownloadFileChunksAsync(asset, context, progress, token)).ConfigureAwait(false);

            // Modified
            await Parallel.ForEachAsync(modifiedAssets, parallelOptions, async (asset, token) =>
            {
                foreach (SophonChunk sophonChunk in asset.Value.DiffChunks)
                {
                    await DownloadChunkAsync(sophonChunk, context, progress, token).ConfigureAwait(false);
                }
            }).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            viewModel.Title = "预下载完成";
            viewModel.RefreshUI();
            launchOptions.IsPredownloadFinished = true;
            return;
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async ValueTask<SophonDecodedBuild> DecodeManifestsAsync(SophonBuild sophonBuild, GamePackageOperationContext context, CancellationToken token = default)
    {
        long totalBytes = 0L;
        List<SophonDecodedManifest> manifests = [];
        foreach (SophonManifest sophonManifest in sophonBuild.Manifests)
        {
            bool exclude = sophonManifest.MatchingField switch
            {
                "game" => false,
                "zh-cn" => !context.GameAudioSystem.Chinese,
                "en-us" => !context.GameAudioSystem.English,
                "ja-jp" => !context.GameAudioSystem.Japanese,
                "ko-kr" => !context.GameAudioSystem.Korean,
                _ => true,
            };

            if (exclude)
            {
                continue;
            }

            totalBytes += sophonManifest.Stats.UncompressedSize;
            string manifestDownloadUrl = $"{sophonManifest.ManifestDownload.UrlPrefix}/{sophonManifest.Manifest.Id}";

            using (HttpClient httpClient = new())
            {
                using (HttpResponseMessage resp = await httpClient.GetAsync(manifestDownloadUrl, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
                {
                    using (Stream manifestStream = await resp.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
                    {
                        using (DecompressionStream decompressionStream = new(manifestStream))
                        {
                            // TODO: Migrate to Snap.ZStandard
                            //string manifestMd5 = await MD5.HashAsync(decompressionStream, token).ConfigureAwait(false);

                            //if (manifestMd5.Equals(sophonManifest.Manifest.Checksum, StringComparison.OrdinalIgnoreCase))
                            //{
                            manifests.Add(new(sophonManifest.ChunkDownload.UrlPrefix, SophonManifestProto.Parser.ParseFrom(decompressionStream)));
                            //}
                        }
                    }
                }
            }
        }

        return new(totalBytes, manifests);
    }

    private async ValueTask VerifyAssetAsync(SophonAsset sophonAsset, List<SophonAsset> conflictAssets, GamePackageOperationContext context, IProgress<SophonChunkDownloadStatus> progress, CancellationToken token = default)
    {
        string assetPath = Path.Combine(context.GameDirectory, sophonAsset.AssetProperty.AssetName);

        if (sophonAsset.AssetProperty.AssetType is 64)
        {
            Directory.CreateDirectory(assetPath);
            return;
        }

        if (!File.Exists(assetPath))
        {
            conflictAssets.Add(sophonAsset);
            await taskContext.SwitchToMainThreadAsync();
            for (int i = 0; i < sophonAsset.AssetProperty.AssetChunks.Count; i++)
            {
                progress.Report(new(0, true));
            }

            return;
        }

        using (FileStream fileStream = File.OpenRead(assetPath))
        {
            for (int i = 0; i < sophonAsset.AssetProperty.AssetChunks.Count; i++)
            {
                AssetChunk chunk = sophonAsset.AssetProperty.AssetChunks[i];
                using (Stream hashStream = await fileStream.CloneSegmentAsync(chunk.ChunkOnFileOffset, chunk.ChunkSizeDecompressed).ConfigureAwait(false))
                {
                    string chunkMd5 = await MD5.HashAsync(hashStream, token).ConfigureAwait(false);
                    if (!chunkMd5.Equals(chunk.ChunkDecompressedHashMd5, StringComparison.OrdinalIgnoreCase))
                    {
                        conflictAssets.Add(sophonAsset);
                        await taskContext.SwitchToMainThreadAsync();
                        for (int j = i; j < sophonAsset.AssetProperty.AssetChunks.Count; j++)
                        {
                            progress.Report(new(0, true));
                        }

                        await taskContext.SwitchToBackgroundAsync();
                        return;
                    }

                    await taskContext.SwitchToMainThreadAsync();
                    progress.Report(new(chunk.ChunkSizeDecompressed, true));
                    await taskContext.SwitchToBackgroundAsync();
                }
            }
        }
    }

    private async ValueTask DownloadFileChunksAsync(SophonAsset sophonAsset, GamePackageOperationContext context, IProgress<SophonChunkDownloadStatus> progress, CancellationToken token = default)
    {
        ParallelOptions parallelOptions = new()
        {
            CancellationToken = token,
            MaxDegreeOfParallelism = Environment.ProcessorCount,
        };

        await Parallel.ForEachAsync(sophonAsset.AssetProperty.AssetChunks, parallelOptions, (chunk, token) => DownloadChunkAsync(new(sophonAsset.UrlPrefix, chunk), context, progress, token)).ConfigureAwait(false);
    }

    private async ValueTask DownloadChunkAsync(SophonChunk sophonChunk, GamePackageOperationContext context, IProgress<SophonChunkDownloadStatus> progress, CancellationToken token = default)
    {
        Directory.CreateDirectory(context.ChunkDirectory);
        string chunkPath = Path.Combine(context.ChunkDirectory, sophonChunk.AssetChunk.ChunkName);
        if (File.Exists(chunkPath))
        {
            string chunkXxh64 = await XXH64.HashFileAsync(chunkPath, token).ConfigureAwait(false);
            if (chunkXxh64.Equals(sophonChunk.AssetChunk.ChunkName.Split("_")[0], StringComparison.OrdinalIgnoreCase))
            {
                await taskContext.SwitchToMainThreadAsync();
                progress.Report(new(sophonChunk.AssetChunk.ChunkSize, true));
                return;
            }

            File.Delete(chunkPath);
        }

        using (FileStream fileStream = File.Create(chunkPath))
        {
            fileStream.Seek(0, SeekOrigin.Begin);

            using (HttpClient httpClient = new())
            {
                using (HttpResponseMessage responseMessage = await httpClient.GetAsync(sophonChunk.ChunkDownloadUrl, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
                {
                    long totalBytes = responseMessage.Content.Headers.ContentLength ?? 0;
                    using (Stream webStream = await responseMessage.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
                    {
                        StreamCopyWorker<SophonChunkDownloadStatus> worker = new(webStream, fileStream, bytesRead => new(bytesRead, false));

                        await worker.CopyAsync(progress).ConfigureAwait(false);

                        fileStream.Seek(0, SeekOrigin.Begin);
                        string chunkXxh64 = await XXH64.HashAsync(fileStream, token).ConfigureAwait(false);
                        if (chunkXxh64.Equals(sophonChunk.AssetChunk.ChunkName.Split("_")[0], StringComparison.OrdinalIgnoreCase))
                        {
                            progress.Report(new(0, true));
                        }
                    }
                }
            }
        }
    }

    private async ValueTask CombineFileAsync(AssetProperty assetProperty, GamePackageOperationContext context, CancellationToken token = default)
    {
        string path = Path.Combine(context.GameDirectory, assetProperty.AssetName);
        string? directory = Path.GetDirectoryName(path);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);
        using (FileStream file = File.Create(path))
        {
            foreach (AssetChunk chunk in assetProperty.AssetChunks)
            {
                string chunkPath = Path.Combine(context.ChunkDirectory, chunk.ChunkName);
                using (FileStream chunkFile = File.OpenRead(chunkPath))
                {
                    using (DecompressionStream decompressionStream = new(chunkFile))
                    {
                        // TODO: Migrate to Snap.ZStandard
                        //string chunkMd5 = await MD5.HashAsync(chunkFile, token).ConfigureAwait(false);
                        //if (chunkMd5.Equals(chunk.ChunkDecompressedHashMd5, StringComparison.OrdinalIgnoreCase))
                        //{
                        file.Position = chunk.ChunkOnFileOffset;
                        await decompressionStream.CopyToAsync(file, token).ConfigureAwait(false);
                        //}
                    }
                }
            }
        }
    }

    private async ValueTask CombineDiffFileAsync(AssetProperty oldAsset, SophonAsset newAsset, GamePackageOperationContext context, CancellationToken token = default)
    {
        using (MemoryStream newAssetStream = new())
        {
            using (FileStream oldAssetStream = File.OpenRead(Path.Combine(context.GameDirectory, oldAsset.AssetName)))
            {
                foreach (AssetChunk chunk in newAsset.AssetProperty.AssetChunks)
                {
                    newAssetStream.Position = chunk.ChunkOnFileOffset;

                    AssetChunk? oldChunk = oldAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5 == chunk.ChunkDecompressedHashMd5);
                    if (oldChunk is null)
                    {
                        using (FileStream diffStream = File.OpenRead(Path.Combine(context.ChunkDirectory, chunk.ChunkName)))
                        {
                            using (DecompressionStream decompressionStream = new(diffStream))
                            {
                                // TODO: Migrate to Snap.ZStandard
                                //string chunkMd5 = await MD5.HashAsync(chunkFile, token).ConfigureAwait(false);
                                //if (chunkMd5.Equals(chunk.ChunkDecompressedHashMd5, StringComparison.OrdinalIgnoreCase))
                                //{
                                await decompressionStream.CopyToAsync(newAssetStream, token).ConfigureAwait(false);
                                //}
                                continue;
                            }
                        }
                    }

                    oldAssetStream.Position = oldChunk.ChunkOnFileOffset;
                    using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(81920))
                    {
                        Memory<byte> buffer = memoryOwner.Memory;
                        long bytesToCopy = oldChunk.ChunkSizeDecompressed;
                        while (bytesToCopy > 0)
                        {
                            int bytesRead = await oldAssetStream.ReadAsync(buffer[..(int)Math.Min(buffer.Length, bytesToCopy)], token).ConfigureAwait(false);
                            if (bytesRead <= 0)
                            {
                                break;
                            }

                            await newAssetStream.WriteAsync(buffer[..bytesRead], token).ConfigureAwait(false);
                            bytesToCopy -= bytesRead;
                        }
                    }
                }

                File.Delete(Path.Combine(context.GameDirectory, oldAsset.AssetName));
                using (FileStream newAssetFileStream = File.Create(Path.Combine(context.GameDirectory, newAsset.AssetProperty.AssetName)))
                {
                    newAssetStream.Position = 0;
                    await newAssetStream.CopyToAsync(newAssetFileStream, token).ConfigureAwait(false);
                }
            }
        }
    }

    private sealed class SophonDecodedBuild
    {
        public SophonDecodedBuild(long totalBytes, List<SophonDecodedManifest> manifests)
        {
            TotalBytes = totalBytes;
            Manifests = manifests;
        }

        public long TotalBytes { get; }

        public List<SophonDecodedManifest> Manifests { get; }
    }

    private sealed class SophonDecodedManifest
    {
        public SophonDecodedManifest(string urlPrefix, SophonManifestProto sophonManifestProto)
        {
            UrlPrefix = urlPrefix;
            ManifestProto = sophonManifestProto;
        }

        public string UrlPrefix { get; }

        public SophonManifestProto ManifestProto { get; }
    }

    private sealed class SophonAsset
    {
        public SophonAsset(string urlPrefix, AssetProperty assetProperty)
        {
            UrlPrefix = urlPrefix;
            AssetProperty = assetProperty;
        }

        public SophonAsset(string urlPrefix, AssetProperty assetProperty, List<SophonChunk> diffChunks)
        {
            UrlPrefix = urlPrefix;
            AssetProperty = assetProperty;
            DiffChunks = diffChunks;
        }

        public string UrlPrefix { get; }

        public AssetProperty AssetProperty { get; }

        public List<SophonChunk> DiffChunks { get; } = default!;
    }

    private sealed class SophonChunk
    {
        public SophonChunk(string urlPrefix, AssetChunk assetChunk)
        {
            UrlPrefix = urlPrefix;
            AssetChunk = assetChunk;
        }

        public string UrlPrefix { get; }

        public AssetChunk AssetChunk { get; }

        public string ChunkDownloadUrl { get => $"{UrlPrefix}/{AssetChunk.ChunkName}"; }
    }

    private sealed class AssetPropertyNameComparer : IEqualityComparer<AssetProperty>
    {
        public bool Equals(AssetProperty? x, AssetProperty? y)
        {
            ArgumentNullException.ThrowIfNull(x);
            ArgumentNullException.ThrowIfNull(y);
            return string.Equals(x.AssetName, y.AssetName, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] AssetProperty obj)
        {
            return obj.GetHashCode();
        }
    }

    private sealed class AssetChunkMd5Comparer : IEqualityComparer<AssetChunk>
    {
        public bool Equals(AssetChunk? x, AssetChunk? y)
        {
            ArgumentNullException.ThrowIfNull(x);
            ArgumentNullException.ThrowIfNull(y);
            return string.Equals(x.ChunkDecompressedHashMd5, y.ChunkDecompressedHashMd5, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] AssetChunk obj)
        {
            return obj.GetHashCode();
        }
    }
}