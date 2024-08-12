// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1202")]
[Obsolete]
internal static class ApiEndpoints
{
    #region ApiTakumiCardApi | ApiTakumiRecordApi
    public const string CardWidgetData = $"{ApiTakumiCardApi}/getWidgetData?game_id=2";
    public const string CardWidgetData2 = $"{ApiTakumiRecordAApi}/widget/v2?game_id=2";

    public static string CardCreateVerification(bool highRisk)
    {
        return $"{ApiTakumiCardWApi}/createVerification?is_high={(highRisk ? "true" : "false")}";
    }

    public const string CardVerifyVerification = $"{ApiTakumiCardWApi}/verifyVerification";

    public static string GameRecordRoleBasicInfo(in PlayerUid uid)
    {
        return $"{ApiTakumiRecordApi}/roleBasicInfo?role_id={uid.Value}&server={uid.Region}";
    }

    public const string GameRecordCharacter = $"{ApiTakumiRecordApi}/character";

    public static string GameRecordDailyNote(in PlayerUid uid)
    {
        return $"{GameRecordDailyNotePath}?server={uid.Region}&role_id={uid.Value}";
    }

    public const string GameRecordDailyNotePath = $"{ApiTakumiRecordApi}/dailyNote";

    public static string GameRecordIndex(in PlayerUid uid)
    {
        return $"{GameRecordIndexPath}?server={uid.Region}&role_id={uid.Value}";
    }

    public const string GameRecordIndexPath = $"{ApiTakumiRecordApi}/index";

    public static string GameRecordSpiralAbyss(Web.Hoyolab.Takumi.GameRecord.ScheduleType scheduleType, in PlayerUid uid)
    {
        return $"{GameRecordSpiralAbyssPath}?schedule_type={(int)scheduleType}&role_id={uid.Value}&server={uid.Region}";
    }

    public const string GameRecordSpiralAbyssPath = $"{ApiTakumiRecordApi}/spiralAbyss";

    public static string GameRecordRoleCombat(in PlayerUid uid)
    {
        return $"{GameRecordRoleCombatPath}?server={uid.Region}&role_id={uid.Value}&need_detail=true";
    }

    public const string GameRecordRoleCombatPath = $"{ApiTakumiRecordApi}/role_combat";
    #endregion

    #region ApiTakumiEventCalculate

    #region V1
    public const string CalculateAvatarList = $"{ApiTakumiEventCalculate}/v1/avatar/list";

    public static string CalculateAvatarSkillList(Web.Hoyolab.Takumi.Event.Calculate.Avatar avatar)
    {
        return $"{ApiTakumiEventCalculate}/v1/avatarSkill/list?avatar_id={avatar.Id}&element_attr_id={(int)avatar.ElementAttrId}";
    }

    public static string CalculateFurnitureBlueprint(string shareCode)
    {
        // &region=cn_gf01
        // ignored
        return $"{ApiTakumiEventCalculate}/v1/furniture/blueprint?share_code={shareCode}";
    }

    public const string CalculateFurnitureCompute = $"{ApiTakumiEventCalculate}/v1/furniture/compute";

    public const string CalculateFurnitureList = $"{ApiTakumiEventCalculate}/v1/furniture/list";

    public static string CalculateSyncAvatarDetail(in AvatarId avatarId, in PlayerUid uid)
    {
        return $"{ApiTakumiEventCalculate}/v1/sync/avatar/detail?avatar_id={avatarId.Value}&uid={uid.Value}&region={uid.Region}";
    }

    public const string CalculateSyncAvatarList = $"{ApiTakumiEventCalculate}/v1/sync/avatar/list";

    public const string CalculateWeaponList = $"{ApiTakumiEventCalculate}/v1/weapon/list";
    #endregion

    #region V2
    public const string CalculateCompute = $"{ApiTakumiEventCalculate}/v2/compute";
    #endregion

    #region V3
    public const string CalculateBatchCompute = $"{ApiTakumiEventCalculate}/v3/batch_compute";
    #endregion

    #endregion

    #region ApiTakumiEventLuna
    public const string LunaActivityId = "e202311201442471";

