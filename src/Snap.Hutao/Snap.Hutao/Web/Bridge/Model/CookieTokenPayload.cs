// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

/// <summary>
/// 获取CookieToken的请求
/// </summary>
public class CookieTokenPayload
{
    /// <summary>
    /// 强制刷新
    /// </summary>
    [JsonPropertyName("forceRefresh")]
    public bool ForceRefresh { get; set; }
}
