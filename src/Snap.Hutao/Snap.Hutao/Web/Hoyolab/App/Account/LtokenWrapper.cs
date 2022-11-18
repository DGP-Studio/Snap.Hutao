// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.App.Account;

/// <summary>
/// Ltoken 包装器
/// </summary>
public class LtokenWrapper
{
    /// <summary>
    /// Ltoken
    /// </summary>
    [JsonPropertyName("ltoken")]
    public string Ltoken { get; set; } = default!;
}