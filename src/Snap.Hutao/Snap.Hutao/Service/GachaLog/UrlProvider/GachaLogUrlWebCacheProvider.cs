// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Service.Game;
using System.IO;
using System.Text;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 浏览器缓存方法
/// </summary>
[Injection(InjectAs.Transient, typeof(IGachaLogUrlProvider))]
internal class GachaLogUrlWebCacheProvider : IGachaLogUrlProvider
{
    private readonly IGameService gameService;

    /// <summary>
    /// 构造一个新的浏览器缓存方法提供器
    /// </summary>
    /// <param name="gameService">游戏服务</param>
    public GachaLogUrlWebCacheProvider(IGameService gameService)
    {
        this.gameService = gameService;
    }

    /// <inheritdoc/>
    public string Name { get => nameof(GachaLogUrlWebCacheProvider); }

    /// <summary>
    /// 获取缓存文件路径
    /// </summary>
    /// <param name="path">游戏路径</param>
    /// <returns>缓存文件路径</returns>
    public static string GetCacheFile(string path)
    {
        string folder = Path.GetDirectoryName(path) ?? string.Empty;
        var cacheDataPath = Path.Combine(folder, @"YuanShen_Data\webCaches\Cache\Cache_Data\data_2");
        var cacheDataPathIntl = Path.Combine(folder, @"GenshinImpact_Data\webCaches\Cache\Cache_Data\data_2");
        if (File.Exists(cacheDataPath))
        {
            return cacheDataPath;
        }
        else
        {
            return cacheDataPathIntl;
        }
    }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, string>> GetQueryAsync()
    {
        (bool isOk, string path) = await gameService.GetGamePathAsync().ConfigureAwait(false);

        if (isOk)
        {
            string cacheFile = GetCacheFile(path);

            using (TempFile? tempFile = TempFile.CreateFromFileCopy(cacheFile))
            {
                if (tempFile == null)
                {
                    return new(false, $"找不到原神内置浏览器缓存路径：\n{cacheFile}");
                }

                using (FileStream fileStream = new(tempFile.Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (MemoryStream memoryStream = new())
                    {
                        await fileStream.CopyToAsync(memoryStream).ConfigureAwait(false);
                        string? result = null;

                        if (tempFile.Path.Contains("YuanShen_Data"))
                        {
                            result = Match(memoryStream);
                        }
                        else
                        {
                            result = MatchIntl(memoryStream);
                        }

                        return new(!string.IsNullOrEmpty(result), result!);
                    }
                }
            }
        }
        else
        {
            return new(false, "未正确提供原神路径，或当前设置的路径不正确");
        }
    }

    private static string? Match(MemoryStream stream)
    {
        ReadOnlySpan<byte> span = stream.ToArray();
        ReadOnlySpan<byte> match = "https://webstatic.mihoyo.com/hk4e/event/e20190909gacha-v2/index.html"u8;
        ReadOnlySpan<byte> zero = "\0"u8;

        int index = span.LastIndexOf(match);
        if (index >= 0)
        {
            int length = span[index..].IndexOf(zero);
            return Encoding.UTF8.GetString(span.Slice(index, length));
        }

        return null;
    }

    private static string? MatchIntl(MemoryStream stream)
    {
        ReadOnlySpan<byte> span = stream.ToArray();
        ReadOnlySpan<byte> match = "https://webstatic-sea.hoyoverse.com/genshin/event/e20190909gacha-v2/index.html"u8;
        ReadOnlySpan<byte> zero = "\0"u8;

        int index = span.LastIndexOf(match);
        if (index >= 0)
        {
            int length = span[index..].IndexOf(zero);
            return Encoding.UTF8.GetString(span.Slice(index, length));
        }

        return null;
    }
}
