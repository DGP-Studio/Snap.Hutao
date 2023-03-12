// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Web;

/// <summary>
/// 国际服 API 端点
/// </summary>
[HighQuality]
[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1124")]
internal static class ApiOsEndpoints
{
    #region ApiOsTaKumiApi

    /// <summary>
    /// 用户游戏角色
    /// </summary>
    /// <returns>用户游戏角色字符串</returns>
    public const string UserGameRolesByCookie = $"{ApiOsTaKumiBindingApi}/getUserGameRolesByCookie?game_biz=hk4e_global";

    #endregion

    #region BbsApiOsApi

    /// <summary>
    /// 查询其他用户详细信息
    /// </summary>
    /// <param name="bbsUid">bbs Uid</param>
    /// <returns>查询其他用户详细信息字符串</returns>
    public static string UserFullInfoQuery(string bbsUid)
    {
        return $"{BbsApiOs}/community/painter/wapi/user/full";
    }

    /// <summary>
    /// 国际服角色基本信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>角色基本信息字符串</returns>
    public static string GameRecordRoleBasicInfo(PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordApi}/roleBasicInfo?role_id={uid.Value}&server={uid.Region}";
    }

    /// <summary>
    /// 国际服角色信息
    /// </summary>
    public const string GameRecordCharacter = $"{BbsApiOsGameRecordApi}/character";

    /// <summary>
    /// 国际服游戏记录实时便笺
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>游戏记录实时便笺字符串</returns>
    public static string GameRecordDailyNote(PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordApi}/dailyNote?server={uid.Region}&role_id={uid.Value}";
    }

    /// <summary>
    /// 国际服游戏记录主页
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>游戏记录主页字符串</returns>
    public static string GameRecordIndex(PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordApi}/index?server={uid.Region}&role_id={uid.Value}";
    }

    /// <summary>
    /// 国际服深渊信息
    /// </summary>
    /// <param name="scheduleType">深渊类型</param>
    /// <param name="uid">Uid</param>
    /// <returns>深渊信息字符串</returns>
    public static string GameRecordSpiralAbyss(Hoyolab.Takumi.GameRecord.SpiralAbyssSchedule scheduleType, PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordApi}/spiralAbyss?schedule_type={(int)scheduleType}&role_id={uid.Value}&server={uid.Region}";
    }

    #endregion

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

    #region SdkStaticLauncherApi

    /// <summary>
    /// 启动器资源
    /// </summary>
    /// <param name="scheme">启动方案</param>
    /// <returns>启动器资源字符串</returns>
    public static string SdkOsStaticLauncherResource(LaunchScheme scheme)
    {
        return $"{SdkOsStaticLauncherApi}/resource?key={scheme.Key}&launcher_id={scheme.LauncherId}&channel_id={scheme.Channel}&sub_channel_id={scheme.SubChannel}";
    }
    #endregion

    #region Hosts | Queries
    private const string ApiOsTaKumi = "https://api-os-takumi.hoyoverse.com";
    private const string ApiOsTaKumiBindingApi = $"{ApiOsTaKumi}/binding/api";

    private const string BbsApiOs = "https://bbs-api-os.hoyolab.com";
    private const string BbsApiOsGameRecordApi = $"{BbsApiOs}/game_record/genshin/api";

    private const string Hk4eApiOs = "https://hk4e-api-os.hoyoverse.com";
    private const string Hk4eApiOsGachaInfoApi = $"{Hk4eApiOs}/event/gacha_info/api";

    private const string SdkOsStatic = "https://sdk-os-static.mihoyo.com";
    private const string SdkOsStaticLauncherApi = $"{SdkOsStatic}/hk4e_global/mdk/launcher/api";
    #endregion
}