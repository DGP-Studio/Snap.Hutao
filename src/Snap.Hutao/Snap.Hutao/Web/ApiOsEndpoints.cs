// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Web;

/// <summary>
/// 国际服 API 端点
/// </summary>
[HighQuality]
[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1202")]
[SuppressMessage("", "SA1124")]
internal static class ApiOsEndpoints
{
    #region ApiAccountOsApi

    /// <summary>
    /// Hoyolab App Login api
    /// Can fetch stoken
    /// </summary>
    public const string WebLoginByPassword = $"{ApiAccountOsAuthApi}/webLoginByPassword";

    /// <summary>
    /// 获取 Ltoken
    /// </summary>
    public const string AccountGetLTokenBySToken = $"{ApiAccountOsAuthApi}/getLTokenBySToken";

    /// <summary>
    /// fetch CookieToken
    /// </summary>
    public const string AccountGetCookieTokenBySToken = $"{ApiAccountOsAuthApi}/getCookieAccountInfoBySToken";
    #endregion

    #region ApiGeetest

    /// <summary>
    /// 获取GT码
    /// </summary>
    /// <param name="gt">gt</param>
    /// <returns>GT码Url</returns>
    public static string GeetestGetType(string gt)
    {
        return $"{ApiNaGeetest}/gettype.php?gt={gt}";
    }

    /// <summary>
    /// 验证接口
    /// </summary>
    /// <param name="gt">gt</param>
    /// <param name="challenge">challenge流水号</param>
    /// <returns>验证接口Url</returns>
    public static string GeetestAjax(string gt, string challenge)
    {
        return $"{ApiNaGeetest}/ajax.php?gt={gt}&challenge={challenge}&lang=zh-cn&pt=0&client_type=web";
    }

    #endregion

    #region ApiOsTakumiAuthApi

    /// <summary>
    /// 获取 stoken 与 ltoken
    /// </summary>
    /// <param name="loginTicket">登录票证</param>
    /// <param name="loginUid">uid</param>
    /// <returns>Url</returns>
    public static string AuthMultiToken(string loginTicket, string loginUid)
    {
        return $"{ApiAccountOsAuthApi}/getMultiTokenByLoginTicket?login_ticket={loginTicket}&uid={loginUid}&token_types=3";
    }

    /// <summary>
    /// 获取 stoken 与 ltoken
    /// </summary>
    /// <param name="actionType">操作类型 game_role</param>
    /// <param name="stoken">SToken</param>
    /// <param name="uid">uid</param>
    /// <returns>Url</returns>
    public static string AuthActionTicket(string actionType, string stoken, string uid)
    {
        return $"{ApiAccountOsAuthApi}/getActionTicketBySToken?action_type={actionType}&stoken={Uri.EscapeDataString(stoken)}&uid={uid}";
    }

    #endregion

    #region ApiOsTakumiBindingApi

    /// <summary>
    /// 用户游戏角色
    /// </summary>
    /// <returns>用户游戏角色字符串</returns>
    public const string UserGameRolesByCookie = $"{ApiOsTakumiBindingApi}/getUserGameRolesByCookie?game_biz=hk4e_global";

    /// <summary>
    /// 用户游戏角色
    /// </summary>
    /// <param name="region">地区代号</param>
    /// <returns>用户游戏角色字符串</returns>
    public static string UserGameRolesByLtoken(string region)
    {
        return $"{ApiAccountOsBindingApi}/getUserGameRolesByLtoken?game_biz=hk4e_global&region={region}";
    }

    /// <summary>
    /// Game Authkey
    /// </summary>
    public const string BindingGenAuthKey = $"{ApiAccountOsBindingApi}/genAuthKey";
    #endregion

    #region BbsApiOsApi

    /// <summary>
    /// 查询其他用户详细信息
    /// </summary>
    /// <param name="bbsUid">bbs Uid</param>
    /// <returns>查询其他用户详细信息字符串</returns>
    public static string UserFullInfoQuery(string bbsUid)
    {
        return $"{BbsApiOs}/community/painter/wapi/user/full";
    }

    /// <summary>
    /// 用户详细信息
    /// </summary>
    public const string UserFullInfo = $"{BbsApiOs}/community/user/wapi/getUserFullInfo?gid=2";

    /// <summary>
    /// 国际服角色基本信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>角色基本信息字符串</returns>
    public static string GameRecordRoleBasicInfo(in PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordAppApi}/roleBasicInfo?role_id={uid.Value}&server={uid.Region}";
    }

    /// <summary>
    /// 国际服角色信息
    /// </summary>
    public const string GameRecordCharacter = $"{BbsApiOsGameRecordAppApi}/character";

    /// <summary>
    /// 国际服游戏记录实时便笺
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>游戏记录实时便笺字符串</returns>
    public static string GameRecordDailyNote(in PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordAppApi}/dailyNote?server={uid.Region}&role_id={uid.Value}";
    }

    /// <summary>
    /// 国际服游戏记录主页
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>游戏记录主页字符串</returns>
    public static string GameRecordIndex(in PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordAppApi}/index?server={uid.Region}&role_id={uid.Value}";
    }

    /// <summary>
    /// 国际服深渊信息
    /// </summary>
    /// <param name="scheduleType">深渊类型</param>
    /// <param name="uid">Uid</param>
    /// <returns>深渊信息字符串</returns>
    public static string GameRecordSpiralAbyss(Hoyolab.Takumi.GameRecord.SpiralAbyssSchedule scheduleType, in PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordAppApi}/spiralAbyss?server={uid.Region}&role_id={uid.Value}&schedule_type={(int)scheduleType}";
    }

    #endregion

    #region Hk4eApiOsGachaInfoApi

    /// <summary>
    /// 获取祈愿记录
    /// </summary>
    /// <param name="query">query string</param>
    /// <returns>祈愿记录信息Url</returns>
    public static string GachaInfoGetGachaLog(string query)
    {
        return $"{Hk4eApiOsGachaInfoApi}/getGachaLog?{query}";
    }
    #endregion

    #region SgPublicApi

    /// <summary>
    /// 计算器家具计算
    /// </summary>
    public const string CalculateFurnitureCompute = $"{SgPublicApi}/event/calculateos/furniture/list";

    /// <summary>
    /// 计算器角色列表 size 20
    /// </summary>
    public const string CalculateAvatarList = $"{SgPublicApi}/event/calculateos/avatar/list";

    /// <summary>
    /// 计算器武器列表 size 20
    /// </summary>
    public const string CalculateWeaponList = $"{SgPublicApi}/event/calculateos/weapon/list";

    /// <summary>
    /// 计算器结果
    /// </summary>
    public const string CalculateCompute = $"{SgPublicApi}/event/calculateos/compute";

    /// <summary>
    /// 计算器同步角色详情 size 20
    /// </summary>
    /// <param name="avatarId">角色Id</param>
    /// <param name="uid">uid</param>
    /// <returns>角色详情</returns>
    public static string CalculateSyncAvatarDetail(in AvatarId avatarId, in PlayerUid uid)
    {
        return $"{SgPublicApi}/event/calculateos/sync/avatar/detail?avatar_id={avatarId.Value}&uid={uid.Value}&region={uid.Region}";
    }

    /// <summary>
    /// 计算器同步角色列表 size 20
    /// </summary>
    public const string CalculateSyncAvatarList = $"{SgPublicApi}/event/calculateos/sync/avatar/list";

    #endregion

    #region SgHk4eApi

    /// <summary>
    /// 签到活动Id
    /// </summary>
    public const string SignInRewardActivityId = "e202102251931481";

    /// <summary>
    /// 签到
    /// </summary>
    public const string SignInRewardSign = $"{SgHk4eApi}/event/sol/sign?lang=zh-cn";

    /// <summary>
    /// 签到
    /// </summary>
    public const string SignInRewardHome = $"{SgHk4eApi}/event/sol/home?lang=zh-cn&act_id={SignInRewardActivityId}";

    /// <summary>
    /// 补签
    /// </summary>
    public const string SignInRewardReSign = $"{SgHk4eApi}/event/sol/resign?lang=zh-cn";

    /// <summary>
    /// 补签信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>补签信息字符串</returns>
    public static string SignInRewardResignInfo(in PlayerUid uid)
    {
        return $"{SgHk4eApi}/event/sol/resign_info?lang=zh-cn&act_id={SignInRewardActivityId}&region={uid.Region}&uid={uid.Value}";
    }

    /// <summary>
    /// 签到信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>签到信息字符串</returns>
    public static string SignInRewardInfo(in PlayerUid uid)
    {
        return $"{SgHk4eApi}/event/sol/info?lang=zh-cn&act_id={SignInRewardActivityId}&region={uid.Region}&uid={uid.Value}";
    }
    #endregion

    #region SdkStaticLauncherApi

    /// <summary>
    /// 启动器资源
    /// </summary>
    /// <param name="scheme">启动方案</param>
    /// <returns>启动器资源字符串</returns>
    public static string SdkOsStaticLauncherResource(LaunchScheme scheme)
    {
        return $"{SdkOsStaticLauncherApi}/resource?key={scheme.Key}&launcher_id={scheme.LauncherId}&channel_id={scheme.Channel:D}&sub_channel_id={scheme.SubChannel:D}";
    }
    #endregion

    #region WebApiOsAccountApi

    /// <summary>
    /// 使用 Cookie 登录
    /// </summary>
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
    private const string Hk4eApiOsGachaInfoApi = $"{Hk4eApiOs}/gacha_info/api";

    private const string SdkOsStatic = "https://sdk-os-static.mihoyo.com";
    private const string SdkOsStaticLauncherApi = $"{SdkOsStatic}/hk4e_global/mdk/launcher/api";

    private const string SgPublicApi = "https://sg-public-api.hoyoverse.com";
    private const string SgHk4eApi = "https://sg-hk4e-api.hoyoverse.com";

    private const string WebApiOs = "https://webapi-os.account.hoyoverse.com";
    private const string WebApiOsAccountApi = $"{WebApiOs}/Api";

    /// <summary>
    /// Web static referer
    /// </summary>
    public const string WebStaticSeaMihoyoReferer = "https://webstatic-sea.mihoyo.com";

    /// <summary>
    /// Act hoyolab referer
    /// </summary>
    public const string ActHoyolabReferer = "https://act.hoyolab.com/";

    /// <summary>
    /// App hoyolab referer
    /// </summary>
    public const string AppHoyolabReferer = "https://app.hoyolab.com/";

    #endregion
}