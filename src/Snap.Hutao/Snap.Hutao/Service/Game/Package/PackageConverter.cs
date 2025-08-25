// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Compression.Zstandard;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Factory.IO;
using Snap.Hutao.Service.Game.Package.Advanced.AssetOperation;
using Snap.Hutao.Service.Game.Package.Advanced.Model;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.Downloader;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using Snap.Hutao.Web.Hoyolab.Takumi.Downloader.Proto;
using Snap.Hutao.Web.Response;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace Snap.Hutao.Service.Game.Package;

[ConstructorGenerated]
[Service(ServiceLifetime.Transient, typeof(IPackageConverter))]
internal sealed partial class PackageConverter : IPackageConverter
{
    private readonly IMemoryStreamFactory memoryStreamFactory;
    private readonly ILogger<PackageConverter> logger;
    private readonly IServiceProvider serviceProvider;

    public async ValueTask EnsureDeprecatedFilesAndSdkAsync(PackageConverterDeprecationContext context)
    {
        // Just try to delete these files, always download from server when needed
        string gameDirectory = context.GameFileSystem.GetGameDirectory();
        FileOperation.Delete(Path.Combine(gameDirectory, GameConstants.YuanShenData, "Plugins\\PCGameSDK.dll"));
        FileOperation.Delete(Path.Combine(gameDirectory, GameConstants.GenshinImpactData, "Plugins\\PCGameSDK.dll"));
        FileOperation.Delete(Path.Combine(gameDirectory, GameConstants.YuanShenData, "Plugins\\EOSSDK-Win64-Shipping.dll"));
        FileOperation.Delete(Path.Combine(gameDirectory, GameConstants.GenshinImpactData, "Plugins\\EOSSDK-Win64-Shipping.dll"));
        FileOperation.Delete(Path.Combine(gameDirectory, GameConstants.YuanShenData, "Plugins\\PluginEOSSDK.dll"));
        FileOperation.Delete(Path.Combine(gameDirectory, GameConstants.GenshinImpactData, "Plugins\\PluginEOSSDK.dll"));
        FileOperation.Delete(Path.Combine(gameDirectory, "sdk_pkg_version"));

        if (context.GameChannelSdk is not null)
        {
            using (Stream sdkWebStream = await context.HttpClient.GetStreamAsync(context.GameChannelSdk.ChannelSdkPackage.Url).ConfigureAwait(false))
            {
                ZipFile.ExtractToDirectory(sdkWebStream, gameDirectory, true);
            }
        }

        if (context.DeprecatedFiles is not null)
        {
            foreach (DeprecatedFile file in context.DeprecatedFiles.DeprecatedFiles)
            {
                string filePath = Path.Combine(gameDirectory, file.Name);
                FileOperation.Move(filePath, $"{filePath}.backup", true);
            }
        }
    }

    public async ValueTask<bool> EnsureGameResourceAsync(PackageConverterContext context)
    {
        // 基本步骤与 ScatteredPackageConverter 相同
        // 以 国服 -> 国际服 为例
        // 1. 获取两服的清单文件
        //
        // 2. 对比两者差异，（类似更新处理）
        //    国际服有 & 国服没有的 为新增
        //    国际服有 & 国服也有的 为替换或修补
        //    剩余国际服没有 & 国服有的 为备份
        //    生成对应的操作信息项，对比文件的尺寸与MD5
        //
        // 3. 根据操作信息项，提取其中需要下载的项进行缓存对比或下载
        //    若缓存中文件的尺寸与MD5与操作信息项中的一致则直接跳过
        //    每个文件下载后需要验证文件文件的尺寸与MD5
        //    若出现下载失败的情况，终止转换进程，此时国服文件尚未替换
        //
        // 4. 全部资源下载完成后，根据操作信息项，进行文件替换
        //    处理顺序：备份/替换/新增
        //    替换操作等于 先备份国服文件，随后新增国际服文件
        // TODO: 可能会存在大量相似代码，逻辑完成后再进行重构
        ArgumentNullException.ThrowIfNull(context.SophonChunksOnly.CurrentBranch);
        ArgumentNullException.ThrowIfNull(context.SophonChunksOnly.TargetBranch);

        // Step 1
        context.Progress.Report(new(SH.UIXamlViewSpecializedSophonProgressDefault));
        SophonDecodedBuild? currentBuild = await DecodeManifestsAsync(context, context.SophonChunksOnly.CurrentBranch, context.CurrentScheme).ConfigureAwait(false);
        SophonDecodedBuild? targetBuild = await DecodeManifestsAsync(context, context.SophonChunksOnly.TargetBranch, context.TargetScheme).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(currentBuild);
        ArgumentNullException.ThrowIfNull(targetBuild);

        // Step 2
        List<PackageItemOperationForSophonChunks> diffOperations = GetDiffOperations(currentBuild, targetBuild).ToList();
        diffOperations.SortBy(o => o.Kind);
        InitializeDuplicatedChunkNames(context, diffOperations.SelectMany(a => a.DiffChunks.Select(c => c.AssetChunk)));

        // Step 3
        await PrepareCacheFilesAsync(context, diffOperations).ConfigureAwait(false);

        // Step 4
        return ReplaceGameResource(context, diffOperations);
    }

