// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Model.Binding.LaunchGame;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Service.Game.Package;

/// <summary>
/// 游戏文件包转换器
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class PackageConverter
{
    private const string GenshinImpactData = "GenshinImpact_Data";
    private const string YuanShenData = "YuanShen_Data";

    private readonly ResourceClient resourceClient;
    private readonly JsonSerializerOptions options;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 构造一个新的游戏文件转换器
    /// </summary>
    /// <param name="resourceClient">资源客户端</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="httpClient">http客户端</param>
    public PackageConverter(ResourceClient resourceClient, JsonSerializerOptions options, HttpClient httpClient)
    {
        this.resourceClient = resourceClient;
        this.options = options;
        this.httpClient = httpClient;
    }

    /// <summary>
    /// 异步替换游戏资源
    /// 调用前需要确认本地文件与服务器上的不同
    /// </summary>
    /// <param name="targetScheme">目标启动方案</param>
    /// <param name="gameFolder">游戏目录</param>
    /// <param name="progress">进度</param>
    /// <returns>任务</returns>
    public async Task<bool> ReplaceGameResourceAsync(LaunchScheme targetScheme, string gameFolder, IProgress<PackageReplaceStatus> progress)
    {
        await ThreadHelper.SwitchToBackgroundAsync();
        progress.Report(new("查询游戏资源信息"));
        Response<GameResource> response = await resourceClient.GetResourceAsync(targetScheme).ConfigureAwait(false);

        if (response.IsOk())
        {
            GameResource remoteGameResouce = response.Data;

            string scatteredFilesUrl = remoteGameResouce.Game.Latest.DecompressedPath;
            Uri pkgVersionUri = new($"{scatteredFilesUrl}/pkg_version");
            ConvertDirection direction = targetScheme.IsOversea ? ConvertDirection.ChineseToOversea : ConvertDirection.OverseaToChinese;

            progress.Report(new("下载包版本信息"));
            Dictionary<string, VersionItem> remoteItems;
            using (Stream remoteSteam = await httpClient.GetStreamAsync(pkgVersionUri).ConfigureAwait(false))
            {
                remoteItems = await GetVersionItemsAsync(remoteSteam).ConfigureAwait(false);
            }

            Dictionary<string, VersionItem> localItems;
            using (FileStream localSteam = File.OpenRead(Path.Combine(gameFolder, "pkg_version")))
            {
                localItems = await GetVersionItemsAsync(localSteam, direction, ConvertRemoteName).ConfigureAwait(false);
            }

            IEnumerable<ItemOperationInfo> diffOperations = GetItemOperationInfos(remoteItems, localItems);
            var a = diffOperations.ToList();
            await ReplaceGameResourceCoreAsync(diffOperations, gameFolder, scatteredFilesUrl, direction, progress).ConfigureAwait(false);
            return true;
        }

        return false;
    }

    private static string ConvertRemoteName(string remoteName, ConvertDirection direction)
    {
        // 我们已经提前重命名了整个 Data 文件夹 所以需要将 RemoteName 中的 Data 同样替换
        if (remoteName.StartsWith(YuanShenData) || remoteName.StartsWith(GenshinImpactData))
        {
            return direction switch
            {
                ConvertDirection.OverseaToChinese => $"{YuanShenData}{remoteName[GenshinImpactData.Length..]}",
                ConvertDirection.ChineseToOversea => $"{GenshinImpactData}{remoteName[YuanShenData.Length..]}",
                _ => remoteName,
            };
        }

        return remoteName;
    }

    private static IEnumerable<ItemOperationInfo> GetItemOperationInfos(Dictionary<string, VersionItem> remote, Dictionary<string, VersionItem> local)
    {
        foreach ((string remoteName, VersionItem remoteItem) in remote)
        {
            if (local.TryGetValue(remoteName, out VersionItem? localItem))
            {
                if (remoteItem.Md5 != localItem.Md5)
                {
                    // 本地发现了同名且不同MD5的项，需要替换为服务器上的项
                    yield return new(ItemOperationType.Replace, remoteItem, localItem);
                }

                local.Remove(remoteName);
            }
            else
            {
                // 本地没有发现同名项
                yield return new(ItemOperationType.Add, remoteItem, remoteItem);
            }
        }

        IEnumerable<ItemOperationInfo> removes = local.Select(kvp => new ItemOperationInfo(ItemOperationType.Remove, kvp.Value, kvp.Value));

        foreach (ItemOperationInfo item in removes)
        {
            yield return item;
        }
    }

    private static void RenameDataFolder(string gameFolder, ConvertDirection direction)
    {
        string yuanShenData = Path.Combine(gameFolder, YuanShenData);
        string genshinImpactData = Path.Combine(gameFolder, GenshinImpactData);

        // We have check the exe path previously
        // so we assume the data folder is present
        if (direction == ConvertDirection.ChineseToOversea)
        {
            Directory.Move(yuanShenData, genshinImpactData);
        }
        else
        {
            Directory.Move(genshinImpactData, yuanShenData);
        }
    }

    private static void MoveToCache(string cacheFolder, string cacheName, string targetFullPath)
    {
        string cacheFilePath = Path.Combine(cacheFolder, cacheName);
        Directory.CreateDirectory(Path.GetDirectoryName(cacheFilePath)!);
        File.Move(targetFullPath, cacheFilePath, true);
    }

    private async Task ReplaceGameResourceCoreAsync(IEnumerable<ItemOperationInfo> operations, string gameFolder, string scatteredFilesUrl, ConvertDirection direction, IProgress<PackageReplaceStatus> progress)
    {
        // 重命名 _Data 目录
        RenameDataFolder(gameFolder, direction);

        // Ensure cache folder
        string cacheFolder = Path.Combine(gameFolder, "Screenshot", "HutaoCache");

        // 执行下载与移动操作
        foreach (ItemOperationInfo info in operations)
        {
            progress.Report(new($"{info.Target}"));

            string targetFilePath = Path.Combine(gameFolder, info.Target);
            string cacheFilePath = Path.Combine(cacheFolder, info.Cache);

            switch (info.Type)
            {
                case ItemOperationType.Add:
                    await ReplaceFromCacheOrWebAsync(cacheFilePath, targetFilePath, scatteredFilesUrl, info).ConfigureAwait(false);
                    break;
                case ItemOperationType.Replace:
                    {
                        MoveToCache(cacheFolder, info.Cache, targetFilePath);
                        await ReplaceFromCacheOrWebAsync(cacheFilePath, targetFilePath, scatteredFilesUrl, info).ConfigureAwait(false);

                        break;
                    }

                case ItemOperationType.Remove:
                    MoveToCache(cacheFolder, info.Cache, targetFilePath);
                    break;

                default:
                    break;
            }
        }

        // 重新下载所有 *pkg_version 文件
        await ReplacePackageVersionsAsync(scatteredFilesUrl, gameFolder).ConfigureAwait(false);
    }

    private async Task ReplaceFromCacheOrWebAsync(string cacheFilePath, string targetFilePath, string scatteredFilesUrl, ItemOperationInfo info)
    {
        if (File.Exists(cacheFilePath))
        {
            string remoteMd5 = await FileDigest.GetMd5Async(cacheFilePath, CancellationToken.None).ConfigureAwait(false);
            if (info.Md5 == remoteMd5.ToLowerInvariant() && new FileInfo(cacheFilePath).Length == info.TotalBytes)
            {
                // Valid, move it to target path
                // There shouldn't be any file in the same name
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
        using (FileStream fileStream = File.Create(targetFilePath))
        {
            using (Stream webStream = await httpClient.GetStreamAsync($"{scatteredFilesUrl}/{info.Target}").ConfigureAwait(false))
            {
                await webStream.CopyToAsync(fileStream).ConfigureAwait(false);
            }
        }
    }

    private async Task ReplacePackageVersionsAsync(string scatteredFilesUrl, string gameFolder)
    {
        foreach (string audioPkgVersionFilePath in Directory.EnumerateFiles(gameFolder, "*pkg_version"))
        {
            string audioPkgVersionFileName = Path.GetFileName(audioPkgVersionFilePath);
            using (FileStream audioPkgVersionFileStream = File.Create(audioPkgVersionFilePath))
            {
                using (Stream webStream = await httpClient.GetStreamAsync($"{scatteredFilesUrl}/{audioPkgVersionFileName}").ConfigureAwait(false))
                {
                    await webStream.CopyToAsync(audioPkgVersionFileStream).ConfigureAwait(false);
                }
            }
        }
    }

    private async Task<Dictionary<string, VersionItem>> GetVersionItemsAsync(Stream stream)
    {
        Dictionary<string, VersionItem> results = new();
        using (StreamReader reader = new(stream))
        {
            while (await reader.ReadLineAsync().ConfigureAwait(false) is string raw)
            {
                if (!string.IsNullOrEmpty(raw))
                {
                    VersionItem item = JsonSerializer.Deserialize<VersionItem>(raw, options)!;
                    results.Add(item.RemoteName, item);
                }
            }
        }

        return results;
    }

    private async Task<Dictionary<string, VersionItem>> GetVersionItemsAsync(Stream stream, ConvertDirection direction, Func<string, ConvertDirection, string> nameConverter)
    {
        Dictionary<string, VersionItem> results = new();
        using (StreamReader reader = new(stream))
        {
            while (await reader.ReadLineAsync().ConfigureAwait(false) is string raw)
            {
                if (!string.IsNullOrEmpty(raw))
                {
                    VersionItem item = JsonSerializer.Deserialize<VersionItem>(raw, options)!;
                    results.Add(nameConverter(item.RemoteName, direction), item);
                }
            }
        }

        return results;
    }
}