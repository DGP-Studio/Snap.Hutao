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
    /// 需要风险验证
    /// </summary>
    RET_NEED_RISK_VERIFY = -3235,

    /// <summary>
    /// 禁止
    /// </summary>
    RET_AIGIS_FAILED = -3202,

    /// <summary>
    /// Ban
    /// </summary>
    RET_BAN = -3201,

    /// <summary>
    /// 需要地理验证
    /// </summary>
    RET_NEED_AIGIS = -3101,

    /// <summary>
    /// 访问过于频繁
    /// </summary>
    VIsitTooFrequently = -110,

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
    /// Ok
    /// </summary>
    RET_SUCCESS = 0,

    /// <summary>
    /// 未定义
    /// </summary>
    NotDefined = 7,

    /// <summary>
    /// 账号有风险
    /// </summary>
    CODE1034 = 1034,

    /// <summary>
    /// 数据未公开
    /// </summary>
    DataIsNotPublicForTheUser = 10102,
}
