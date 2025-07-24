// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class Risk
{
    [JsonPropertyName("risk_ticket")]
    public required string RiskTicket { get; set; }

    [JsonPropertyName("verify_str")]
    public string? VerifyString { get; set; }
}