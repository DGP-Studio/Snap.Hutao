// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore.Sqlite.Query.Internal;
using Microsoft.UI.Xaml;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Web;

/// <summary>
/// 国际服 API 端点
/// </summary>
[HighQuality]
[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1124")]
internal static class ApiOsEndpoints
{
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
        return $"{ApiOsTakumiAuthApi}/getMultiTokenByLoginTicket?login_ticket={loginTicket}&uid={loginUid}&token_types=3";
    }

    /// <summary>
    /// 获取 stoken 与 ltoken
    /// </summary>
    /// <param name="actionType">操作类型 game_role</param>
    /// <param name="stoken">Stoken</param>
    /// <param name="uid">uid</param>
    /// <returns>Url</returns>
    public static string AuthActionTicket(string actionType, string stoken, string uid)
    {
        return $"{ApiAcountOsAuthApi}/getActionTicketBySToken?action_type={actionType}&stoken={Uri.EscapeDataString(stoken)}&uid={uid}";
    }

    #endregion

    #region ApiOsTaKumiApi

    /// <summary>
    /// 用户游戏角色
    /// </summary>
    /// <returns>用户游戏角色字符串</returns>
    public const string UserGameRolesByCookie = $"{ApiOsTaKumiBindingApi}/getUserGameRolesByCookie?game_biz=hk4e_global";

    /// <summary>
    /// 用户游戏角色
    /// </summary>
    /// <param name="region">地区代号</param>
    /// <returns>用户游戏角色字符串</returns>
    public static string UserGameRolesByLtoken(string region)
    {
        return $"{ApiAccountOsBindingApi}/getUserGameRolesByLtoken?game_biz=hk4e_global&region={region}";
    }

    #endregion

    #region SgPublicApi

    /// <summary>
    /// 计算器家具计算
    /// </summary>
    public const string CalculateOsFurnitureCompute = $"{SgPublicApi}/event/calculateos/furniture/list";

    /// <summary>
    /// 计算器角色列表 size 20
    /// </summary>
    public const string CalculateOsAvatarList = $"{SgPublicApi}/event/calculateos/avatar/list";

    /// <summary>
    /// 计算器武器列表 size 20
    /// </summary>
    public const string CalculateOsWeaponList = $"{SgPublicApi}/event/calculateos/weapon/list";

    /// <summary>
    /// 计算器结果
    /// </summary>
    public const string CalculateOsCompute = $"{SgPublicApi}/event/calculateos/compute";

    /// <summary>
    /// 计算器同步角色详情 size 20
    /// </summary>
    /// <param name="avatarId">角色Id</param>
    /// <param name="uid">uid</param>
    /// <returns>角色详情</returns>
    public static string CalculateOsSyncAvatarDetail(AvatarId avatarId, PlayerUid uid)
    {
        return $"{SgPublicApi}/event/calculateos/sync/avatar/detail?avatar_id={avatarId.Value}&uid={uid.Value}&region={uid.Region}";
    }

    /// <summary>
    /// 计算器同步角色列表 size 20
    /// </summary>
    public const string CalculateOsSyncAvatarList = $"{SgPublicApi}/event/calculateos/sync/avatar/list";

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
    /// 国际服角色基本信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>角色基本信息字符串</returns>
    public static string GameRecordRoleBasicInfo(PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordApi}/roleBasicInfo?role_id={uid.Value}&server={uid.Region}";
    }

    /// <summary>
    /// 国际服角色信息
    /// </summary>
    public const string GameRecordCharacter = $"{BbsApiOsGameRecordApi}/character";

    /// <summary>
    /// 国际服游戏记录实时便笺
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>游戏记录实时便笺字符串</returns>
    public static string GameRecordDailyNote(PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordApi}/dailyNote?server={uid.Region}&role_id={uid.Value}";
    }

    /// <summary>
    /// 国际服游戏记录主页
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>游戏记录主页字符串</returns>
    public static string GameRecordIndex(PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordApi}/index?server={uid.Region}&role_id={uid.Value}";
    }

    /// <summary>
    /// 国际服深渊信息
    /// </summary>
    /// <param name="scheduleType">深渊类型</param>
    /// <param name="uid">Uid</param>
    /// <returns>深渊信息字符串</returns>
    public static string GameRecordSpiralAbyss(Hoyolab.Takumi.GameRecord.SpiralAbyssSchedule scheduleType, PlayerUid uid)
    {
        return $"{BbsApiOsGameRecordApi}/spiralAbyss?schedule_type={(int)scheduleType}&role_id={uid.Value}&server={uid.Region}";
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

    #region PassportApi

    /// <summary>
    /// Hoyolab App Login api
    /// Can fetch stoken
    /// </summary>
    public const string WebLoginByPassword = $"{ApiAcountOsAuthApi}/webLoginByPassword";

    /// <summary>
    /// 获取 Ltoken
    /// </summary>
    public const string AccountGetLtokenByStoken = $"{ApiAcountOsAuthApi}/getLTokenBySToken";

    /// <summary>
    /// fetch CookieToken
    /// </summary>
    public const string AccountGetCookieTokenBySToken = $"{ApiAcountOsAuthApi}/getCookieAccountInfoBySToken";
    #endregion

    #region SdkStaticLauncherApi

    /// <summary>
    /// 启动器资源
    /// </summary>
    /// <param name="scheme">启动方案</param>
    /// <returns>启动器资源字符串</returns>
    public static string SdkOsStaticLauncherResource(LaunchScheme scheme)
    {
        return $"{SdkOsStaticLauncherApi}/resource?key={scheme.Key}&launcher_id={scheme.LauncherId}&channel_id={scheme.Channel}&sub_channel_id={scheme.SubChannel}";
    }
    #endregion

    #region Hosts | Queries
    private const string ApiNaGeetest = "https://api-na.geetest.com";

    private const string ApiOsTaKumi = "https://api-os-takumi.hoyoverse.com";
    private const string ApiOsTaKumiBindingApi = $"{ApiOsTaKumi}/binding/api";

    // 和上面的域名不同
    private const string ApiOsTakumiAuthApi = "https://api-os-takumi.mihoyo.com/auth/api/getMultiTokenByLoginTicket";

    private const string ApiAccountOs = "https://api-account-os.hoyolab.com";
    private const string ApiAccountOsBindingApi = $"{ApiAccountOs}/binding/api";
    private const string ApiAcountOsAuthApi = $"{ApiAccountOs}/account/auth/api";

    private const string BbsApiOs = "https://bbs-api-os.hoyolab.com";
    private const string BbsApiOsGameRecordApi = $"{BbsApiOs}/game_record/genshin/api";

    private const string Hk4eApiOs = "https://hk4e-api-os.hoyoverse.com";
    private const string Hk4eApiOsGachaInfoApi = $"{Hk4eApiOs}/event/gacha_info/api";

    private const string SdkOsStatic = "https://sdk-os-static.mihoyo.com";
    private const string SdkOsStaticLauncherApi = $"{SdkOsStatic}/hk4e_global/mdk/launcher/api";

    private const string SgPublicApi = "https://sg-public-api.hoyolab.com";

    /// <summary>
    /// Web static referer
    /// </summary>
    public const string WebStaticSeaMihoyoReferer = "https://webstatic-sea.mihoyo.com";
    public const string ActHoyolabReferer = "https://act.hoyolab.com/";

    #endregion
}