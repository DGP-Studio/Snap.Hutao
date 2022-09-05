// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// 米哈游Url端点
/// </summary>
[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1124")]
internal static class ApiEndpoints
{
    #region Announcement

    /// <summary>
    /// 公告列表
    /// </summary>
    public const string AnnList = $"{Hk4eApiAnnouncementApi}/getAnnList?{AnnouncementQuery}";

    /// <summary>
    /// 公告内容
    /// </summary>
    public const string AnnContent = $"{Hk4eApiAnnouncementApi}/getAnnContent?{AnnouncementQuery}";
    #endregion

    #region GachaInfo

    /// <summary>
    /// 获取祈愿记录
    /// </summary>
    /// <param name="query">query string</param>
    /// <returns>祈愿记录信息Url</returns>
    public static string GachaInfoGetGachaLog(string query)
    {
        return $"{Hk4eApiGachaInfoApi}/getGachaLog?{query}";
    }
    #endregion

    #region GameRecord

    /// <summary>
    /// 角色信息
    /// </summary>
    public const string GameRecordCharacter = $"{ApiTakumiRecordApi}/character";

    /// <summary>
    /// 游戏记录主页
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="server">服务器区域</param>
    /// <returns>游戏记录主页字符串</returns>
    public static string GameRecordIndex(string uid, string server)
    {
        return $"{ApiTakumiRecordApi}/index?role_id={uid}&server={server}";
    }

    /// <summary>
    /// 深渊信息
    /// </summary>
    /// <param name="scheduleType">深渊类型</param>
    /// <param name="uid">Uid</param>
    /// <returns>深渊信息字符串</returns>
    public static string GameRecordSpiralAbyss(Takumi.GameRecord.SpiralAbyssSchedule scheduleType, PlayerUid uid)
    {
        return $"{ApiTakumiRecordApi}/spiralAbyss?schedule_type={(int)scheduleType}&role_id={uid.Value}&server={uid.Region}";
    }
    #endregion

    #region UserFullInfo

    /// <summary>
    /// 用户详细信息
    /// </summary>
    public const string UserFullInfo = $"{BbsApiUserApi}/getUserFullInfo?gids=2";

    /// <summary>
    /// 查询其他用户详细信息
    /// </summary>
    /// <param name="bbsUid">bbs Uid</param>
    /// <returns>查询其他用户详细信息字符串</returns>
    public static string UserFullInfoQuery(string bbsUid)
    {
        return $"{BbsApiUserApi}/getUserFullInfo?uid={bbsUid}&gids=2";
    }
    #endregion

    #region UserGameRole

    /// <summary>
    /// 用户游戏角色
    /// </summary>
    public const string UserGameRoles = $"{ApiTaKumiBindingApi}/getUserGameRolesByCookie?game_biz=hk4e_cn";
    #endregion

    // consts
    private const string ApiTakumi = "https://api-takumi.mihoyo.com";
    private const string ApiTaKumiBindingApi = $"{ApiTakumi}/binding/api";
    private const string ApiTakumiRecord = "https://api-takumi-record.mihoyo.com";
    private const string ApiTakumiRecordApi = $"{ApiTakumiRecord}/game_record/app/genshin/api";

    private const string BbsApi = "https://bbs-api.mihoyo.com";
    private const string BbsApiUserApi = $"{BbsApi}/user/wapi";

    private const string Hk4eApi = "https://hk4e-api.mihoyo.com";
    private const string Hk4eApiAnnouncementApi = $"{Hk4eApi}/common/hk4e_cn/announcement/api";
    private const string Hk4eApiGachaInfoApi = $"{Hk4eApi}/event/gacha_info/api";

    private const string AnnouncementQuery = "game=hk4e&game_biz=hk4e_cn&lang=zh-cn&bundle_id=hk4e_cn&platform=pc&region=cn_gf01&level=55&uid=100000000";
}
