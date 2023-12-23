// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Web;

/// <summary>
/// 国服 API 端点
/// </summary>
[HighQuality]
[SuppressMessage("", "SA1201")]
[SuppressMessage("", "SA1202")]
[SuppressMessage("", "SA1124")]
internal static class ApiEndpoints
{
    #region ApiTakumiAuthApi

    /// <summary>
    /// 获取 stoken 与 ltoken
    /// </summary>
    /// <param name="actionType">操作类型 game_role</param>
    /// <param name="stoken">SToken</param>
    /// <param name="uid">uid</param>
    /// <returns>Url</returns>
    public static string AuthActionTicket(string actionType, string stoken, string uid)
    {
        return $"{ApiTakumiAuthApi}/getActionTicketBySToken?action_type={actionType}&stoken={Uri.EscapeDataString(stoken)}&uid={uid}";
    }

    /// <summary>
    /// 获取 stoken 与 ltoken
    /// </summary>
    /// <param name="loginTicket">登录票证</param>
    /// <param name="loginUid">uid</param>
    /// <returns>Url</returns>
    public static string AuthMultiToken(string loginTicket, string loginUid)
    {
        return $"{ApiTakumiAuthApi}/getMultiTokenByLoginTicket?login_ticket={loginTicket}&uid={loginUid}&token_types=3";
    }

    #endregion

    #region ApiTaKumiBindingApi

    /// <summary>
    /// 用户游戏角色
    /// </summary>
    /// <param name="actionTicket">操作凭证</param>
    /// <returns>用户游戏角色字符串</returns>
    public static string UserGameRolesByActionTicket(string actionTicket)
    {
        return $"{ApiTaKumiBindingApi}/getUserGameRoles?action_ticket={actionTicket}&game_biz=hk4e_cn";
    }

    /// <summary>
    /// 用户游戏角色
    /// </summary>
    public const string UserGameRolesByCookie = $"{ApiTaKumiBindingApi}/getUserGameRolesByCookie?game_biz=hk4e_cn";

    /// <summary>
    /// 用户游戏角色
    /// </summary>
    public const string UserGameRolesBySToken = $"{ApiTaKumiBindingApi}/getUserGameRolesByStoken";

    /// <summary>
    /// AuthKey
    /// </summary>
    public const string BindingGenAuthKey = $"{ApiTaKumiBindingApi}/genAuthKey";
    #endregion

    #region ApiTakumiCardApi | ApiTakumiRecordApi

    /// <summary>
    /// 小组件数据
    /// </summary>
    public const string CardWidgetData = $"{ApiTakumiCardApi}/getWidgetData?game_id=2";

    /// <summary>
    /// 小组件数据v2
    /// </summary>
    public const string CardWidgetData2 = $"{ApiTakumiRecordAapi}/widget/v2?game_id=2";

    /// <summary>
    /// 发起验证码
    /// </summary>
    /// <param name="highRisk">是否为高风险</param>
    /// <returns>发起验证码Url</returns>
    public static string CardCreateVerification(bool highRisk)
    {
        return $"{ApiTakumiCardWApi}/createVerification?is_high={Core.StringLiterals.LowerBoolean(highRisk)}";
    }

    /// <summary>
    /// 验证验证码
    /// </summary>
    public const string CardVerifyVerification = $"{ApiTakumiCardWApi}/verifyVerification";

    /// <summary>
    /// 角色基本信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>角色基本信息字符串</returns>
    public static string GameRecordRoleBasicInfo(in PlayerUid uid)
    {
        return $"{ApiTakumiRecordApi}/roleBasicInfo?role_id={uid.Value}&server={uid.Region}";
    }

    /// <summary>
    /// 角色信息
    /// </summary>
    public const string GameRecordCharacter = $"{ApiTakumiRecordApi}/character";

    /// <summary>
    /// 游戏记录实时便笺
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>游戏记录实时便笺字符串</returns>
    public static string GameRecordDailyNote(in PlayerUid uid)
    {
        return $"{GameRecordDailyNotePath}?server={uid.Region}&role_id={uid.Value}";
    }

