// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Account;

internal sealed class AccountInformation
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    [JsonPropertyName("mid")]
    public string Mid { get; set; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = default!;

    [JsonPropertyName("is_email_verify")]
    public bool IsEmailVerify { get; set; }

    [JsonPropertyName("realname")]
    public string Realname { get; set; } = default!;

    [JsonPropertyName("identity_card")]
    public string IdentityCard { get; set; } = default!;

    [JsonPropertyName("token_type")]
    public int TokenType { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; } = default!;

    [JsonPropertyName("stoken")]
    public string Stoken { get; set; } = default!;

    [JsonPropertyName("is_guest")]
    public bool IsGuest { get; set; }

    [JsonPropertyName("guest_id")]
    public string GuestId { get; set; } = default!;

    [JsonPropertyName("safe_mobile")]
    public string SafeMobile { get; set; } = default!;

    [JsonPropertyName("account")]
    public string Account { get; set; } = default!;

    [JsonPropertyName("is_login")]
    public bool IsLogin { get; set; }

    [JsonPropertyName("login_type")]
    public int LoginType { get; set; }

    [JsonPropertyName("payload")]
    public string Payload { get; set; } = default!;

    [JsonPropertyName("channel_id")]
    public int ChannelId { get; set; }

    [JsonPropertyName("asterisk_name")]
    public string AsteriskName { get; set; } = default!;

    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = default!;

    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; } = default!;

    [JsonPropertyName("country")]
    public string Country { get; set; } = default!;

    [JsonPropertyName("area_code")]
    public string AreaCode { get; set; } = default!;

    [JsonPropertyName("reactivate_ticket")]
    public string ReactivateTicket { get; set; } = default!;

    [JsonPropertyName("device_grant_ticket")]
    public string DeviceGrantTicket { get; set; } = default!;

    [JsonPropertyName("thirdLoginTimestamp")]
    public long ThirdLoginTimestamp { get; set; }

    [JsonPropertyName("account_display_type")]
    public string AccountDisplayType { get; set; } = default!;

    [JsonPropertyName("imageName")]
    public string ImageName { get; set; } = default!;

    [JsonPropertyName("loginPattern")]
    public int LoginPattern { get; set; }

    [JsonPropertyName("loginTime")]
    public long LoginTime { get; set; }

    [JsonPropertyName("agreeSaveAccount")]
    public bool AgreeSaveAccount { get; set; }

    [JsonPropertyName("emailLastLogin")]
    public bool EmailLastLogin { get; set; }

    [JsonPropertyName("authTicketThirdParty")]
    public int AuthTicketThirdParty { get; set; }

    [JsonPropertyName("links")]
    public List<JsonElement> Links { get; set; } = default!;

    [JsonPropertyName("agree_persistent_login_data")]
    public bool AgreePersistentLoginData { get; set; }
}