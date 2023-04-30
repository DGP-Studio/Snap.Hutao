// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO;
using Snap.Hutao.Service.Game;
using System.IO;
using System.Text;

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

/// <summary>
/// 浏览器缓存方法
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient, typeof(IGachaLogQueryProvider))]
internal sealed class GachaLogQueryWebCacheProvider : IGachaLogQueryProvider
{
    private readonly IGameService gameService;

    /// <summary>
    /// 构造一个新的浏览器缓存方法提供器
    /// </summary>
    /// <param name="gameService">游戏服务</param>
    public GachaLogQueryWebCacheProvider(IGameService gameService)
    {
        this.gameService = gameService;
    }

    /// <inheritdoc/>
    public string Name { get => nameof(GachaLogQueryWebCacheProvider); }

    /// <summary>
    /// 获取缓存文件路径
    /// </summary>
    /// <param name="path">游戏路径</param>
    /// <returns>缓存文件路径</returns>
    public static string GetCacheFile(string path)
    {
        string exeName = Path.GetFileName(path);
        string dataFolder = exeName == GameConstants.GenshinImpactFileName
            ? GameConstants.GenshinImpactData
            : GameConstants.YuanShenData;

        return Path.Combine(Path.GetDirectoryName(path)!, dataFolder, @"webCaches\Cache\Cache_Data\data_2");
    }

    /// <inheritdoc/>
    public async Task<ValueResult<bool, GachaLogQuery>> GetQueryAsync()
    {
        (bool isOk, string path) = await gameService.GetGamePathAsync().ConfigureAwait(false);

        if (isOk && (!string.IsNullOrEmpty(path)))
        {
            string cacheFile = GetCacheFile(path);

            using (TempFile? tempFile = TempFile.CopyFrom(cacheFile))
            {
                if (tempFile == null)
                {
                    return new(false, string.Format(SH.ServiceGachaLogUrlProviderCachePathNotFound, cacheFile));
                }

                using (FileStream fileStream = new(tempFile.Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (MemoryStream memoryStream = new())
                    {
                        await fileStream.CopyToAsync(memoryStream).ConfigureAwait(false);
                        string? result = Match(memoryStream, cacheFile.Contains(GameConstants.GenshinImpactData));

                        if (!string.IsNullOrEmpty(result))
                        {
                            return new(true, new(result));
                        }
                        else
                        {
                            return new(false, SH.ServiceGachaLogUrlProviderCacheUrlNotFound);
                        }
                    }
                }
            }
        }
        else
        {
            return new(false, SH.ServiceGachaLogUrlProviderCachePathInvalid);
        }
    }

    private static string? Match(MemoryStream stream, bool isOversea)
    {
        ReadOnlySpan<byte> span = stream.ToArray();
        ReadOnlySpan<byte> match = isOversea
            ? "https://webstatic-sea.hoyoverse.com/genshin/event/e20190909gacha-v2/index.html"u8
            : "https://webstatic.mihoyo.com/hk4e/event/e20190909gacha-v2/index.html"u8;

        int index = span.LastIndexOf(match);
        if (index >= 0)
        {
            int length = span[index..].IndexOf("\0"u8);
            return Encoding.UTF8.GetString(span.Slice(index, length));
        }

        return null;
    }
}