    public const string GameRecordDailyNotePath = $"{ApiTakumiRecordApi}/dailyNote";

    /// <summary>
    /// 游戏记录主页
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>游戏记录主页字符串</returns>
    public static string GameRecordIndex(in PlayerUid uid)
    {
        return $"{GameRecordIndexPath}?server={uid.Region}&role_id={uid.Value}";
    }

    public const string GameRecordIndexPath = $"{ApiTakumiRecordApi}/index";

    /// <summary>
    /// 深渊信息
    /// </summary>
    /// <param name="scheduleType">深渊类型</param>
    /// <param name="uid">Uid</param>
    /// <returns>深渊信息字符串</returns>
    public static string GameRecordSpiralAbyss(Hoyolab.Takumi.GameRecord.SpiralAbyssSchedule scheduleType, in PlayerUid uid)
    {
        return $"{GameRecordSpiralAbyssPath}?schedule_type={(int)scheduleType}&role_id={uid.Value}&server={uid.Region}";
    }

    public const string GameRecordSpiralAbyssPath = $"{ApiTakumiRecordApi}/spiralAbyss";
    #endregion

    #region ApiTakumiEventCalculate

    /// <summary>
    /// 计算器角色列表 size 20
    /// </summary>
    public const string CalculateAvatarList = $"{ApiTakumiEventCalculate}/v1/avatar/list";

    /// <summary>
    /// 计算器角色技能列表
    /// </summary>
    /// <param name="avatar">元素类型</param>
    /// <returns>技能列表</returns>
    public static string CalculateAvatarSkillList(Hoyolab.Takumi.Event.Calculate.Avatar avatar)
    {
        return $"{ApiTakumiEventCalculate}/v1/avatarSkill/list?avatar_id={avatar.Id}&element_attr_id={(int)avatar.ElementAttrId}";
    }

    /// <summary>
    /// 计算器结果
    /// </summary>
    public const string CalculateCompute = $"{ApiTakumiEventCalculate}/v2/compute";

    /// <summary>
    /// 计算器洞天摹本
    /// </summary>
    /// <param name="shareCode">分享码</param>
    /// <returns>洞天摹本</returns>
    public static string CalculateFurnitureBlueprint(string shareCode)
    {
        // &region=cn_gf01
        // ignored
        return $"{ApiTakumiEventCalculate}/v1/furniture/blueprint?share_code={shareCode}";
    }

    /// <summary>
    /// 计算器家具计算
    /// </summary>
    public const string CalculateFurnitureCompute = $"{ApiTakumiEventCalculate}/v1/furniture/compute";

    /// <summary>
    /// 计算器家具列表 size 32
    /// </summary>
    public const string CalculateFurnitureList = $"{ApiTakumiEventCalculate}/v1/furniture/list";

    /// <summary>
    /// 计算器同步角色详情 size 20
    /// </summary>
    /// <param name="avatarId">角色Id</param>
    /// <param name="uid">uid</param>
    /// <returns>角色详情</returns>
    public static string CalculateSyncAvatarDetail(in AvatarId avatarId, in PlayerUid uid)
    {
        return $"{ApiTakumiEventCalculate}/v1/sync/avatar/detail?avatar_id={avatarId.Value}&uid={uid.Value}&region={uid.Region}";
    }

    /// <summary>
    /// 计算器同步角色列表 size 20
    /// </summary>
    public const string CalculateSyncAvatarList = $"{ApiTakumiEventCalculate}/v1/sync/avatar/list";

    /// <summary>
    /// 计算器武器列表 size 20
    /// </summary>
    public const string CalculateWeaponList = $"{ApiTakumiEventCalculate}/v1/weapon/list";
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

    /// <summary>
    /// 另一个AuthKey
    /// </summary>
    public const string AppAuthGenAuthKey = $"{AppAuthApi}/genAuthKey";
    #endregion