    private static IEnumerable<PackageItemOperationForSophonChunks> GetDiffOperations(SophonDecodedBuild currentDecodedBuild, SophonDecodedBuild targetDecodedBuild)
    {
        foreach ((SophonDecodedManifest currentManifest, SophonDecodedManifest targetManifest) in currentDecodedBuild.Manifests.Zip(targetDecodedBuild.Manifests))
        {
            foreach (AssetProperty targetAsset in targetManifest.Data.Assets)
            {
                if (currentManifest.Data.Assets.FirstOrDefault(currentAsset => IsSameAsset(currentAsset, targetAsset)) is not { } currentAsset)
                {
                    yield return PackageItemOperationForSophonChunks.Add(targetManifest.UrlPrefix, targetManifest.UrlSuffix, targetAsset);
                    continue;
                }

                if (currentAsset.AssetHashMd5.Equals(targetAsset.AssetHashMd5, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                List<SophonChunk> diffChunks = [];
                foreach (AssetChunk chunk in targetAsset.AssetChunks)
                {
                    if (currentAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5.Equals(chunk.ChunkDecompressedHashMd5, StringComparison.OrdinalIgnoreCase)) is null)
                    {
                        diffChunks.Add(new(targetManifest.UrlPrefix, targetManifest.UrlSuffix, chunk));
                    }
                }

                yield return PackageItemOperationForSophonChunks.ModifyOrReplace(targetManifest.UrlPrefix, targetManifest.UrlSuffix, currentAsset, targetAsset, diffChunks);
            }

            foreach (AssetProperty currentAsset in currentManifest.Data.Assets)
            {
                if (targetManifest.Data.Assets.FirstOrDefault(a => IsSameAsset(a, currentAsset)) is null)
                {
                    yield return PackageItemOperationForSophonChunks.Backup(currentAsset);
                }
            }
        }

        static bool IsSameAsset(AssetProperty currentAsset, AssetProperty targetAsset)
        {
            // Ignore YuanShen_Data/ or GenshinImpact_Data/
            string currentAssetName = currentAsset.AssetName[(currentAsset.AssetName.IndexOf('/', StringComparison.OrdinalIgnoreCase) + 1)..];
            string targetAssetName = targetAsset.AssetName[(targetAsset.AssetName.IndexOf('/', StringComparison.OrdinalIgnoreCase) + 1)..];
            return currentAssetName.Equals(targetAssetName, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static void InitializeDuplicatedChunkNames(PackageConverterContext context, IEnumerable<AssetChunk> chunks)
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

    private static async ValueTask DownloadChunksAsync(PackageConverterContext context, IEnumerable<SophonChunk> sophonChunks)
    {
        await Parallel.ForEachAsync(sophonChunks, context.ParallelOptions, (chunk, token) => DownloadChunkAsync(context, chunk)).ConfigureAwait(false);
    }

    private static async ValueTask DownloadChunkAsync(PackageConverterContext context, SophonChunk sophonChunk)
    {
        Directory.CreateDirectory(context.ServerCacheChunksFolder);
        string chunkPath = Path.Combine(context.ServerCacheChunksFolder, sophonChunk.AssetChunk.ChunkName);

        using (await context.ExclusiveProcessChunkAsync(sophonChunk.AssetChunk.ChunkName).ConfigureAwait(false))
        {
            if (File.Exists(chunkPath))
            {
                string chunkXxh64 = await XxHash64.HashFileAsync(chunkPath).ConfigureAwait(false);
                if (chunkXxh64.Equals(sophonChunk.AssetChunk.ChunkName.Split("_")[0], StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                File.Delete(chunkPath);
            }

            using (FileStream fileStream = File.Create(chunkPath))
            {
                fileStream.Position = 0;

                using (Stream webStream = await context.HttpClient.GetStreamAsync(sophonChunk.ChunkDownloadUrl).ConfigureAwait(false))
                {
                    using (StreamCopyWorker<PackageConvertStatus> worker = new(webStream, fileStream, (_, totalBytesRead) => new PackageConvertStatus(sophonChunk.AssetChunk.ChunkName, totalBytesRead, sophonChunk.AssetChunk.ChunkSize)))
                    {
                        await worker.CopyAsync(context.Progress).ConfigureAwait(false);

                        fileStream.Position = 0;
                        string chunkXxh64 = await XxHash64.HashAsync(fileStream).ConfigureAwait(false);
                        if (chunkXxh64.Equals(sophonChunk.AssetChunk.ChunkName.Split("_")[0], StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }
                    }
                }
            }
        }
    }

    private static async ValueTask MergeNewAssetAsync(PackageConverterContext context, AssetProperty assetProperty)
    {
        using (SafeFileHandle fileHandle = File.OpenHandle(context.GetServerCacheTargetFilePath(assetProperty.AssetName), FileMode.Create, FileAccess.Write, FileShare.None, preallocationSize: assetProperty.AssetSize))
        {
            await Parallel.ForEachAsync(assetProperty.AssetChunks, context.ParallelOptions, (chunk, token) => MergeChunkIntoAssetAsync(context, fileHandle, chunk)).ConfigureAwait(false);
        }
    }

    private static async ValueTask MergeChunkIntoAssetAsync(PackageConverterContext context, SafeFileHandle fileHandle, AssetChunk chunk)
    {
        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(GameAssetOperation.ChunkBufferSize))
        {
            Memory<byte> buffer = memoryOwner.Memory;

            string chunkPath = Path.Combine(context.ServerCacheChunksFolder, chunk.ChunkName);
            if (!File.Exists(chunkPath))
            {
                return;
            }

            using (await context.ExclusiveProcessChunkAsync(chunk.ChunkName).ConfigureAwait(true))
            {
                using (FileStream chunkFile = File.OpenRead(chunkPath))
                {
                    using (ZstandardDecompressStream decompressStream = new(chunkFile))
                    {
                        long offset = chunk.ChunkOnFileOffset;
                        do
                        {
                            int bytesRead = await decompressStream.ReadAsync(buffer).ConfigureAwait(true);
                            if (bytesRead <= 0)
                            {
                                break;
                            }

                            await RandomAccess.WriteAsync(fileHandle, buffer[..bytesRead], offset).ConfigureAwait(true);
                            offset += bytesRead;
                        }
                        while (true);
                    }
                }

                if (!context.DuplicatedChunkNames.ContainsKey(chunk.ChunkName))
                {
                    FileOperation.Delete(chunkPath);
                }
            }
        }
    }

    private async ValueTask<SophonDecodedBuild?> DecodeManifestsAsync(PackageConverterContext context, BranchWrapper branch, LaunchScheme scheme)
    {
        SophonBuild? build;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ISophonClient client = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<ISophonClient>>()
                .Create(scheme.IsOversea);

            Response<SophonBuild> response = await client.GetBuildAsync(branch).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(response, serviceProvider, out build))
            {
                return default;
            }
        }

        SophonManifest sophonManifest = build.Manifests.Single(m => m.MatchingField == "game");
        string manifestDownloadUrl = $"{sophonManifest.ManifestDownload.UrlPrefix}/{sophonManifest.Manifest.Id}";
        using (Stream rawManifestStream = await context.HttpClient.GetStreamAsync(manifestDownloadUrl).ConfigureAwait(false))
        {
            using (ZstandardDecompressStream decompressor = new(rawManifestStream))
            {
                using (MemoryStream inMemoryManifestStream = await memoryStreamFactory.GetStreamAsync(decompressor).ConfigureAwait(false))
                {
                    string manifestMd5 = await Hash.ToHexStringAsync(HashAlgorithmName.MD5, inMemoryManifestStream).ConfigureAwait(false);
                    if (!manifestMd5.Equals(sophonManifest.Manifest.Checksum, StringComparison.OrdinalIgnoreCase))
                    {
                        return default!;
                    }

                    inMemoryManifestStream.Position = 0;
                    SophonDecodedManifest decodedManifest = new(sophonManifest.ChunkDownload.UrlPrefix, sophonManifest.ChunkDownload.UrlSuffix, SophonManifestProto.Parser.ParseFrom(inMemoryManifestStream));
                    return new(build.Tag, sophonManifest.Stats.CompressedSize, sophonManifest.Stats.UncompressedSize, [decodedManifest]);
                }
            }
        }
    }

    private async ValueTask PrepareCacheFilesAsync(PackageConverterContext context, List<PackageItemOperationForSophonChunks> operations)
    {
        foreach (PackageItemOperationForSophonChunks operation in operations)
        {
            ValueTask task = operation.Kind switch
            {
                PackageItemOperationKind.Replace or PackageItemOperationKind.Add => SkipOrProcessAsync(context, operation),
                _ => ValueTask.CompletedTask,
            };

            await task.ConfigureAwait(false);
        }

        if (Directory.Exists(context.ServerCacheChunksFolder))
        {
            Directory.Delete(context.ServerCacheChunksFolder, true);
        }
    }

    private async ValueTask SkipOrProcessAsync(PackageConverterContext context, PackageItemOperationForSophonChunks operation)
    {
        string cacheFile = context.GetServerCacheTargetFilePath(operation.NewAsset.AssetName);

        if (File.Exists(cacheFile))
        {
            if (operation.NewAsset.AssetSize == new System.IO.FileInfo(cacheFile).Length)
            {
                if (operation.NewAsset.AssetHashMd5.Equals(await Hash.FileToHexStringAsync(HashAlgorithmName.MD5, cacheFile).ConfigureAwait(false), StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            // Invalid file, delete it
            File.Delete(cacheFile);
        }

        string? directory = Path.GetDirectoryName(cacheFile);
        ArgumentException.ThrowIfNullOrEmpty(directory);
        Directory.CreateDirectory(directory);

        await EnsureAssetAsync(context, operation).ConfigureAwait(false);
    }

    private async ValueTask EnsureAssetAsync(PackageConverterContext context, PackageItemOperationForSophonChunks asset)
    {
        IEnumerable<SophonChunk> chunks = asset.Kind switch
        {
            PackageItemOperationKind.Add => asset.NewAsset.AssetChunks.Select(chunk => new SophonChunk(asset.UrlPrefix, asset.UrlSuffix, chunk)),
            PackageItemOperationKind.Replace => asset.DiffChunks,
            _ => [],
        };

        await DownloadChunksAsync(context, chunks).ConfigureAwait(false);
        await MergeAssetAsync(context, asset).ConfigureAwait(false);
    }

    private async ValueTask MergeAssetAsync(PackageConverterContext context, PackageItemOperationForSophonChunks asset)
    {
        ValueTask task = asset.Kind switch
        {
            PackageItemOperationKind.Add => MergeNewAssetAsync(context, asset.NewAsset),
            PackageItemOperationKind.Replace => MergeDiffAssetAsync(context, asset),
            _ => ValueTask.CompletedTask,
        };

        // TODO: Set Progress as indeterminate
        await task.ConfigureAwait(false);
    }

    private async ValueTask MergeDiffAssetAsync(PackageConverterContext context, PackageItemOperationForSophonChunks asset)
    {
        using (MemoryStream newAssetStream = memoryStreamFactory.GetStream())
        {
            string oldAssetPath = Path.Combine(context.GameFileSystem.GetGameDirectory(), asset.OldAsset.AssetName);
            if (!File.Exists(oldAssetPath))
            {
                // File not found, skip this asset and repair later
                return;
            }

            using (SafeFileHandle oldAssetHandle = File.OpenHandle(oldAssetPath, options: FileOptions.RandomAccess))
            {
                foreach (AssetChunk chunk in asset.NewAsset.AssetChunks)
                {
                    newAssetStream.Position = chunk.ChunkOnFileOffset;

                    if (asset.OldAsset.AssetChunks.FirstOrDefault(c => c.ChunkDecompressedHashMd5 == chunk.ChunkDecompressedHashMd5) is not { } oldChunk)
                    {
                        string chunkPath = Path.Combine(context.ServerCacheChunksFolder, chunk.ChunkName);
                        if (!File.Exists(chunkPath))
                        {
                            // File not found, skip this asset and repair later
                            return;
                        }

                        using (await context.ExclusiveProcessChunkAsync(chunk.ChunkName).ConfigureAwait(false))
                        {
                            using (FileStream diffStream = File.OpenRead(chunkPath))
                            {
                                using (ZstandardDecompressStream decompressor = new(diffStream))
                                {
                                    await decompressor.CopyToAsync(newAssetStream).ConfigureAwait(false);
                                }
                            }

                            if (!context.DuplicatedChunkNames.ContainsKey(chunk.ChunkName))
                            {
                                FileOperation.Delete(chunkPath);
                            }
                        }
                    }
                    else
                    {
                        using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(GameAssetOperation.ChunkBufferSize))
                        {
                            Memory<byte> buffer = memoryOwner.Memory;
                            long offset = oldChunk.ChunkOnFileOffset;
                            long bytesToCopy = oldChunk.ChunkSizeDecompressed;
                            while (bytesToCopy > 0)
                            {
                                int bytesRead = await RandomAccess.ReadAsync(oldAssetHandle, buffer[..(int)Math.Min(buffer.Length, bytesToCopy)], offset).ConfigureAwait(false);
                                if (bytesRead <= 0)
                                {
                                    break;
                                }

                                await newAssetStream.WriteAsync(buffer[..bytesRead]).ConfigureAwait(false);
                                offset += bytesRead;
                                bytesToCopy -= bytesRead;
                            }
                        }
                    }
                }
            }

            string path = context.GetServerCacheTargetFilePath(asset.NewAsset.AssetName);
            using (FileStream newAssetFileStream = File.Create(path))
            {
                newAssetStream.Position = 0;
                await newAssetStream.CopyToAsync(newAssetFileStream).ConfigureAwait(false);
            }
        }
    }

    private bool ReplaceGameResource(PackageConverterContext context, List<PackageItemOperationForSophonChunks> operations)
    {
        // 执行下载与移动操作
        foreach (PackageItemOperationForSophonChunks operation in operations)
        {
            (bool moveToBackup, bool moveToTarget) = operation.Kind switch
            {
                PackageItemOperationKind.Backup => (true, false),
                PackageItemOperationKind.Replace => (true, true),
                PackageItemOperationKind.Add => (false, true),
                _ => (false, false),
            };

            // 先备份
            if (moveToBackup)
            {
                string localFileName = operation.OldAsset.AssetName;
                context.Progress.Report(new(SH.FormatServiceGamePackageConvertMoveFileBackup(localFileName)));

                string localFilePath = context.GetGameFolderFilePath(localFileName);
                string cacheFilePath = context.GetServerCacheBackupFilePath(localFileName);
                string? cacheFileDirectory = Path.GetDirectoryName(cacheFilePath);
                ArgumentException.ThrowIfNullOrEmpty(cacheFileDirectory);
                Directory.CreateDirectory(cacheFileDirectory);

                logger.LogInformation("Backing file from:{Src} to:{Dst}", localFilePath, cacheFilePath);
                FileOperation.Move(localFilePath, cacheFilePath, true);
            }

            // 后替换
            if (moveToTarget)
            {
                string targetFileName = operation.NewAsset.AssetName;
                context.Progress.Report(new(SH.FormatServiceGamePackageConvertMoveFileRestore(targetFileName)));

                string targetFilePath = context.GetGameFolderFilePath(targetFileName);
                string? targetFileDirectory = Path.GetDirectoryName(targetFilePath);
                string cacheFilePath = context.GetServerCacheTargetFilePath(targetFileName);
                ArgumentException.ThrowIfNullOrEmpty(targetFileDirectory);
                Directory.CreateDirectory(targetFileDirectory);

                logger.LogInformation("Restoring file from:{Src} to:{Dst}", cacheFilePath, targetFilePath);
                FileOperation.Move(cacheFilePath, targetFilePath, true);
            }
        }

        // 重命名 _Data 目录
        try
        {
            context.Progress.Report(new(SH.FormatServiceGamePackageConvertMoveFileRename(context.FromDataFolderName, context.ToDataFolderName)));
            DirectoryOperation.Move(context.FromDataFolder, context.ToDataFolder);
        }
        catch (IOException ex)
        {
            // Access to the path is denied.
            // When user install the game in special folder like 'Program Files'
            throw HutaoException.Throw(SH.ServiceGamePackageRenameDataFolderFailed, ex);
        }

        return true;
    }
}