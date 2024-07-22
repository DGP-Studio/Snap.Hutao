// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Web;

[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1202")]
internal static class ApiOsEndpoints
{
    #region ApiAccountOsApi
    public const string WebLoginByPassword = $"{ApiAccountOsAuthApi}/webLoginByPassword";
    public const string AccountGetLTokenBySToken = $"{ApiAccountOsAuthApi}/getLTokenBySToken";
    public const string AccountGetCookieTokenBySToken = $"{ApiAccountOsAuthApi}/getCookieAccountInfoBySToken";
    #endregion

    #region ApiOsTakumiAuthApi
    public static string AuthMultiToken(string loginTicket, string loginUid)
    {
        return $"{ApiAccountOsAuthApi}/getMultiTokenByLoginTicket?login_ticket={loginTicket}&uid={loginUid}&token_types=3";
    }

    public static string AuthActionTicket(string actionType, string stoken, string uid)
    {
        return $"{ApiAccountOsAuthApi}/getActionTicketBySToken?action_type={actionType}&stoken={Uri.EscapeDataString(stoken)}&uid={uid}";
    }
    #endregion

    #region ApiOsTakumiBindingApi
    public const string UserGameRolesByCookie = $"{ApiOsTakumiBindingApi}/getUserGameRolesByCookie?game_biz=hk4e_global";

    public static string UserGameRolesByLtoken(in Region region)
    {
        return $"{ApiAccountOsBindingApi}/getUserGameRolesByLtoken?game_biz=hk4e_global&region={region}";
    }

    public const string BindingGenAuthKey = $"{ApiAccountOsBindingApi}/genAuthKey";
    #endregion

    #region BbsApiOsApi
    public static string UserFullInfoQuery(string bbsUid)
    {
        return $"{BbsApiOs}/community/painter/wapi/user/full";
    }

    public const string UserFullInfo = $"{BbsApiOs}/community/user/wapi/getUserFullInfo?gid=2";

    public static string GameRecordRoleBasicInfo(in PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordAppApi}/roleBasicInfo?role_id={uid.Value}&server={uid.Region}";
    }

    public const string GameRecordCharacter = $"{BbsApiOsGameRecordAppApi}/character";

    public static string GameRecordDailyNote(in PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordAppApi}/dailyNote?server={uid.Region}&role_id={uid.Value}";
    }

    public static string GameRecordIndex(in PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordAppApi}/index?server={uid.Region}&role_id={uid.Value}";
    }

    public static string GameRecordSpiralAbyss(Hoyolab.Takumi.GameRecord.ScheduleType scheduleType, in PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordAppApi}/spiralAbyss?server={uid.Region}&role_id={uid.Value}&schedule_type={(int)scheduleType}";
    }
    #endregion

    #region Hk4eApiOsGachaInfoApi
    public static string GachaInfoGetGachaLog(string query)
    {
        return $"{Hk4eApiOsGachaInfoApi}/getGachaLog?{query}";
    }
    #endregion

    #region Hk4eApiOsAnnouncementApi
    public static string AnnList(string languageCode, in Region region)
    {
        return $"{Hk4eApiOsAnnouncementApi}/getAnnList?{AnnouncementQuery(languageCode, region)}";
    }

    public static string AnnContent(string languageCode, in Region region)
    {
        return $"{Hk4eApiOsAnnouncementApi}/getAnnContent?{AnnouncementQuery(languageCode, region)}";
    }

    #endregion

    #region SgPublicApi
    public const string CalculateFurnitureCompute = $"{SgPublicApi}/event/calculateos/furniture/list";
    public const string CalculateAvatarList = $"{SgPublicApi}/event/calculateos/avatar/list";
    public const string CalculateWeaponList = $"{SgPublicApi}/event/calculateos/weapon/list";
    public const string CalculateCompute = $"{SgPublicApi}/event/calculateos/compute";
    public const string CalculateBatchCompute = $"{SgPublicApi}/event/calculateos/batch_compute";

    public static string CalculateSyncAvatarDetail(in AvatarId avatarId, in PlayerUid uid)
    {
        return $"{SgPublicApi}/event/calculateos/sync/avatar/detail?avatar_id={avatarId.Value}&uid={uid.Value}&region={uid.Region}";
    }

    public const string CalculateSyncAvatarList = $"{SgPublicApi}/event/calculateos/sync/avatar/list";
    #endregion

    #region SgHk4eApi
    public const string SignInRewardActivityId = "e202102251931481";
    public const string SignInRewardSign = $"{SgHk4eApi}/event/sol/sign?lang=zh-cn";
    public const string SignInRewardHome = $"{SgHk4eApi}/event/sol/home?lang=zh-cn&act_id={SignInRewardActivityId}";
    public const string SignInRewardReSign = $"{SgHk4eApi}/event/sol/resign?lang=zh-cn";

    public static string SignInRewardResignInfo(in PlayerUid uid)
    {
        return $"{SgHk4eApi}/event/sol/resign_info?lang=zh-cn&act_id={SignInRewardActivityId}&region={uid.Region}&uid={uid.Value}";
    }

    public static string SignInRewardInfo(in PlayerUid uid)
    {
        return $"{SgHk4eApi}/event/sol/info?lang=zh-cn&act_id={SignInRewardActivityId}&region={uid.Region}&uid={uid.Value}";
    }
    #endregion

    #region SgHoyoPlayApi
    public static string HoyoPlayConnectGamePackages(LaunchScheme scheme)
    {
        return $"{SgHoyoPlayApiConnectApi}/getGamePackages?game_ids[]={scheme.GameId}&launcher_id={scheme.LauncherId}";
    }

    public static string HoyoPlayConnectGameChannelSDKs(LaunchScheme scheme)
    {
        return $"{SgHoyoPlayApiConnectApi}/getGameChannelSDKs?channel={scheme.Channel:D}&game_ids[]={scheme.GameId}&launcher_id={scheme.LauncherId}&sub_channel={scheme.SubChannel:D}";
    }

    public static string HoyoPlayConnectDeprecatedFileConfigs(LaunchScheme scheme)
    {
        return $"{SgHoyoPlayApiConnectApi}/getGameDeprecatedFileConfigs?channel={scheme.Channel:D}&game_ids[]={scheme.GameId}&launcher_id={scheme.LauncherId}&sub_channel={scheme.SubChannel:D}";
    }
    #endregion

    #region WebApiOsAccountApi
    public const string WebApiOsAccountLoginByCookie = $"{WebApiOsAccountApi}/login_by_cookie";
    #endregion

    #region Hosts | Queries
    private const string ApiNaGeetest = "https://api-na.geetest.com";

    private const string ApiOsTakumi = "https://api-os-takumi.hoyoverse.com";
    private const string ApiOsTakumiBindingApi = $"{ApiOsTakumi}/binding/api";

    private const string ApiAccountOs = "https://api-account-os.hoyoverse.com";
    private const string ApiAccountOsBindingApi = $"{ApiAccountOs}/binding/api";
    private const string ApiAccountOsAuthApi = $"{ApiAccountOs}/account/auth/api";

    private const string BbsApiOs = "https://bbs-api-os.hoyoverse.com";
    private const string BbsApiOsGameRecordAppApi = $"{BbsApiOs}/game_record/app/genshin/api";

    private const string Hk4eApiOs = "https://hk4e-api-os.hoyoverse.com";
    private const string Hk4eApiOsAnnouncementApi = $"{Hk4eApiOs}/common/hk4e_global/announcement/api";
    private const string Hk4eApiOsGachaInfoApi = $"{Hk4eApiOs}/gacha_info/api";

    private const string SgPublicApi = "https://sg-public-api.hoyoverse.com";
    private const string SgHk4eApi = "https://sg-hk4e-api.hoyoverse.com";

    private const string SgHoyoPlayApi = "https://sg-hyp-api.hoyoverse.com";
    private const string SgHoyoPlayApiConnectApi = $"{SgHoyoPlayApi}/hyp/hyp-connect/api";

    private const string WebApiOs = "https://webapi-os.account.hoyoverse.com";
    private const string WebApiOsAccountApi = $"{WebApiOs}/Api";

    public const string WebStaticSeaMihoyoReferer = "https://webstatic-sea.mihoyo.com";

    public const string ActHoyolabReferer = "https://act.hoyolab.com/";

    public const string AppHoyolabReferer = "https://app.hoyolab.com/";

    private static string AnnouncementQuery(string languageCode, in Region region)
    {
        return $"game=hk4e&game_biz=hk4e_global&lang={languageCode}&bundle_id=hk4e_global&platform=pc&region={region}&level=55&uid=100000000";
    }
    #endregion
}