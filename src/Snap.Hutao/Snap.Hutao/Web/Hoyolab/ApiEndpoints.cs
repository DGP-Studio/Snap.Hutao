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
    public static readonly string AnnList = $"{Hk4eApi}/common/hk4e_cn/announcement/api/getAnnList?{AnnouncementQuery}";

    /// <summary>
    /// 公告内容
    /// </summary>
    public static readonly string AnnContent = $"{Hk4eApi}/common/hk4e_cn/announcement/api/getAnnContent?{AnnouncementQuery}";

    private const string Hk4eApi = "https://hk4e-api.mihoyo.com";
    private const string AnnouncementQuery = "game=hk4e&game_biz=hk4e_cn&lang=zh-cn&bundle_id=hk4e_cn&platform=pc&region=cn_gf01&level=55&uid=100000000";
}
