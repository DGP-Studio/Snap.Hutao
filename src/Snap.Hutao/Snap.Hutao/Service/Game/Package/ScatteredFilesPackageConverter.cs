// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Core.IO.Http.Sharding;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.RegularExpressions;
using static Snap.Hutao.Service.Game.GameConstants;
using RelativePathVersionItemDictionary = System.Collections.Generic.Dictionary<string, Snap.Hutao.Service.Game.Package.VersionItem>;

namespace Snap.Hutao.Service.Game.Package;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IPackageConverter), Key = PackageConverterMode.ScatteredFiles)]
internal sealed partial class ScatteredFilesPackageConverter : IPackageConverter
{
    private const string PackageVersion = "pkg_version";

    private readonly ILogger<ScatteredFilesPackageConverter> logger;
    private readonly JsonSerializerOptions options;

    public async ValueTask<bool> EnsureGameResourceAsync(PackageConverterContext context)
    {
        // 以 国服 -> 国际服 为例
        // 1. 下载国际服的 pkg_version 文件，转换为索引字典
        //    获取本地对应 pkg_version 文件，转换为索引字典
        //
        // 2. 对比两者差异，
        //    国际服有 & 国服没有的 为新增
        //    国际服有 & 国服也有的 为替换
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
        ArgumentNullException.ThrowIfNull(context.TargetPackage);

        // Step 1
        context.Progress.Report(new(SH.ServiceGamePackageRequestPackageVerion));
        RelativePathVersionItemDictionary remoteItems = await GetRemoteItemsAsync(context).ConfigureAwait(false);
        RelativePathVersionItemDictionary localItems = await GetLocalItemsAsync(context.GameFileSystem.GameDirectory).ConfigureAwait(false);

        // Step 2
        List<PackageItemOperationInfo> diffOperations = GetItemOperationInfos(remoteItems, localItems).ToList();
        diffOperations.SortBy(i => i.Kind);

        // Step 3
        await PrepareCacheFilesAsync(context, diffOperations).ConfigureAwait(false);

        // Step 4
        return await ReplaceGameResourceAsync(context, diffOperations).ConfigureAwait(false);
    }

    public async ValueTask EnsureDeprecatedFilesAndSdkAsync(PackageConverterContext context)
    {
        // Just try to delete these files, always download from server when needed
        FileOperation.Delete(Path.Combine(context.GameFileSystem.GameDirectory, YuanShenData, "Plugins\\PCGameSDK.dll"));
        FileOperation.Delete(Path.Combine(context.GameFileSystem.GameDirectory, GenshinImpactData, "Plugins\\PCGameSDK.dll"));
        FileOperation.Delete(Path.Combine(context.GameFileSystem.GameDirectory, YuanShenData, "Plugins\\EOSSDK-Win64-Shipping.dll"));
        FileOperation.Delete(Path.Combine(context.GameFileSystem.GameDirectory, GenshinImpactData, "Plugins\\EOSSDK-Win64-Shipping.dll"));
        FileOperation.Delete(Path.Combine(context.GameFileSystem.GameDirectory, YuanShenData, "Plugins\\PluginEOSSDK.dll"));
        FileOperation.Delete(Path.Combine(context.GameFileSystem.GameDirectory, GenshinImpactData, "Plugins\\PluginEOSSDK.dll"));
        FileOperation.Delete(Path.Combine(context.GameFileSystem.GameDirectory, "sdk_pkg_version"));

        if (context.GameChannelSDK is not null)
        {
            using (Stream sdkWebStream = await context.HttpClient.GetStreamAsync(context.GameChannelSDK.ChannelSdkPackage.Url).ConfigureAwait(false))
            {
                ZipFile.ExtractToDirectory(sdkWebStream, context.GameFileSystem.GameDirectory, true);
            }
        }

        if (context.DeprecatedFiles is not null)
        {
            foreach (DeprecatedFile file in context.DeprecatedFiles.DeprecatedFiles)
            {
                string filePath = Path.Combine(context.GameFileSystem.GameDirectory, file.Name);
                FileOperation.Move(filePath, $"{filePath}.backup", true);
            }
        }
    }

