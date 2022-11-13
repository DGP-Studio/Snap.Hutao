// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab;

/// <summary>
/// Cookie 类型
/// </summary>
public enum CookieType
{
    /// <summary>
    /// 不需要 Cookie
    /// </summary>
    None,

    /// <summary>
    /// 需要 Ltoken &amp; CookieToken &amp; e_hk4e_token
    /// </summary>
    Ltoken,

    /// <summary>
    /// 需要 Stoken
    /// </summary>
    Stoken,

    /// <summary>
    /// 全部
    /// </summary>
    All,
}