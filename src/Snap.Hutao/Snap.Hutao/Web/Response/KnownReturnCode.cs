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
    /// 尚未登录
    /// </summary>
    NotLoggedIn = -100,

    /// <summary>
    /// 验证密钥过期
    /// </summary>
    AuthKeyTimeOut = -101,

    /// <summary>
    /// Ok
    /// </summary>
    OK = 0,

    /// <summary>
    /// 未定义
    /// </summary>
    NotDefined = 7,

    /// <summary>
    /// 数据未公开
    /// </summary>
    DataIsNotPublicForTheUser = 10102,
}
