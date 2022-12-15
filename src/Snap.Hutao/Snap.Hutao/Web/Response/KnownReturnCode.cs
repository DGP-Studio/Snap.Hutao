// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Response;

/// <summary>
/// 已知的返回代码
/// </summary>
public enum KnownReturnCode : int
{
    /// <summary>
    /// 无效请求
    /// </summary>
    InvalidRequest = -10001,

    /// <summary>
    /// 已经签到过了
    /// </summary>
    AlreadySignedIn = -5003,

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
    /// 登录信息已失效，请重新登录
    /// </summary>
    LoginDataOutdated = -262,

    /// <summary>
    /// 访问过于频繁
    /// </summary>
    VisitTooFrequently = -110,

    /// <summary>
    /// 应用Id错误
    /// </summary>
    AppIdError = -109,

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
    /// 账号有风险
    /// </summary>
    CODE1034 = 1034,

    /// <summary>
    /// 请登录
    /// </summary>
    PleaseLogin = 10001,

    /// <summary>
    /// 数据未公开
    /// </summary>
    DataIsNotPublicForTheUser = 10102,

    /// <summary>
    /// 实时便笺
    /// </summary>
    CODE10104 = 10104,
}
