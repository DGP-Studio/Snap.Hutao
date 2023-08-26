// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

/// <summary>
/// 签到结果
/// </summary>
internal sealed class SignInResult
{
    /// <summary>
    /// 通常是 ""
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = default!;

    /// <summary>
    /// 通常是 0
    /// </summary>
    [JsonPropertyName("risk_code")]
    public int RiskCode { get; set; }

    /// <summary>
    /// 通常是 ""
    /// </summary>
    [JsonPropertyName("gt")]
    public string Gt { get; set; } = default!;

    /// <summary>
    /// 通常是 ""
    /// </summary>
    [JsonPropertyName("challenge")]
    public string Challenge { get; set; } = default!;

    /// <summary>
    /// 通常是 ""
    /// </summary>
    [JsonPropertyName("success")]
    public int Success { get; set; }
}