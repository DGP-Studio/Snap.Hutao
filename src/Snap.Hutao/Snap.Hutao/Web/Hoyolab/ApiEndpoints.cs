// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// 主机Url集合
/// </summary>
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
    public const string GameRecordIndex = $"{ApiTakumiRecordApi}/index?role_id={{0}}&server={{1}}";

    /// <summary>
    /// 深渊信息
    /// </summary>
    public const string SpiralAbyss = $"{ApiTakumiRecordApi}/spiralAbyss?schedule_type={{0}}&role_id={{1}}&server={{2}}";

    /// <summary>
    /// 角色信息
    /// </summary>
    public const string Character = $"{ApiTakumiRecordApi}/character";

    public const string UserGameRoles = $"{ApiTakumi}/binding/api/getUserGameRolesByCookie?game_biz=hk4e_cn";

    private const string ApiTakumi = "https://api-takumi.mihoyo.com";
    private const string ApiTakumiRecord = "https://api-takumi-record.mihoyo.com";
    private const string ApiTakumiRecordApi = $"{ApiTakumiRecord}/game_record/app/genshin/api";
    private const string Hk4eApi = "https://hk4e-api.mihoyo.com";

    private const string AnnouncementQuery = "game=hk4e&game_biz=hk4e_cn&lang=zh-cn&bundle_id=hk4e_cn&platform=pc&region=cn_gf01&level=55&uid=100000000";
}
