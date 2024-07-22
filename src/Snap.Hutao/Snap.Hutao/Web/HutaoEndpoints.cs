// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hutao.GachaLog;

namespace Snap.Hutao.Web;

[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1203")]
internal static class HutaoEndpoints
{
    #region HomaAPI

    #region GachaLog

    public static string GachaLogEndIds(string uid)
    {
        return $"{HomaSnapGenshin}/GachaLog/EndIds?Uid={uid}";
    }

    public const string GachaLogRetrieve = $"{HomaSnapGenshin}/GachaLog/Retrieve";
    public const string GachaLogUpload = $"{HomaSnapGenshin}/GachaLog/Upload";
    public const string GachaLogUids = $"{HomaSnapGenshin}/GachaLog/Uids";
    public const string GachaLogEntries = $"{HomaSnapGenshin}/GachaLog/Entries";

    public static string GachaLogDelete(string uid)
    {
        return $"{HomaSnapGenshin}/GachaLog/Delete?Uid={uid}";
    }

    public const string GachaLogStatisticsCurrentEvents = $"{HomaSnapGenshin}/GachaLog/Statistics/CurrentEventStatistics";

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
    public const string HutaoLogUpload = $"{HomaSnapGenshin}/HutaoLog/Upload";
    #endregion

    #region Passport
    public const string PassportVerify = $"{HomaSnapGenshin}/Passport/Verify";
    public const string PassportRegister = $"{HomaSnapGenshin}/Passport/Register";
    public const string PassportCancel = $"{HomaSnapGenshin}/Passport/Cancel";
    public const string PassportResetPassword = $"{HomaSnapGenshin}/Passport/ResetPassword";
    public const string PassportLogin = $"{HomaSnapGenshin}/Passport/Login";
    public const string PassportUserInfo = $"{HomaSnapGenshin}/Passport/UserInfo";
    #endregion

    #region SpiralAbyss
    public static string RecordCheck(string uid)
    {
        return $"{HomaSnapGenshin}/Record/Check?uid={uid}";
    }

    public static string RecordRank(string uid)
    {
        return $"{HomaSnapGenshin}/Record/Rank?uid={uid}";
    }

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
    public static string Metadata(string locale, string fileName)
    {
        return $"{ApiSnapGenshinMetadata}/Genshin/{locale}/{fileName}";
    }
    #endregion

    #region Patch
    public const string PatchYaeAchievement = $"{ApiSnapGenshinPatch}/yae";
    public const string PatchSnapHutao = $"{ApiSnapGenshinPatch}/hutao";

    public static string PatchAlphaSnapHutao(bool isCN)
    {
       return isCN
            ? $"{ApiAlphaSnapGenshin}/cn/patch/hutao"
            : $"{ApiAlphaSnapGenshin}/global/patch/hutao";
    }
    #endregion

    #region StaticResources
    public static readonly Uri UIIconNone = StaticRaw("Bg", "UI_Icon_None.png").ToUri();
    public static readonly Uri UIItemIconNone = StaticRaw("Bg", "UI_ItemIcon_None.png").ToUri();
    public static readonly Uri UIAvatarIconSideNone = StaticRaw("AvatarIcon", "UI_AvatarIcon_Side_None.png").ToUri();

    public static string StaticRaw(string category, string fileName)
    {
        return $"{ApiSnapGenshinStaticRaw}/{category}/{fileName}";
    }

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

    private const string ApiAlphaSnapGenshin = "https://api-alpha.snapgenshin.cn";
    private const string ApiSnapGenshin = "https://api.snapgenshin.com";
    private const string ApiSnapGenshinMetadata = $"{ApiSnapGenshin}/metadata";
    private const string ApiSnapGenshinPatch = $"{ApiSnapGenshin}/patch";
    private const string ApiSnapGenshinStaticRaw = $"{ApiSnapGenshin}/static/raw";
    private const string ApiSnapGenshinStaticZip = $"{ApiSnapGenshin}/static/zip";
    private const string ApiSnapGenshinEnka = $"{ApiSnapGenshin}/enka";
    private const string HomaSnapGenshin = "https://homa.snapgenshin.com";
}