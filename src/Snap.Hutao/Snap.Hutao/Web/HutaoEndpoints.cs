﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hutao.GachaLog;

namespace Snap.Hutao.Web;

/// <summary>
/// 胡桃 API 端点
/// </summary>
[HighQuality]
[SuppressMessage("", "SA1124")]
[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1203")]
internal static class HutaoEndpoints
{
    #region HomaAPI

    #region GachaLog

    /// <summary>
    /// 获取末尾Id
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>获取末尾Id Url</returns>
    public static string GachaLogEndIds(string uid)
    {
        return $"{HomaSnapGenshin}/GachaLog/EndIds?Uid={uid}";
    }

    /// <summary>
    /// 获取祈愿记录
    /// </summary>
    public const string GachaLogRetrieve = $"{HomaSnapGenshin}/GachaLog/Retrieve";

    /// <summary>
    /// 上传祈愿记录
    /// </summary>
    public const string GachaLogUpload = $"{HomaSnapGenshin}/GachaLog/Upload";

    /// <summary>
    /// 获取Uid列表
    /// </summary>
    public const string GachaLogUids = $"{HomaSnapGenshin}/GachaLog/Uids";

    /// <summary>
    /// 获取Uid列表
    /// </summary>
    public const string GachaLogEntries = $"{HomaSnapGenshin}/GachaLog/Entries";

    /// <summary>
    /// 删除祈愿记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>删除祈愿记录 Url</returns>
    public static string GachaLogDelete(string uid)
    {
        return $"{HomaSnapGenshin}/GachaLog/Delete?Uid={uid}";
    }

    /// <summary>
    /// 获取祈愿统计信息
    /// </summary>
    public const string GachaLogStatisticsCurrentEvents = $"{HomaSnapGenshin}/GachaLog/Statistics/CurrentEventStatistics";

    /// <summary>
    /// 获取祈愿统计信息
    /// </summary>
    /// <param name="distributionType">分布类型</param>
    /// <returns>祈愿统计信息Url</returns>
    public static string GachaLogStatisticsDistribution(GachaDistributionType distributionType)
    {
        return $"{HomaSnapGenshin}/GachaLog/Statistics/Distribution/{distributionType}";
    }
    #endregion

    #region Hutao as a Service
    public static string Announcement(string locale)
    {
        return $"{HomaSnapGenshin}/Announcement/List?locale={locale}";
    }

    public const string AnnouncementUpload = $"{HomaSnapGenshin}/Service/Announcement/Upload";

    public static string GachaLogCompensation(int days)
    {
        return $"{HomaSnapGenshin}/Service/GachaLog/Compensation?days={days}";
    }

    public static string GachaLogDesignation(string userName, int days)
    {
        return $"{HomaSnapGenshin}/Service/GachaLog/Designation?userName={userName}&days={days}";
    }
    #endregion

    #region LogUpload

    /// <summary>
    /// 上传日志
    /// </summary>
    public const string HutaoLogUpload = $"{HomaSnapGenshin}/HutaoLog/Upload";
    #endregion

    #region Passport

    /// <summary>
    /// 获取注册验证码
    /// </summary>
    public const string PassportVerify = $"{HomaSnapGenshin}/Passport/Verify";

    /// <summary>
    /// 注册账号
    /// </summary>
    public const string PassportRegister = $"{HomaSnapGenshin}/Passport/Register";

    /// <summary>
    /// 注销账号
    /// </summary>
    public const string PassportCancel = $"{HomaSnapGenshin}/Passport/Cancel";

    /// <summary>
    /// 重设密码
    /// </summary>
    public const string PassportResetPassword = $"{HomaSnapGenshin}/Passport/ResetPassword";

    /// <summary>
    /// 登录
    /// </summary>
    public const string PassportLogin = $"{HomaSnapGenshin}/Passport/Login";

    /// <summary>
    /// 用户信息
    /// </summary>
    public const string PassportUserInfo = $"{HomaSnapGenshin}/Passport/UserInfo";
    #endregion

    #region SpiralAbyss

    /// <summary>
    /// 检查 uid 是否上传记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>路径</returns>
    public static string RecordCheck(string uid)
    {
        return $"{HomaSnapGenshin}/Record/Check?uid={uid}";
    }

    /// <summary>
    /// uid 排行
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>路径</returns>
    public static string RecordRank(string uid)
    {
        return $"{HomaSnapGenshin}/Record/Rank?uid={uid}";
    }

    /// <summary>
    /// 上传记录
    /// </summary>
    public const string RecordUpload = $"{HomaSnapGenshin}/Record/Upload";

    public const string StatisticsOverview = $"{HomaSnapGenshin}/Statistics/Overview";
    public const string StatisticsOverviewLast = $"{StatisticsOverview}?Last=true";
    public const string StatisticsAvatarAttendanceRate = $"{HomaSnapGenshin}/Statistics/Avatar/AttendanceRate";
    public const string StatisticsAvatarAttendanceRateLast = $"{StatisticsAvatarAttendanceRate}?Last=true";
    public const string StatisticsAvatarUtilizationRate = $"{HomaSnapGenshin}/Statistics/Avatar/UtilizationRate";
    public const string StatisticsAvatarUtilizationRateLast = $"{StatisticsAvatarUtilizationRate}?Last=true";
    public const string StatisticsAvatarAvatarCollocation = $"{HomaSnapGenshin}/Statistics/Avatar/AvatarCollocation";
    public const string StatisticsAvatarAvatarCollocationLast = $"{StatisticsAvatarAvatarCollocation}?Last=true";
    public const string StatisticsAvatarHoldingRate = $"{HomaSnapGenshin}/Statistics/Avatar/HoldingRate";
    public const string StatisticsAvatarHoldingRateLast = $"{StatisticsAvatarHoldingRate}?Last=true";
    public const string StatisticsWeaponWeaponCollocation = $"{HomaSnapGenshin}/Statistics/Weapon/WeaponCollocation";
    public const string StatisticsWeaponWeaponCollocationLast = $"{StatisticsWeaponWeaponCollocation}?Last=true";
    public const string StatisticsTeamCombination = $"{HomaSnapGenshin}/Statistics/Team/Combination";
    public const string StatisticsTeamCombinationLast = $"{StatisticsTeamCombination}?Last=true";
    #endregion

    public static string Website(string path)
    {
        return $"{HomaSnapGenshin}/{path}";
    }

    #endregion

    #region Infrasturcture

    public static string Enka(in PlayerUid uid)
    {
        return $"{ApiSnapGenshinEnka}/{uid}";
    }

    public static string EnkaPlayerInfo(in PlayerUid uid)
    {
        return $"{ApiSnapGenshinEnka}/{uid}/info";
    }

    public const string Ip = $"{ApiSnapGenshin}/ip";

    #region Metadata

    /// <summary>
    /// 胡桃元数据2文件
    /// </summary>
    /// <param name="locale">语言</param>
    /// <param name="fileName">文件名称</param>
    /// <returns>路径</returns>
    public static string Metadata(string locale, string fileName)
    {
        return $"{ApiSnapGenshinMetadata}/Genshin/{locale}/{fileName}";
    }
    #endregion

    #region Patch
    public const string PatchYaeAchievement = $"{ApiSnapGenshinPatch}/yae";
    public const string PatchSnapHutao = $"{ApiSnapGenshinPatch}/hutao";
    #endregion

    #region StaticResources

    /// <summary>
    /// UI_Icon_None
    /// </summary>
    public static readonly Uri UIIconNone = StaticRaw("Bg", "UI_Icon_None.png").ToUri();

    /// <summary>
    /// UI_ItemIcon_None
    /// </summary>
    public static readonly Uri UIItemIconNone = StaticRaw("Bg", "UI_ItemIcon_None.png").ToUri();

    /// <summary>
    /// UI_AvatarIcon_Side_None
    /// </summary>
    public static readonly Uri UIAvatarIconSideNone = StaticRaw("AvatarIcon", "UI_AvatarIcon_Side_None.png").ToUri();

    /// <summary>
    /// 图片资源
    /// </summary>
    /// <param name="category">分类</param>
    /// <param name="fileName">文件名称 包括后缀</param>
    /// <returns>路径</returns>
    public static string StaticRaw(string category, string fileName)
    {
        return $"{ApiSnapGenshinStaticRaw}/{category}/{fileName}";
    }

    /// <summary>
    /// 压缩包资源
    /// </summary>
    /// <param name="fileName">文件名称 不包括后缀</param>
    /// <returns>路径</returns>
    public static string StaticZip(string fileName)
    {
        return $"{ApiSnapGenshinStaticZip}/{fileName}.zip";
    }

    public const string StaticSize = $"{ApiSnapGenshin}/static/size";
    #endregion

    #region Wallpaper

    public const string WallpaperBing = $"{ApiSnapGenshin}/wallpaper/bing";

    public const string WallpaperGenshinLauncher = $"{ApiSnapGenshin}/wallpaper/hoyoplay";

    public const string WallpaperToday = $"{ApiSnapGenshin}/wallpaper/today";
    #endregion

    #endregion

    private const string ApiSnapGenshin = "https://api.snapgenshin.com";
    private const string ApiSnapGenshinMetadata = $"{ApiSnapGenshin}/metadata";
    private const string ApiSnapGenshinPatch = $"{ApiSnapGenshin}/patch";
    private const string ApiSnapGenshinStaticRaw = $"{ApiSnapGenshin}/static/raw";
    private const string ApiSnapGenshinStaticZip = $"{ApiSnapGenshin}/static/zip";
    private const string ApiSnapGenshinEnka = $"{ApiSnapGenshin}/enka";
    private const string HomaSnapGenshin = "https://homa.snapgenshin.com";
}