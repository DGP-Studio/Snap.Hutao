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
    LToken = 0B0010,

    /// <summary>
    /// 需要 CookieToken 与 LToken
    /// </summary>
    Cookie = CookieToken | LToken,

    /// <summary>
    /// 需要 SToken
    /// </summary>
    SToken = 0B0100,
}