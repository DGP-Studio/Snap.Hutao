// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// Cookie 类型
/// </summary>
[Flags]
[HighQuality]
internal enum CookieType
{
    /// <summary>
    /// 不需要 Cookie
    /// </summary>
    None = 0B0000,

    /// <summary>
    /// 需要 CookieToken
    /// </summary>
    CookieToken = 0B0001,

    /// <summary>
    /// 需要 Ltoken
    /// </summary>
    Ltoken = 0B0010,

    /// <summary>
    /// 需要 CookieToken 与 Ltoken
    /// </summary>
    Cookie = CookieToken | Ltoken,

    /// <summary>
    /// 需要 Stoken
    /// </summary>
    Stoken = 0B0100,
}