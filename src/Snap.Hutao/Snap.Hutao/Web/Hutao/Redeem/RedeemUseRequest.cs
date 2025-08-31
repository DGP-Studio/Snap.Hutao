// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Redeem;

internal sealed class RedeemUseRequest
{
    public RedeemUseRequest(string code)
    {
        Code = code;
    }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }
}