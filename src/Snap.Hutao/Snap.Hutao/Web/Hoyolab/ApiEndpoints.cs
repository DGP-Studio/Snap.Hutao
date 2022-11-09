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
    /// 游戏记录实时便笺
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="server">服务器区域</param>
    /// <returns>游戏记录实时便笺字符串</returns>
    public static string GameRecordDailyNote(string uid, string server)
    {
        return $"{ApiTakumiRecordApi}/dailyNote?server={server}&role_id={uid}";
    }

    /// <summary>
    /// 游戏记录主页
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="server">服务器区域</param>
    /// <returns>游戏记录主页字符串</returns>
    public static string GameRecordIndex(string uid, string server)
    {
        return $"{ApiTakumiRecordApi}/index?server={server}&role_id={uid}";
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
    /// BBS 指向引用
    /// </summary>
    public const string BbsReferer = "https://bbs.mihoyo.com/";

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

    #region Binding

    /// <summary>
    /// 用户游戏角色
    /// </summary>
    public const string UserGameRoles = $"{ApiTaKumiBindingApi}/getUserGameRolesByCookie?game_biz=hk4e_cn";

    /// <summary>
    /// AuthKey
    /// </summary>
    public const string GenAuthKey = $"{ApiTaKumiBindingApi}/genAuthKey";
    #endregion

    #region Auth

    /// <summary>
    /// 获取 stoken 与 ltoken
    /// </summary>
    /// <param name="loginTicket">登录票证</param>
    /// <param name="loginUid">uid</param>
    /// <returns>Url</returns>
    public static string AuthMultiToken(string loginTicket, string loginUid)
    {
        return $"{ApiTakumiAuthApi}/getMultiTokenByLoginTicket?login_ticket={loginTicket}&uid={loginUid}&token_types=3";
    }
    #endregion

    #region Resource

    /// <summary>
    /// 启动器资源
    /// </summary>
    /// <param name="launcherId">启动器Id</param>
    /// <param name="channel">通道</param>
    /// <param name="subChannel">子通道</param>
    /// <returns>启动器资源字符串</returns>
    public static string SdkStaticLauncherResource(string launcherId, string channel, string subChannel)
    {
        return $"{SdkStaticLauncherApi}/resource?key=eYd89JmJ&launcher_id={launcherId}&channel_id={channel}&sub_channel_id={subChannel}";
    }

    // https://sdk-static.mihoyo.com/hk4e_cn/mdk/launcher/api/content?filter_adv=true&key=eYd89JmJ&language=zh-cn&launcher_id=18
    // https://sdk-static.mihoyo.com/hk4e_cn/mdk/launcher/api/content?key=eYd89JmJ&language=zh-cn&launcher_id=18
    #endregion

    // consts
    private const string ApiTakumi = "https://api-takumi.mihoyo.com";
    private const string ApiTakumiAuthApi = $"{ApiTakumi}/auth/api";
    private const string ApiTaKumiBindingApi = $"{ApiTakumi}/binding/api";
    private const string ApiTakumiRecord = "https://api-takumi-record.mihoyo.com";
    private const string ApiTakumiRecordApi = $"{ApiTakumiRecord}/game_record/app/genshin/api";

    private const string BbsApi = "https://bbs-api.mihoyo.com";
    private const string BbsApiUserApi = $"{BbsApi}/user/wapi";

    private const string Hk4eApi = "https://hk4e-api.mihoyo.com";
    private const string Hk4eApiAnnouncementApi = $"{Hk4eApi}/common/hk4e_cn/announcement/api";
    private const string Hk4eApiGachaInfoApi = $"{Hk4eApi}/event/gacha_info/api";

    private const string SdkStatic = "https://sdk-static.mihoyo.com";
    private const string SdkStaticLauncherApi = $"{SdkStatic}/hk4e_cn/mdk/launcher/api";

    private const string AnnouncementQuery = "game=hk4e&game_biz=hk4e_cn&lang=zh-cn&bundle_id=hk4e_cn&platform=pc&region=cn_gf01&level=55&uid=100000000";
}