    public static string LunaHome(string languageCode)
    {
        return $"{ApiTakumiEventLuna}/home?lang={languageCode}&act_id={LunaActivityId}";
    }

    public const string LunaSign = $"{ApiTakumiEventLuna}/sign";

    public const string LunaReSign = $"{ApiTakumiEventLuna}/resign";

    public static string LunaExtraAward(in PlayerUid uid, string languageCode)
    {
        return $"{ApiTakumiEventLuna}/home?act_id={LunaActivityId}&{uid.ToUidRegionQueryString()}&lang={languageCode}";
    }

    public static string LunaResignInfo(in PlayerUid uid)
    {
        return $"{ApiTakumiEventLuna}/resign_info?act_id={LunaActivityId}&{uid.ToUidRegionQueryString()}";
    }

    public static string LunaInfo(in PlayerUid uid, string languageCode)
    {
        return $"{ApiTakumiEventLuna}/info?lang={languageCode}&act_id={LunaActivityId}&{uid.ToUidRegionQueryString()}";
    }
    #endregion

    #region AppAuthApi
    public const string AppAuthGenAuthKey = $"{AppAuthApi}/genAuthKey";
    #endregion

    #region BbsApiUserApi
    public const string BbsReferer = "https://bbs.mihoyo.com/";
    public const string UserFullInfo = $"{BbsApiUserWApi}/getUserFullInfo?gids=2";
    #endregion

    #region DownloaderApi

    public static string SophonChunkGetBuild(BranchWrapper branch)
    {
        return $"{DownloaderSophonChunkApi}/getBuild?branch={branch.Branch}&package_id={branch.PackageId}&password={branch.Password}&tag={branch.Tag}";
    }

    #endregion

    #region Hk4eApiAnnouncementApi
    public static string AnnList(string languageCode, in Region region)
    {
        return $"{Hk4eApiAnnouncementApi}/getAnnList?{AnnouncementQuery(languageCode, region)}";
    }

    public static string AnnContent(string languageCode, in Region region)
    {
        return $"{Hk4eApiAnnouncementApi}/getAnnContent?{AnnouncementQuery(languageCode, region)}";
    }
    #endregion

    #region Hk4eSdk
    public const string QrCodeFetch = $"{Hk4eSdk}/hk4e_cn/combo/panda/qrcode/fetch";
    public const string QrCodeQuery = $"{Hk4eSdk}/hk4e_cn/combo/panda/qrcode/query";
    #endregion

    #region HoyoPlayApi
    public static string HoyoPlayConnectGamePackages(LaunchScheme scheme)
    {
        return $"{HoyoPlayApiConnectApi}/getGamePackages?game_ids[]={scheme.GameId}&launcher_id={scheme.LauncherId}";
    }

    public static string HoyoPlayConnectGameChannelSDKs(LaunchScheme scheme)
    {
        return $"{HoyoPlayApiConnectApi}/getGameChannelSDKs?channel={scheme.Channel:D}&game_ids[]={scheme.GameId}&launcher_id={scheme.LauncherId}&sub_channel={scheme.SubChannel:D}";
    }

    public static string HoyoPlayConnectDeprecatedFileConfigs(LaunchScheme scheme)
    {
        return $"{HoyoPlayApiConnectApi}/getGameDeprecatedFileConfigs?channel={scheme.Channel:D}&game_ids[]={scheme.GameId}&launcher_id={scheme.LauncherId}&sub_channel={scheme.SubChannel:D}";
    }

    public static string HoyoPlayConnectGameBranches(LaunchScheme scheme)
    {
        return $"{HoyoPlayApiConnectApi}/getGameBranches?game_ids[]={scheme.GameId}&launcher_id={scheme.LauncherId}";
    }

    #endregion