    private static IEnumerable<PackageItemOperationInfo> GetItemOperationInfos(RelativePathVersionItemDictionary remote, RelativePathVersionItemDictionary local)
    {
        foreach ((string remoteName, VersionItem remoteItem) in remote)
        {
            if (local.TryGetValue(remoteName, out VersionItem? localItem))
            {
                if (!(remoteItem.FileSize == localItem.FileSize && remoteItem.Md5.Equals(localItem.Md5, StringComparison.OrdinalIgnoreCase)))
                {
                    // 本地发现了同名且不同 MD5 的项，需要替换为服务器上的项
                    yield return new(PackageItemOperationKind.Replace, remoteItem, localItem);
                }

                // 同名同MD5，跳过
                local.Remove(remoteName);
            }
            else
            {
                // 本地没有发现同名项
                yield return new(PackageItemOperationKind.Add, remoteItem, remoteItem);
            }
        }

        foreach ((_, VersionItem localItem) in local)
        {
            yield return new(PackageItemOperationKind.Backup, localItem, localItem);
        }
    }

    [GeneratedRegex("^(?:YuanShen_Data|GenshinImpact_Data)(?=/)")]
    private static partial Regex DataFolderRegex();

    private async ValueTask<RelativePathVersionItemDictionary> GetVersionItemsAsync(Stream stream)
    {
        RelativePathVersionItemDictionary results = [];
        using (StreamReader reader = new(stream))
        {
            while (await reader.ReadLineAsync().ConfigureAwait(false) is { Length: > 0 } row)
            {
                VersionItem? item = JsonSerializer.Deserialize<VersionItem>(row, options);
                ArgumentNullException.ThrowIfNull(item);
                item.RelativePath = DataFolderRegex().Replace(item.RelativePath, "{0}");
                results.Add(item.RelativePath, item);
            }
        }

        return results;
    }

