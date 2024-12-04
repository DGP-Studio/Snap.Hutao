// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class SignInResult
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = default!;

    [JsonPropertyName("risk_code")]
    public int RiskCode { get; set; }

    [JsonPropertyName("gt")]
    public string Gt { get; set; } = default!;

    [JsonPropertyName("challenge")]
    public string Challenge { get; set; } = default!;

    [JsonPropertyName("success")]
    public int Success { get; set; }

    [JsonPropertyName("is_risk")]
    public bool IsRisk { get; set; }
}