    #region PassportApi | PassportApiV4
    public const string AccountGetCookieTokenBySToken = $"{PassportApiAuthApi}/getCookieAccountInfoBySToken";
    public const string AccountGetLTokenBySToken = $"{PassportApiAuthApi}/getLTokenBySToken";
    public const string AccountGetSTokenByGameToken = $"{PassportApi}/account/ma-cn-session/app/getTokenByGameToken";
    public const string AccountGetSTokenByOldToken = $"{PassportApi}/account/ma-cn-session/app/getTokenBySToken";
    public const string AccountLoginByPassword = $"{PassportApi}/account/ma-cn-passport/app/loginByPassword";
    public const string AccountVerifyLtoken = $"{PassportApiV4}/account/ma-cn-session/web/verifyLtoken";
    public const string AccountCreateActionTicket = $"{PassportApi}/account/ma-cn-verifier/app/createActionTicketByToken";
    public const string AccountCreateLoginCaptcha = $"{PassportApi}/account/ma-cn-verifier/verifier/createLoginCaptcha";
    public const string AccountLoginByMobileCaptcha = $"{PassportApi}/account/ma-cn-passport/app/loginByMobileCaptcha";
    #endregion

    #region PublicDataApi
    public const string DeviceFpGetFp = $"{PublicDataApiDeviceFpApi}/getFp";

    public static string DeviceFpGetExtList(int platform)
    {
        return $"{PublicDataApiDeviceFpApi}/getExtList?platform={platform:D}";
    }
    #endregion

    #region PublicOperationHk4eGachaInfoApi
    public static string GachaInfoGetGachaLog(string query)
    {
        return $"{PublicOperationHk4eGachaInfoApi}/getGachaLog?{query}";
    }
    #endregion

    #region Hosts | Queries
    private const string ApiTakumi = "https://api-takumi.mihoyo.com";

    private const string ApiTakumiCardApi = $"{ApiTakumiRecord}/game_record/app/card/api";
    private const string ApiTakumiCardWApi = $"{ApiTakumiRecord}/game_record/app/card/wapi";

    private const string ApiTakumiEvent = $"{ApiTakumi}/event";
    private const string ApiTakumiEventCalculate = $"{ApiTakumiEvent}/e20200928calculate";
    private const string ApiTakumiEventLuna = $"{ApiTakumiEvent}/luna";

    private const string ApiTakumiRecord = "https://api-takumi-record.mihoyo.com";
    private const string ApiTakumiRecordApi = $"{ApiTakumiRecord}/game_record/app/genshin/api";
    private const string ApiTakumiRecordAApi = $"{ApiTakumiRecord}/game_record/app/genshin/aapi";

    private const string AppAuthApi = $"{AppMihoyoReferer}/account/auth/api";
    public const string AppMihoyoReferer = "https://app.mihoyo.com";

    private const string BbsApi = "https://bbs-api.mihoyo.com";
    private const string BbsApiUserWApi = $"{BbsApi}/user/wapi";

    private const string DownloaderApi = "https://downloader-api.mihoyo.com";
    private const string DownloaderSophonChunkApi = $"{DownloaderApi}/downloader/sophon_chunk/api";

    private const string Hk4eApi = "https://hk4e-api.mihoyo.com";
    private const string Hk4eApiAnnouncementApi = $"{Hk4eApi}/common/hk4e_cn/announcement/api";

    private const string Hk4eSdk = "https://hk4e-sdk.mihoyo.com";

    private const string HoyoPlayApi = "https://hyp-api.mihoyo.com";
    private const string HoyoPlayApiConnectApi = $"{HoyoPlayApi}/hyp/hyp-connect/api";

    private const string PassportApi = "https://passport-api.mihoyo.com";
    private const string PassportApiAuthApi = $"{PassportApi}/account/auth/api";
    private const string PassportApiV4 = "https://passport-api-v4.mihoyo.com";

    private const string PublicDataApi = "https://public-data-api.mihoyo.com";
    private const string PublicDataApiDeviceFpApi = $"{PublicDataApi}/device-fp/api";

    private const string PublicOperationHk4e = "https://public-operation-hk4e.mihoyo.com";
    private const string PublicOperationHk4eGachaInfoApi = $"{PublicOperationHk4e}/gacha_info/api";

    public const string WebStaticMihoyoReferer = "https://webstatic.mihoyo.com";

    private static string AnnouncementQuery(string languageCode, in Region region)
    {
        return $"game=hk4e&game_biz=hk4e_cn&lang={languageCode}&bundle_id=hk4e_cn&platform=pc&region={region}&level=55&uid=100000000";
    }
    #endregion
}