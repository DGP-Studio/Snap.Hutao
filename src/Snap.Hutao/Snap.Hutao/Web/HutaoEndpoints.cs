// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.GachaLog;

namespace Snap.Hutao.Web;

/// <summary>
/// 胡桃 API 端点
/// </summary>
[HighQuality]
[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1124")]
internal static class HutaoEndpoints
{
    #region Hutao as a Service
    public static string Announcement(string locale)
    {
        return $"{HomaSnapGenshinApi}/Announcement/List?locale={locale}";
    }

    public const string AnnouncementUpload = $"{HomaSnapGenshinApi}/Service/Announcement/Upload";

    public static string GachaLogCompensation(int days)
    {
        return $"{HomaSnapGenshinApi}/Service/GachaLog/Compensation?days={days}";
    }

    public static string GachaLogDesignation(string userName, int days)
    {
        return $"{HomaSnapGenshinApi}/Service/GachaLog/Designation?userName={userName}&days={days}";
    }
    #endregion

    #region GachaLog

    /// <summary>
    /// 获取末尾Id
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>获取末尾Id Url</returns>
    public static string GachaLogEndIds(string uid)
    {
        return $"{HomaSnapGenshinApi}/GachaLog/EndIds?Uid={uid}";
    }

    /// <summary>
    /// 获取祈愿记录
    /// </summary>
    public const string GachaLogRetrieve = $"{HomaSnapGenshinApi}/GachaLog/Retrieve";

    /// <summary>
    /// 上传祈愿记录
    /// </summary>
    public const string GachaLogUpload = $"{HomaSnapGenshinApi}/GachaLog/Upload";

    /// <summary>
    /// 获取Uid列表
    /// </summary>
    public const string GachaLogUids = $"{HomaSnapGenshinApi}/GachaLog/Uids";

    /// <summary>
    /// 获取Uid列表
    /// </summary>
    public const string GachaLogEntries = $"{HomaSnapGenshinApi}/GachaLog/Entries";

    /// <summary>
    /// 删除祈愿记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>删除祈愿记录 Url</returns>
    public static string GachaLogDelete(string uid)
    {
        return $"{HomaSnapGenshinApi}/GachaLog/Delete?Uid={uid}";
    }

    /// <summary>
    /// 获取祈愿统计信息
    /// </summary>
    public const string GachaLogStatisticsCurrentEvents = $"{HomaSnapGenshinApi}/GachaLog/Statistics/CurrentEventStatistics";

    /// <summary>
    /// 获取祈愿统计信息
    /// </summary>
    /// <param name="distributionType">分布类型</param>
    /// <returns>祈愿统计信息Url</returns>
    public static string GachaLogStatisticsDistribution(GachaDistributionType distributionType)
    {
        return $"{HomaSnapGenshinApi}/GachaLog/Statistics/Distribution/{distributionType}";
    }
    #endregion

    #region Passport

    /// <summary>
    /// 获取注册验证码
    /// </summary>
    public const string PassportVerify = $"{HomaSnapGenshinApi}/Passport/Verify";

    /// <summary>
    /// 注册账号
    /// </summary>
    public const string PassportRegister = $"{HomaSnapGenshinApi}/Passport/Register";

    /// <summary>
    /// 注销账号
    /// </summary>
    public const string PassportCancel = $"{HomaSnapGenshinApi}/Passport/Cancel";

    /// <summary>
    /// 重设密码
    /// </summary>
    public const string PassportResetPassword = $"{HomaSnapGenshinApi}/Passport/ResetPassword";

    /// <summary>
    /// 登录
    /// </summary>
    public const string PassportLogin = $"{HomaSnapGenshinApi}/Passport/Login";

    /// <summary>
    /// 用户信息
    /// </summary>
    public const string PassportUserInfo = $"{HomaSnapGenshinApi}/Passport/UserInfo";
    #endregion

    #region LogUpload

    /// <summary>
    /// 上传日志
    /// </summary>
    public const string HutaoLogUpload = $"{HomaSnapGenshinApi}/HutaoLog/Upload";
    #endregion

    #region SpiralAbyss

    /// <summary>
    /// 检查 uid 是否上传记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>路径</returns>
    public static string RecordCheck(string uid)
    {
        return $"{HomaSnapGenshinApi}/Record/Check?uid={uid}";
    }

    /// <summary>
    /// uid 排行
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>路径</returns>
    public static string RecordRank(string uid)
    {
        return $"{HomaSnapGenshinApi}/Record/Rank?uid={uid}";
    }

    /// <summary>
    /// 上传记录
    /// </summary>
    public const string RecordUpload = $"{HomaSnapGenshinApi}/Record/Upload";

    /// <summary>
    /// 统计信息
    /// </summary>
    public const string StatisticsOverview = $"{HomaSnapGenshinApi}/Statistics/Overview";

    /// <summary>
    /// 出场率
    /// </summary>
    public const string StatisticsAvatarAttendanceRate = $"{HomaSnapGenshinApi}/Statistics/Avatar/AttendanceRate";

    /// <summary>
    /// 使用率
    /// </summary>
    public const string StatisticsAvatarUtilizationRate = $"{HomaSnapGenshinApi}/Statistics/Avatar/UtilizationRate";

    /// <summary>
    /// 角色搭配
    /// </summary>
    public const string StatisticsAvatarAvatarCollocation = $"{HomaSnapGenshinApi}/Statistics/Avatar/AvatarCollocation";

    /// <summary>
    /// 角色持有率
    /// </summary>
    public const string StatisticsAvatarHoldingRate = $"{HomaSnapGenshinApi}/Statistics/Avatar/HoldingRate";

    /// <summary>
    /// 武器搭配
    /// </summary>
    public const string StatisticsWeaponWeaponCollocation = $"{HomaSnapGenshinApi}/Statistics/Weapon/WeaponCollocation";

    /// <summary>
    /// 持有率
    /// </summary>
    public const string StatisticsTeamCombination = $"{HomaSnapGenshinApi}/Statistics/Team/Combination";
    #endregion

    #region Metadata

    /// <summary>
    /// 胡桃元数据文件
    /// </summary>
    /// <param name="locale">语言</param>
    /// <param name="fileName">文件名称</param>
    /// <returns>路径</returns>
    public static string HutaoMetadataFile(string locale, string fileName)
    {
        return $"{HutaoMetadataSnapGenshinApi}/{locale}/{fileName}";
    }

    /// <summary>
    /// 胡桃元数据2文件
    /// </summary>
    /// <param name="locale">语言</param>
    /// <param name="fileName">文件名称</param>
    /// <returns>路径</returns>
    public static string HutaoMetadata2File(string locale, string fileName)
    {
        return $"{HutaoMetadata2SnapGenshinApi}/Genshin/{locale}/{fileName}";
    }

    /// <summary>
    /// Github 元数据文件
    /// </summary>
    /// <param name="locale">语言</param>
    /// <param name="fileName">文件名称</param>
    /// <returns>路径</returns>
    public static string RawGithubUserContentMetadataFile(string locale, string fileName)
    {
        return $"{RawGithubUserContentSnapMetadataApi}/Genshin/{locale}/{fileName}";
    }
    #endregion

    #region Static & Zip

    /// <summary>
    /// UI_Icon_None
    /// </summary>
    public static readonly Uri UIIconNone = StaticFile("Bg", "UI_Icon_None.png").ToUri();

    /// <summary>
    /// UI_ItemIcon_None
    /// </summary>
    public static readonly Uri UIItemIconNone = StaticFile("Bg", "UI_ItemIcon_None.png").ToUri();

    /// <summary>
    /// UI_AvatarIcon_Side_None
    /// </summary>
    public static readonly Uri UIAvatarIconSideNone = StaticFile("AvatarIcon", "UI_AvatarIcon_Side_None.png").ToUri();

    /// <summary>
    /// 图片资源
    /// </summary>
    /// <param name="category">分类</param>
    /// <param name="fileName">文件名称 包括后缀</param>
    /// <returns>路径</returns>
    public static string StaticFile(string category, string fileName)
    {
        return $"{StaticSnapGenshinApi}/{category}/{fileName}";
    }

    /// <summary>
    /// 压缩包资源
    /// </summary>
    /// <param name="fileName">文件名称 不包括后缀</param>
    /// <returns>路径</returns>
    public static string StaticZip(string fileName)
    {
        return $"{StaticZipSnapGenshinApi}/{fileName}.zip";
    }
    #endregion

    private const string HomaSnapGenshinApi = "https://homa.snapgenshin.com";
    private const string HutaoMetadataSnapGenshinApi = "https://hutao-metadata.snapgenshin.com";
    private const string HutaoMetadata2SnapGenshinApi = "https://hutao-metadata2.snapgenshin.com";
    private const string RawGithubUserContentSnapMetadataApi = "https://raw.githubusercontent.com/DGP-Studio/Snap.Metadata/main";
    private const string StaticSnapGenshinApi = "https://static.snapgenshin.com";
    private const string StaticZipSnapGenshinApi = "https://static-zip.snapgenshin.com";
}