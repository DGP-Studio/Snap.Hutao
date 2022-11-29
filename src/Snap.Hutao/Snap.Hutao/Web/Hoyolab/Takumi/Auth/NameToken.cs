// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Auth;

/// <summary>
/// 名称与令牌
/// </summary>
public sealed class NameToken
{
    /// <summary>
    /// Token名称
    /// stoken
    /// ltoken
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 令牌
    /// </summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = default!;
}