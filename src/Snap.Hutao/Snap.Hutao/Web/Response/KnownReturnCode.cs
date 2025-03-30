// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Response;

internal enum KnownReturnCode
{
    /// <summary>
    /// 用户不存在
    /// </summary>
    UserNotExist = -20001,

    /// <summary>
    /// 无效请求
    /// 因战绩功能服务优化升级，V2.10及以下版本将无法正常使用战绩功能，请更新米游社至最新版本再进行使用。
    /// </summary>
    InvalidRequest = -10001,

    /// <summary>
    /// 已经签到过了
    /// </summary>
    AlreadySignedIn = -5003,

    /// <summary>
    /// 补签资格已用完 / 补签卡已用完 (HoYoLAB)
    /// </summary>
    ResignQuotaUsedUp = -5005,

    /// <summary>
    /// 请先完成本日签到
    /// </summary>
    PleaseSignInFirst = -5007,

    /// <summary>
    /// 无可用补签日期
    /// </summary>
    NoAvailableResignDate = -5008,

    /// <summary>
    /// 米游币不足
    /// 网络出小差了，请稍后重试
    /// </summary>
    NotEnoughCoin = -5014,

    /// <summary>
    /// 请求失败，当前设备或网络环境存在风险
    /// </summary>
    CODEN3503 = -3503,

    /// <summary>
    /// 二维码已过期
    /// </summary>
    QRLoginExpired = -3501,

    /// <summary>
    /// 需要风险验证(闪验)
    /// </summary>
    RET_NEED_RISK_VERIFY = -3235,

    /// <summary>
    /// 验证码已失效，请重新获取
    /// </summary>
    VerifyCodeOutdated = -3209,

    /// <summary>
    /// 极验失败
    /// </summary>
    RET_AIGIS_FAILED = -3202,

    /// <summary>
    /// Ban
    /// </summary>
    RET_BAN = -3201,

    /// <summary>
    /// 需要极验
    /// </summary>
    RET_NEED_AIGIS = -3101,

    /// <summary>
    /// 参数不合法
    /// </summary>
    InvalidParameter = -3001,

    /// <summary>
    /// 请在米游社App内打开~
    /// </summary>
    PleaseOpenInBbsApp = -1104,

    /// <summary>
    /// 天赋等级超出限制~
    /// </summary>
    SkillLevelLimitExcceed = -1009,

    /// <summary>
    /// 服务器内部错误
    /// </summary>
    SomethingWentWrong = -502,

    /// <summary>
    /// 登录信息已失效，请重新登录
    /// </summary>
    LoginDataOutdated = -262,

    /// <summary>
    /// 无效的 Key
    /// </summary>
    InvalidKey = -205,

    /// <summary>
    /// 无效的参数
    /// </summary>
    ParameterError = -201,

    /// <summary>
    /// 访问过于频繁
    /// </summary>
    VisitTooFrequently = -110,

    /// <summary>
    /// 应用Id错误
    /// </summary>
    AppIdError = -109,

    /// <summary>
    /// 二维码已过期
    /// </summary>
    QrCodeExpired = -106,

    /// <summary>
    /// 验证密钥过期
    /// </summary>
    AuthKeyTimeOut = -101,

    /// <summary>
    /// 尚未登录
    /// </summary>
    RET_TOKEN_INVALID = -100,

    /// <summary>
    /// 数据不存在
    /// </summary>
    DataNotExists = -1,

    /// <summary>
    /// Ok
    /// </summary>
    RET_SUCCESS = 0,

    /// <summary>
    /// 未定义
    /// </summary>
    NotDefined = 7,

    /// <summary>
    /// 登录状态已失效，请重新登录
    /// </summary>
    LoginStateInvalid = 1004,

    /// <summary>
    /// 实时便笺 账号有风险
    /// </summary>
    CODE1034 = 1034,

    /// <summary>
    /// Hoyolab 登录失败
    /// </summary>
    SignInError = 2001,

    /// <summary>
    /// 实时便笺 当前账号存在风险，暂无数据
    /// </summary>
    CODE5003 = 5003,

    /// <summary>
    /// 请登录 登录后可查看战绩信息
    /// </summary>
    PleaseLogin = 10001,

    /// <summary>
    /// 原神战绩 查看他人战绩次数过多，请休息一会儿再试
    /// </summary>
    CODE10101 = 10101,

    /// <summary>
    /// 数据未公开
    /// </summary>
    DataIsNotPublicForTheUser = 10102,

    /// <summary>
    /// 实时便笺 你的账号已被封禁，无法查看
    /// </summary>
    CODE10103 = 10103,

    /// <summary>
    /// 实时便笺
    /// </summary>
    CODE10104 = 10104,
}