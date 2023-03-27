// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// LToken 包装器
/// </summary>
[HighQuality]
internal sealed class LTokenWrapper
{
    /// <summary>
    /// LToken
    /// </summary>
    [JsonPropertyName("ltoken")]
    public string LToken { get; set; } = default!;
}