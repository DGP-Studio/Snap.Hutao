// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.IO.Hashing;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using static Snap.Hutao.Service.Game.GameConstants;

namespace Snap.Hutao.Service.Game.Package;

/// <summary>
/// 游戏文件包转换器
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class PackageConverter
{
    private const string PackageVersion = "pkg_version";

    private readonly IServiceProvider serviceProvider;
    private readonly JsonSerializerOptions options;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步检查替换游戏资源
    /// 调用前需要确认本地文件与服务器上的不同
    /// </summary>
    /// <param name="targetScheme">目标启动方案</param>
    /// <param name="gameResource">游戏资源</param>
    /// <param name="gameFolder">游戏目录</param>
    /// <param name="progress">进度</param>
    /// <returns>替换结果与资源</returns>
    public async ValueTask<bool> EnsureGameResourceAsync(LaunchScheme targetScheme, GameResource gameResource, string gameFolder, IProgress<PackageReplaceStatus> progress)
    {
        // 以 国服 => 国际 为例
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
        string scatteredFilesUrl = gameResource.Game.Latest.DecompressedPath;
        Uri pkgVersionUri = $"{scatteredFilesUrl}/{PackageVersion}".ToUri();
        ConvertDirection direction = targetScheme.IsOversea ? ConvertDirection.ChineseToOversea : ConvertDirection.OverseaToChinese;

        progress.Report(new(SH.ServiceGamePackageRequestPackageVerion));
        Dictionary<string, VersionItem> remoteItems = await GetRemoteItemsAsync(pkgVersionUri).ConfigureAwait(false);
        Dictionary<string, VersionItem> localItems = await GetLocalItemsAsync(gameFolder, direction).ConfigureAwait(false);

        IEnumerable<ItemOperationInfo> diffOperations = GetItemOperationInfos(remoteItems, localItems).OrderBy(i => i.Type);
        return await ReplaceGameResourceAsync(diffOperations, gameFolder, scatteredFilesUrl, direction, progress).ConfigureAwait(false);
    }

    /// <summary>
    /// 检查过时文件与Sdk
    /// 只在国服环境有效
    /// </summary>
    /// <param name="resource">游戏资源</param>
    /// <param name="gameFolder">游戏文件夹</param>
    /// <returns>任务</returns>
    public async ValueTask EnsureDeprecatedFilesAndSdkAsync(GameResource resource, string gameFolder)
    {
        string sdkDllBackup = Path.Combine(gameFolder, YuanShenData, "Plugins\\PCGameSDK.dll.backup");
        string sdkDll = Path.Combine(gameFolder, YuanShenData, "Plugins\\PCGameSDK.dll");

        string sdkVersionBackup = Path.Combine(gameFolder, "sdk_pkg_version.backup");
        string sdkVersion = Path.Combine(gameFolder, "sdk_pkg_version");

        // Only bilibili's sdk is not null
        if (resource.Sdk != null)
        {
            // TODO: verify sdk md5
            if (File.Exists(sdkDllBackup) && File.Exists(sdkVersionBackup))
            {
                FileOperation.Move(sdkDllBackup, sdkDll, false);
                FileOperation.Move(sdkVersionBackup, sdkVersion, false);
            }
            else
            {
                using (Stream sdkWebStream = await httpClient.GetStreamAsync(resource.Sdk.Path).ConfigureAwait(false))
                {
                    using (ZipArchive zip = new(sdkWebStream))
                    {
                        foreach (ZipArchiveEntry entry in zip.Entries)
                        {
                            // skip folder entry.
                            if (entry.Length != 0)
                            {
                                string targetPath = Path.Combine(gameFolder, entry.FullName);
                                Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                                entry.ExtractToFile(targetPath, true);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            // backup
            FileOperation.Move(sdkDll, sdkDllBackup, true);
            FileOperation.Move(sdkVersion, sdkVersionBackup, true);
        }

        if (resource.DeprecatedFiles != null)
        {
            foreach (NameMd5 file in resource.DeprecatedFiles)
            {
                string filePath = Path.Combine(gameFolder, file.Name);
                FileOperation.Move(filePath, $"{filePath}.backup", true);
            }
        }
    }

    private static IEnumerable<ItemOperationInfo> GetItemOperationInfos(Dictionary<string, VersionItem> remote, Dictionary<string, VersionItem> local)
    {
        foreach ((string remoteName, VersionItem remoteItem) in remote)
        {
            if (local.TryGetValue(remoteName, out VersionItem? localItem))
            {
                if (!remoteItem.Md5.Equals(localItem.Md5, StringComparison.OrdinalIgnoreCase))
                {
                    // 本地发现了同名且不同 MD5 的项，需要替换为服务器上的项
                    yield return new(ItemOperationType.Replace, remoteItem, localItem);
                }

                // 同名同MD5，跳过
                local.Remove(remoteName);
            }
            else
            {
                // 本地没有发现同名项
                yield return new(ItemOperationType.Add, remoteItem, remoteItem);
            }
        }

        foreach ((_, VersionItem localItem) in local)
        {
            yield return new(ItemOperationType.Backup, localItem, localItem);
        }
    }

    private static void TryRenameDataFolder(string gameFolder, ConvertDirection direction)
    {
        string yuanShenData = Path.Combine(gameFolder, YuanShenData);
        string genshinImpactData = Path.Combine(gameFolder, GenshinImpactData);

        try
        {
            _ = direction == ConvertDirection.ChineseToOversea
                ? DirectoryOperation.Move(yuanShenData, genshinImpactData)
                : DirectoryOperation.Move(genshinImpactData, yuanShenData);
        }
        catch (IOException ex)
        {
            // Access to the path is denied.
            // When user install the game in special folder like 'Program Files'
            throw ThrowHelper.GameFileOperation(SH.ServiceGamePackageRenameDataFolderFailed, ex);
        }
    }

    private static void MoveToCache(string cacheFilePath, string targetFullPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(cacheFilePath)!);
        File.Move(targetFullPath, cacheFilePath, true);
    }

    private async ValueTask<Dictionary<string, VersionItem>> GetLocalItemsAsync(string gameFolder, ConvertDirection direction)
    {
        using (FileStream localSteam = File.OpenRead(Path.Combine(gameFolder, PackageVersion)))
        {
            return await GetLocalVersionItemsAsync(localSteam, direction).ConfigureAwait(false);
        }
    }

    private async ValueTask<Dictionary<string, VersionItem>> GetRemoteItemsAsync(Uri pkgVersionUri)
    {
        try
        {
            using (Stream remoteSteam = await httpClient.GetStreamAsync(pkgVersionUri).ConfigureAwait(false))
            {
                return await GetRemoteVersionItemsAsync(remoteSteam).ConfigureAwait(false);
            }
        }
        catch (IOException ex)
        {
            throw ThrowHelper.PackageConvert(SH.ServiceGamePackageRequestPackageVerionFailed, ex);
        }
    }

    private async ValueTask<bool> ReplaceGameResourceAsync(IEnumerable<ItemOperationInfo> operations, string gameFolder, string scatteredFilesUrl, ConvertDirection direction, IProgress<PackageReplaceStatus> progress)
    {
        // 重命名 _Data 目录
        TryRenameDataFolder(gameFolder, direction);

        // Cache folder
        Core.RuntimeOptions runtimeOptions = serviceProvider.GetRequiredService<Core.RuntimeOptions>();
        string cacheFolder = Path.Combine(runtimeOptions.DataFolder, "ServerCache");

        // 执行下载与移动操作
        foreach (ItemOperationInfo info in operations)
        {
            progress.Report(new($"{info.Target}"));

            string targetFilePath = Path.Combine(gameFolder, info.Target);
            string cacheFilePath = Path.Combine(cacheFolder, info.Target);
            string moveToFilePath = Path.Combine(cacheFolder, info.MoveTo);

            switch (info.Type)
            {
                case ItemOperationType.Backup:
                    MoveToCache(moveToFilePath, targetFilePath);
                    break;
                case ItemOperationType.Replace:
                    MoveToCache(moveToFilePath, targetFilePath);
                    await ReplaceFromCacheOrWebAsync(cacheFilePath, targetFilePath, scatteredFilesUrl, info, progress).ConfigureAwait(false);
                    break;
                case ItemOperationType.Add:
                    await ReplaceFromCacheOrWebAsync(cacheFilePath, targetFilePath, scatteredFilesUrl, info, progress).ConfigureAwait(false);
                    break;
                default:
                    break;
            }
        }

        // 重新下载所有 *pkg_version 文件
        await ReplacePackageVersionFilesAsync(scatteredFilesUrl, gameFolder).ConfigureAwait(false);
        return true;
    }

    private async ValueTask ReplaceFromCacheOrWebAsync(string cacheFilePath, string targetFilePath, string scatteredFilesUrl, ItemOperationInfo info, IProgress<PackageReplaceStatus> progress)
    {
        if (File.Exists(cacheFilePath))
        {
            string remoteMd5 = await MD5.HashFileAsync(cacheFilePath).ConfigureAwait(false);
            if (info.Md5 == remoteMd5.ToLowerInvariant() && new FileInfo(cacheFilePath).Length == info.TotalBytes)
            {
                // Valid, move it to target path
                // There shouldn't be any file in the path/name
                File.Move(cacheFilePath, targetFilePath, false);
                return;
            }
            else
            {
                // Invalid file, delete it
                File.Delete(cacheFilePath);
            }
        }

        // Cache no item, download it anyway.
        while (true)
        {
            using (FileStream fileStream = File.Create(targetFilePath))
            {
                using (HttpResponseMessage response = await httpClient.GetAsync($"{scatteredFilesUrl}/{info.Target}", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    long totalBytes = response.Content.Headers.ContentLength ?? 0;
                    using (Stream webStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        try
                        {
                            StreamCopyWorker<PackageReplaceStatus> streamCopyWorker = new(webStream, fileStream, bytesRead => new(info.Target, bytesRead, totalBytes));
                            await streamCopyWorker.CopyAsync(progress).ConfigureAwait(false);
                            fileStream.Position = 0;
                            string remoteMd5 = await MD5.HashAsync(fileStream).ConfigureAwait(false);
                            if (string.Equals(info.Md5, remoteMd5, StringComparison.OrdinalIgnoreCase))
                            {
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            // System.IO.IOException: The response ended prematurely.
                            // System.IO.IOException: Received an unexpected EOF or 0 bytes from the transport stream.

                            // We want to retry forever.
                            serviceProvider.GetRequiredService<IInfoBarService>().Error(ex);
                            await Delay.FromSeconds(2).ConfigureAwait(false);
                        }
                    }
                }
            }
        }
    }

    private async ValueTask ReplacePackageVersionFilesAsync(string scatteredFilesUrl, string gameFolder)
    {
        foreach (string versionFilePath in Directory.EnumerateFiles(gameFolder, "*pkg_version"))
        {
            string versionFileName = Path.GetFileName(versionFilePath);

            if (versionFileName == "sdk_pkg_version")
            {
                // Skipping the sdk_pkg_version file,
                // it can't be claimed from remote.
                continue;
            }

            using (FileStream versionFileStream = File.Create(versionFilePath))
            {
                using (Stream webStream = await httpClient.GetStreamAsync($"{scatteredFilesUrl}/{versionFileName}").ConfigureAwait(false))
                {
                    await webStream.CopyToAsync(versionFileStream).ConfigureAwait(false);
                }
            }
        }
    }

    private async ValueTask<Dictionary<string, VersionItem>> GetRemoteVersionItemsAsync(Stream stream)
    {
        Dictionary<string, VersionItem> results = new();
        using (StreamReader reader = new(stream))
        {
            while (await reader.ReadLineAsync().ConfigureAwait(false) is { } raw)
            {
                if (string.IsNullOrEmpty(raw))
                {
                    continue;
                }

                VersionItem item = JsonSerializer.Deserialize<VersionItem>(raw, options)!;
                results.Add(item.RemoteName, item);
            }
        }

        return results;
    }

    private async ValueTask<Dictionary<string, VersionItem>> GetLocalVersionItemsAsync(Stream stream, ConvertDirection direction)
    {
        Dictionary<string, VersionItem> results = new();

        using (StreamReader reader = new(stream))
        {
            while (await reader.ReadLineAsync().ConfigureAwait(false) is { } row)
            {
                if (string.IsNullOrEmpty(row))
                {
                    continue;
                }

                VersionItem item = JsonSerializer.Deserialize<VersionItem>(row, options)!;

                string remoteName = item.RemoteName;

                // 我们已经提前重命名了整个 Data 文件夹 所以需要将 RemoteName 中的 Data 同样替换
                if (remoteName.StartsWith(YuanShenData) || remoteName.StartsWith(GenshinImpactData))
                {
                    remoteName = direction switch
                    {
                        ConvertDirection.OverseaToChinese => $"{YuanShenData}{remoteName[GenshinImpactData.Length..]}",
                        ConvertDirection.ChineseToOversea => $"{GenshinImpactData}{remoteName[YuanShenData.Length..]}",
                        _ => remoteName,
                    };
                }

                results.Add(remoteName, item);
            }
        }

        return results;
    }
}