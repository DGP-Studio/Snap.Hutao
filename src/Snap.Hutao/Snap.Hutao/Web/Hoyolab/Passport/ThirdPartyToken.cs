// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class ThirdPartyToken
{
    [JsonPropertyName("thirdparty_type")]
    public string ThirdPartyType { get; set; } = default!;

    [JsonPropertyName("token")]
    public string Token { get; set; } = default!;
}