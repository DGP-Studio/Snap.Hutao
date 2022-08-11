// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// 米哈游Url端点
/// </summary>
[SuppressMessage("", "SA1201")]
internal static class ApiEndpoints
{
    /// <summary>
    /// 公告列表
    /// </summary>
    public const string AnnList = $"{Hk4eApi}/common/hk4e_cn/announcement/api/getAnnList?{AnnouncementQuery}";

    /// <summary>
    /// 公告内容
    /// </summary>
    public const string AnnContent = $"{Hk4eApi}/common/hk4e_cn/announcement/api/getAnnContent?{AnnouncementQuery}";

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

    /// <summary>
    /// 签到活动Id
    /// </summary>
    public const string SignInRewardActivityId = "e202009291139501";

    /// <summary>
    /// 签到
    /// </summary>
    public const string SignInRewardHome = $"{ApiTakumi}/event/bbs_sign_reward/home?act_id={SignInRewardActivityId}";

    /// <summary>
    /// 签到信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>签到信息字符串</returns>
    public static string SignInRewardInfo(PlayerUid uid)
    {
        return $"{ApiTakumi}/event/bbs_sign_reward/info?act_id={SignInRewardActivityId}&region={uid.Region}&uid={uid.Value}";
    }

    /// <summary>
    /// 签到
    /// </summary>
    public const string SignInRewardReSign = $"{ApiTakumi}/event/bbs_sign_reward/resign";

    /// <summary>
    /// 补签信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>补签信息字符串</returns>
    public static string SignInRewardResignInfo(PlayerUid uid)
    {
        return $"{ApiTakumi}/event/bbs_sign_reward/resign_info?act_id=e202009291139501&region={uid.Region}&uid={uid.Value}";
    }

    /// <summary>
    /// 签到
    /// </summary>
    public const string SignInRewardSign = $"{ApiTakumi}/event/bbs_sign_reward/sign";

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

    /// <summary>
    /// 用户游戏角色
    /// </summary>
    public const string UserGameRoles = $"{ApiTakumi}/binding/api/getUserGameRolesByCookie?game_biz=hk4e_cn";

    // consts
    private const string ApiTakumi = "https://api-takumi.mihoyo.com";
    private const string ApiTakumiRecord = "https://api-takumi-record.mihoyo.com";
    private const string ApiTakumiRecordApi = $"{ApiTakumiRecord}/game_record/app/genshin/api";
    private const string BbsApi = "https://bbs-api.mihoyo.com";
    private const string BbsApiUserApi = $"{BbsApi}/user/wapi";
    private const string Hk4eApi = "https://hk4e-api.mihoyo.com";

    private const string AnnouncementQuery = "game=hk4e&game_biz=hk4e_cn&lang=zh-cn&bundle_id=hk4e_cn&platform=pc&region=cn_gf01&level=55&uid=100000000";
}
