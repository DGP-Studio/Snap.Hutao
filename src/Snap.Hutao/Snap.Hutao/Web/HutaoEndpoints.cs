// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hutao.GachaLog;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Web;

[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1203")]
internal static partial class HutaoEndpoints
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
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/enka/{uid}",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/enka/{uid}",
            _ => $"{ApiSnapGenshin}/enka/{uid}",
        };
    }

    public static string EnkaPlayerInfo(in PlayerUid uid)
    {
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/enka/{uid}/info",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/enka/{uid}/info",
            _ => $"{ApiSnapGenshin}/enka/{uid}/info",
        };
    }

    public static string Ip()
    {
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/ip",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/ip",
            _ => $"{ApiSnapGenshin}/ip",
        };
    }

    #region Feature
    public static string Feature(string name)
    {
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/client/{name}.json",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/client/{name}.json",
            _ => $"{ApiSnapGenshin}/client/{name}.json",
        };
    }
    #endregion

    #region Metadata
    public static string Metadata(string locale, string fileName)
    {
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/metadata/Genshin/{locale}/{fileName}",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/metadata/Genshin/{locale}/{fileName}",
            _ => $"{ApiSnapGenshin}/metadata/Genshin/{locale}/{fileName}",
        };
    }
    #endregion

    #region Patch
    public static string PatchYaeAchievement()
    {
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/patch/yae",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/patch/yae",
            _ => $"{ApiAlphaSnapGenshin}/patch/yae",
        };
    }

    public static string PatchSnapHutao()
    {
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/patch/hutao",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/patch/hutao",
            _ => $"{ApiAlphaSnapGenshin}/patch/hutao",
        };
    }
    #endregion

    #region StaticResources
    public static readonly Uri UIIconNone = StaticRaw("Bg", "UI_Icon_None.png").ToUri();
    public static readonly Uri UIItemIconNone = StaticRaw("Bg", "UI_ItemIcon_None.png").ToUri();
    public static readonly Uri UIAvatarIconSideNone = StaticRaw("AvatarIcon", "UI_AvatarIcon_Side_None.png").ToUri();

    public static string StaticRaw(string category, string fileName)
    {
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/static/raw/{category}/{fileName}",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/static/raw/{category}/{fileName}",
            _ => $"{ApiSnapGenshin}/static/raw/{category}/{fileName}",
        };
    }

    public static string StaticZip(string fileName)
    {
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/static/zip/{fileName}.zip",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/static/zip/{fileName}.zip",
            _ => $"{ApiSnapGenshin}/static/zip/{fileName}.zip",
        };
    }

    public static string StaticSize()
    {
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/static/size",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/static/size",
            _ => $"{ApiSnapGenshin}/static/size",
        };
    }
    #endregion

    #region Wallpaper

    public static string WallpaperBing()
    {
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/wallpaper/bing",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/wallpaper/bing",
            _ => $"{ApiSnapGenshin}/wallpaper/bing",
        };
    }

    public static string WallpaperGenshinLauncher()
    {
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/wallpaper/genshinlauncher",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/wallpaper/genshinlauncher",
            _ => $"{ApiSnapGenshin}/wallpaper/genshinlauncher",
        };
    }

    public static string WallpaperToday()
    {
        return Kind switch
        {
            ApiKind.AlphaCN => $"{ApiAlphaSnapGenshin}/cn/wallpaper/today",
            ApiKind.AlphaOS => $"{ApiAlphaSnapGenshin}/global/wallpaper/today",
            _ => $"{ApiSnapGenshin}/wallpaper/today",
        };
    }
    #endregion

    #endregion

    private const string ApiSnapGenshin = "https://api.snapgenshin.com";
    private const string HomaSnapGenshin = "https://homa.snapgenshin.com";
}

internal static partial class HutaoEndpoints
{
    private enum ApiKind
    {
        AlphaCN,
        AlphaOS,
        Formal,
    }

    private static ApiKind Kind
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
#if IS_ALPHA_BUILD
            return Core.Setting.LocalSetting.Get(Core.Setting.SettingKeys.AlphaBuildUseCNPatchEndpoint, false) ? ApiKind.AlphaCN : ApiKind.AlphaOS;
#else
            return ApiKind.Formal;
#endif
        }
    }

    private const string ApiAlphaSnapGenshin = "https://api-alpha.snapgenshin.cn";
}