    #region BbsApiUserApi

    /// <summary>
    /// BBS 指向引用
    /// </summary>
    public const string BbsReferer = "https://bbs.mihoyo.com/";

    /// <summary>
    /// 用户详细信息
    /// </summary>
    public const string UserFullInfo = $"{BbsApiUserApi}/getUserFullInfo?gids=2";

    /// <summary>
    /// 查询其他用户详细信息
    /// </summary>
    /// <param name="bbsUid">bbs Uid</param>
    /// <returns>查询其他用户详细信息字符串</returns>
    public static string UserFullInfoQuery(string bbsUid)
    {
        return $"{BbsApiUserApi}/getUserFullInfo?uid={bbsUid}&gids=2";
    }
    #endregion

    #region Hk4eApiAnnouncementApi

    /// <summary>
    /// 公告列表
    /// </summary>
    /// <param name="languageCode">语言代码</param>
    /// <param name="region">服务器</param>
    /// <returns>公告列表Url</returns>
    public static string AnnList(string languageCode, in Region region)
    {
        return $"{Hk4eApiAnnouncementApi}/getAnnList?{AnnouncementQuery(languageCode, region)}";
    }

    /// <summary>
    /// 公告内容
    /// </summary>
    /// <param name="languageCode">语言代码</param>
    /// <param name="region">服务器</param>
    /// <returns>公告列表Url</returns>
    public static string AnnContent(string languageCode, in Region region)
    {
        return $"{Hk4eApiAnnouncementApi}/getAnnContent?{AnnouncementQuery(languageCode, region)}";
    }
    #endregion

    #region Hk4eSdk

    public const string QrCodeFetch = $"{Hk4eSdk}/hk4e_cn/combo/panda/qrcode/fetch";

    public const string QrCodeQuery = $"{Hk4eSdk}/hk4e_cn/combo/panda/qrcode/query";

    #endregion

    #region PassportApi | PassportApiV4

    /// <summary>
    /// 获取 CookieToken
    /// </summary>
    public const string AccountGetCookieTokenBySToken = $"{PassportApiAuthApi}/getCookieAccountInfoBySToken";

    /// <summary>
    /// 获取LToken
    /// </summary>
    public const string AccountGetLTokenBySToken = $"{PassportApiAuthApi}/getLTokenBySToken";

    /// <summary>
    /// 通过GameToken获取V2SToken
    /// </summary>
    public const string AccountGetSTokenByGameToken = $"{PassportApi}/account/ma-cn-session/app/getTokenByGameToken";

    /// <summary>
    /// 获取V2SToken
    /// </summary>
    public const string AccountGetSTokenByOldToken = $"{PassportApi}/account/ma-cn-session/app/getTokenBySToken";

    /// <summary>
    /// 登录
    /// </summary>
    public const string AccountLoginByPassword = $"{PassportApi}/account/ma-cn-passport/app/loginByPassword";

    /// <summary>
    /// 验证 Ltoken 有效性
    /// </summary>
    public const string AccountVerifyLtoken = $"{PassportApiV4}/account/ma-cn-session/web/verifyLtoken";

    /// <summary>
    /// 创建 ActionTicket
    /// </summary>
    public const string AccountCreateActionTicket = $"{PassportApi}/account/ma-cn-verifier/app/createActionTicketByToken";
    #endregion

    #region PublicDataApi

    /// <summary>
    /// 获取 fingerprint
    /// </summary>
    public const string DeviceFpGetFp = $"{PublicDataApiDeviceFpApi}/getFp";

    public static string DeviceFpGetExtList(int platform)
    {
        return $"{PublicDataApiDeviceFpApi}/getExtList?platform={platform:D}";
    }
    #endregion

    #region PublicOperationHk4eGachaInfoApi

    /// <summary>
    /// 获取祈愿记录
    /// </summary>
    /// <param name="query">query string</param>
    /// <returns>祈愿记录信息Url</returns>
    public static string GachaInfoGetGachaLog(string query)
    {
        return $"{PublicOperationHk4eGachaInfoApi}/getGachaLog?{query}";
    }
    #endregion

