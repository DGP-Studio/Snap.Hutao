// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Response;

/// <summary>
/// 已知的返回代码
/// </summary>
public enum KnownReturnCode
{
    /// <summary>
    /// Json 异常
    /// </summary>
    JsonParseIssue = -2000000002,

    /// <summary>
    /// Url为 空
    /// </summary>
    UrlIsEmpty = -2000000001,

    /// <summary>
    /// 内部错误
    /// </summary>
    InternalFailure = -2000000000,

    /// <summary>
    /// 已经签到过了
    /// </summary>
    AlreadySignedIn = -5003,

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
