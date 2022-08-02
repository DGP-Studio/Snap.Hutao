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
    public const string SpiralAbyss = $"{ApiTakumiRecordApi}/spiralAbyss?schedule_type={{0}}&role_id={{1}}&server={{2}}";

    /// <summary>
    /// 角色信息
    /// </summary>
    public const string Character = $"{ApiTakumiRecordApi}/character";

    /// <summary>
    /// 用户游戏角色
    /// </summary>
    public const string UserGameRoles = $"{ApiTakumi}/binding/api/getUserGameRolesByCookie?game_biz=hk4e_cn";

    /// <summary>
    /// 用户详细信息
    /// </summary>
    public const string UserFullInfo = $"{BbsApiUserApi}/getUserFullInfo?gids=2";

    /// <summary>
    /// 查询其他用户详细信息
    /// </summary>
    public const string UserFullInfoQuery = $"{BbsApiUserApi}/getUserFullInfo?uid={{0}}&gids=2";

    private const string ApiTakumi = "https://api-takumi.mihoyo.com";
    private const string ApiTakumiRecord = "https://api-takumi-record.mihoyo.com";
    private const string ApiTakumiRecordApi = $"{ApiTakumiRecord}/game_record/app/genshin/api";
    private const string BbsApi = "https://bbs-api.mihoyo.com";
    private const string BbsApiUserApi = $"{BbsApi}/user/wapi";
    private const string Hk4eApi = "https://hk4e-api.mihoyo.com";

    private const string AnnouncementQuery = "game=hk4e&game_biz=hk4e_cn&lang=zh-cn&bundle_id=hk4e_cn&platform=pc&region=cn_gf01&level=55&uid=100000000";
}