    private async ValueTask<RelativePathVersionItemDictionary> GetRemoteItemsAsync(PackageConverterContext context)
    {
        try
        {
            // Server might close the connection shortly,
            // we have to cache the content immediately.
            using (HttpResponseMessage responseMessage = await context.HttpClient.GetAsync(context.PkgVersionUrl, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
            {
                using (Stream remoteSteam = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    return await GetVersionItemsAsync(remoteSteam).ConfigureAwait(false);
                }
            }
        }
        catch (IOException ex)
        {
            throw HutaoException.Throw(SH.ServiceGamePackageRequestPackageVerionFailed, ex);
        }
    }

    private async ValueTask<RelativePathVersionItemDictionary> GetLocalItemsAsync(string gameFolder)
    {
        try
        {
            using (FileStream localSteam = File.OpenRead(Path.Combine(gameFolder, PackageVersion)))
            {
                return await GetVersionItemsAsync(localSteam).ConfigureAwait(false);
            }
        }
        catch (JsonException ex)
        {
            throw HutaoException.Throw(SH.ServiceGamePackageReadLocalPackageVerionFailed, ex);
        }
    }

    private async ValueTask PrepareCacheFilesAsync(PackageConverterContext context, List<PackageItemOperationInfo> operations)
    {
        foreach (PackageItemOperationInfo info in operations)
        {
            switch (info.Kind)
            {
                case PackageItemOperationKind.Backup:
                    continue;
                case PackageItemOperationKind.Replace:
                case PackageItemOperationKind.Add:
                    await SkipOrDownloadAsync(context, info).ConfigureAwait(false);
                    break;
            }
        }
    }

    private async ValueTask SkipOrDownloadAsync(PackageConverterContext context, PackageItemOperationInfo info)
    {
        // 还原正确的远程地址
        string remoteName = string.Format(CultureInfo.CurrentCulture, info.Remote.RelativePath, context.ToDataFolderName);
        string cacheFile = context.GetServerCacheTargetFilePath(remoteName);

        if (File.Exists(cacheFile))
        {
            if (info.Remote.FileSize == new FileInfo(cacheFile).Length)
            {
                if (info.Remote.Md5.Equals(await MD5.HashFileAsync(cacheFile).ConfigureAwait(false), StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            // Invalid file, delete it
            File.Delete(cacheFile);
        }

        // Cache no matching item, download
        string? directory = Path.GetDirectoryName(cacheFile);
        ArgumentException.ThrowIfNullOrEmpty(directory);
        Directory.CreateDirectory(directory);

        string remoteUrl = context.GetScatteredFilesUrl(remoteName);
        HttpShardCopyWorkerOptions<PackageConvertStatus> options = new()
        {
            HttpClient = context.HttpClient,
            SourceUrl = remoteUrl,
            DestinationFilePath = cacheFile,
            StatusFactory = (bytesRead, totalBytes) => new(remoteName, bytesRead, totalBytes),
        };

        using (IHttpShardCopyWorker<PackageConvertStatus> worker = await HttpShardCopyWorker.CreateAsync(options).ConfigureAwait(false))
        {
            try
            {
                await worker.CopyAsync(context.Progress).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // System.IO.IOException: The response ended prematurely.
                // System.IO.IOException: Received an unexpected EOF or 0 bytes from the transport stream.
                HutaoException.Throw(SH.FormatServiceGamePackageRequestScatteredFileFailed(remoteName), ex);
            }
        }

        if (!string.Equals(info.Remote.Md5, await MD5.HashFileAsync(cacheFile).ConfigureAwait(false), StringComparison.OrdinalIgnoreCase))
        {
            HutaoException.Throw(SH.FormatServiceGamePackageRequestScatteredFileFailed(remoteName));
        }
    }

    private async ValueTask<bool> ReplaceGameResourceAsync(PackageConverterContext context, List<PackageItemOperationInfo> operations)
    {
        // 执行下载与移动操作
        foreach (PackageItemOperationInfo info in operations)
        {
            (bool moveToBackup, bool moveToTarget) = info.Kind switch
            {
                PackageItemOperationKind.Backup => (true, false),
                PackageItemOperationKind.Replace => (true, true),
                PackageItemOperationKind.Add => (false, true),
                _ => (false, false),
            };

            // 先备份
            if (moveToBackup)
            {
                string localFileName = string.Format(CultureInfo.CurrentCulture, info.Local.RelativePath, context.FromDataFolderName);
                context.Progress.Report(new(SH.FormatServiceGamePackageConvertMoveFileBackupFormat(localFileName)));

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
                string targetFileName = string.Format(CultureInfo.CurrentCulture, info.Remote.RelativePath, context.ToDataFolderName);
                context.Progress.Report(new(SH.FormatServiceGamePackageConvertMoveFileRestoreFormat(targetFileName)));

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
            context.Progress.Report(new(SH.FormatServiceGamePackageConvertMoveFileRenameFormat(context.FromDataFolderName, context.ToDataFolderName)));
            DirectoryOperation.Move(context.FromDataFolder, context.ToDataFolder);
        }
        catch (IOException ex)
        {
            // Access to the path is denied.
            // When user install the game in special folder like 'Program Files'
            throw HutaoException.Throw(SH.ServiceGamePackageRenameDataFolderFailed, ex);
        }

        // 重新下载所有 *pkg_version 文件
        await ReplacePackageVersionFilesAsync(context).ConfigureAwait(false);
        return true;
    }

    private async ValueTask ReplacePackageVersionFilesAsync(PackageConverterContext context)
    {
        foreach (string versionFilePath in Directory.EnumerateFiles(context.GameFileSystem.GameDirectory, "*pkg_version"))
        {
            string versionFileName = Path.GetFileName(versionFilePath);

            if (string.Equals(versionFileName, "sdk_pkg_version", StringComparison.OrdinalIgnoreCase))
            {
                // Skipping the sdk_pkg_version file,
                // it can't be claimed from remote.
                continue;
            }

            using (FileStream versionFileStream = File.Create(versionFilePath))
            {
                using (Stream webStream = await context.HttpClient.GetStreamAsync(context.GetScatteredFilesUrl(versionFileName)).ConfigureAwait(false))
                {
                    await webStream.CopyToAsync(versionFileStream).ConfigureAwait(false);
                }
            }
        }
    }
}