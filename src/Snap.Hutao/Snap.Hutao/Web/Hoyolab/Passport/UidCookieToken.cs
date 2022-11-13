// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// uid 与 cookie token
/// </summary>
public class UidCookieToken
{
    /// <summary>
    /// Uid
    /// </summary>
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    /// <summary>
    /// CookieToken
    /// </summary>
    [JsonPropertyName("cookie_token")]
    public string CookieToken { get; set; } = default!;
}