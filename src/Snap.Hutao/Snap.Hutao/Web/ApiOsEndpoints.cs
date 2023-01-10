// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web;

/// <summary>
/// 国际服 API 端点
/// </summary>
[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1124")]
internal static class ApiOsEndpoints
{
    #region Hk4eApiOsGachaInfoApi

    /// <summary>
    /// 获取祈愿记录
    /// </summary>
    /// <param name="query">query string</param>
    /// <returns>祈愿记录信息Url</returns>
    public static string GachaInfoGetGachaLog(string query)
    {
        return $"{Hk4eApiOsGachaInfoApi}/getGachaLog?{query}";
    }
    #endregion

    #region Hosts | Queries
    private const string Hk4eApiOs = "https://hk4e-api-os.hoyoverse.com";
    private const string Hk4eApiOsGachaInfoApi = $"{Hk4eApiOs}/event/gacha_info/api";
    #endregion
}