    #region SdkStaticLauncherApi

    /// <summary>
    /// 启动器资源
    /// </summary>
    /// <param name="scheme">启动方案</param>
    /// <returns>启动器资源字符串</returns>
    public static string SdkStaticLauncherResource(LaunchScheme scheme)
    {
        return $"{SdkStaticLauncherApi}/resource?key={scheme.Key}&launcher_id={scheme.LauncherId}&channel_id={scheme.Channel:D}&sub_channel_id={scheme.SubChannel:D}";
    }

    // https://sdk-static.mihoyo.com/hk4e_cn/mdk/launcher/api/content?filter_adv=true&key=eYd89JmJ&language=zh-cn&launcher_id=18
    // https://sdk-static.mihoyo.com/hk4e_cn/mdk/launcher/api/content?key=eYd89JmJ&language=zh-cn&launcher_id=18
    #endregion

    #region Hosts | Queries
    private const string ApiTakumi = "https://api-takumi.mihoyo.com";
    private const string ApiTakumiAuthApi = $"{ApiTakumi}/auth/api";
    private const string ApiTaKumiBindingApi = $"{ApiTakumi}/binding/api";

    private const string ApiTakumiCardApi = $"{ApiTakumiRecord}/game_record/app/card/api";
    private const string ApiTakumiCardWApi = $"{ApiTakumiRecord}/game_record/app/card/wapi";

    private const string ApiTakumiEvent = $"{ApiTakumi}/event";
    private const string ApiTakumiEventCalculate = $"{ApiTakumiEvent}/e20200928calculate";
    private const string ApiTakumiEventLuna = $"{ApiTakumiEvent}/luna";

    private const string ApiTakumiRecord = "https://api-takumi-record.mihoyo.com";
    private const string ApiTakumiRecordApi = $"{ApiTakumiRecord}/game_record/app/genshin/api";
    private const string ApiTakumiRecordAapi = $"{ApiTakumiRecord}/game_record/app/genshin/aapi";

    private const string AppAuthApi = $"{AppMihoyoReferer}/account/auth/api";
    public const string AppMihoyoReferer = "https://app.mihoyo.com";

    private const string BbsApi = "https://bbs-api.mihoyo.com";
    private const string BbsApiUserApi = $"{BbsApi}/user/wapi";

    private const string Hk4eApi = "https://hk4e-api.mihoyo.com";
    private const string Hk4eApiAnnouncementApi = $"{Hk4eApi}/common/hk4e_cn/announcement/api";

    private const string Hk4eSdk = "https://hk4e-sdk.mihoyo.com";

    private const string PassportApi = "https://passport-api.mihoyo.com";
    private const string PassportApiAuthApi = $"{PassportApi}/account/auth/api";
    private const string PassportApiV4 = "https://passport-api-v4.mihoyo.com";

    private const string PublicDataApi = "https://public-data-api.mihoyo.com";
    private const string PublicDataApiDeviceFpApi = $"{PublicDataApi}/device-fp/api";

    private const string PublicOperationHk4e = "https://public-operation-hk4e.mihoyo.com";
    private const string PublicOperationHk4eGachaInfoApi = $"{PublicOperationHk4e}/gacha_info/api";

    private const string SdkStatic = "https://sdk-static.mihoyo.com";
    private const string SdkStaticLauncherApi = $"{SdkStatic}/hk4e_cn/mdk/launcher/api";

    /// <summary>
    /// Referer: https://webstatic.mihoyo.com
    /// </summary>
    public const string WebStaticMihoyoReferer = "https://webstatic.mihoyo.com";

    private static string AnnouncementQuery(string languageCode, in Region region)
    {
        return $"game=hk4e&game_biz=hk4e_cn&lang={languageCode}&bundle_id=hk4e_cn&platform=pc&region={region}&level=55&uid=100000000";
    }
    #endregion
}