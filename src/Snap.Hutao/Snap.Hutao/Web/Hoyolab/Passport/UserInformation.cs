// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class UserInformation
{
    [JsonPropertyName("aid")]
    public string Aid { get; set; } = default!;

    [JsonPropertyName("mid")]
    public string Mid { get; set; } = default!;

    [JsonPropertyName("account_name")]
    public string AccountName { get; set; } = default!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    [JsonPropertyName("is_email_verify")]
    public JsonElement IsEmailVerify { get; set; } = default!;

    [JsonPropertyName("area_code")]
    public string AreaCode { get; set; } = default!;

    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = default!;

    [JsonPropertyName("safe_area_code")]
    public string SafeAreaCode { get; set; } = default!;

    [JsonPropertyName("safe_mobile")]
    public string SafeMobile { get; set; } = default!;

    [JsonPropertyName("realname")]
    public string Realname { get; set; } = default!;

    [JsonPropertyName("identity_code")]
    public string IdentityCode { get; set; } = default!;

    [JsonPropertyName("rebind_area_code")]
    public string RebindAreaCode { get; set; } = default!;

    [JsonPropertyName("rebind_mobile")]
    public string RebindMobile { get; set; } = default!;

    [JsonPropertyName("rebind_mobile_time")]
    public string RebindMobileTime { get; set; } = default!;

    [JsonPropertyName("links")]
    public List<Link> Links { get; set; } = default!;
}
