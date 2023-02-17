// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// Stoken包装
/// </summary>
[HighQuality]
internal sealed class TokenWrapper
{
    /// <summary>
    /// Stoken的类型为 1
    /// </summary>
    [JsonPropertyName("token_type")]
    public int TokenType { get; set; }

    /// <summary>
    /// Stoken
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = default!;
}