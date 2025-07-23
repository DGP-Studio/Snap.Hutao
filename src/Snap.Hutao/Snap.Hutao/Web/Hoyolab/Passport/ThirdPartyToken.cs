// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class ThirdPartyToken : IVerifyProvider
{
    public ThirdPartyToken(string thirdPartyType, string token)
    {
        ThirdPartyType = thirdPartyType;
        Token = token;
    }

    [JsonPropertyName("thirdparty_type")]
    public string ThirdPartyType { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonIgnore]
    public string? Verify { get; set; }
}