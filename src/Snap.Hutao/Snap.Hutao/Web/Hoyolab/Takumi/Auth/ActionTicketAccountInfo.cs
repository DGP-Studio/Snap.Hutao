// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Auth;

/// <summary>
/// 账户信息
/// </summary>
[SuppressMessage("", "SA1600")]
public class ActionTicketAccountInfo
{
    [JsonPropertyName("is_realname")]
    public bool IsRealname { get; set; }

    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = default!;

    [JsonPropertyName("safe_mobile")]
    public string SafeMobile { get; set; } = default!;

    [JsonPropertyName("account_id")]
    public string AccountId { get; set; } = default!;

    [JsonPropertyName("account_name")]
    public string AccountName { get; set; } = default!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    [JsonPropertyName("is_email_verify")]
    public bool IsEmailVerify { get; set; }

    [JsonPropertyName("area_code")]
    public string AreaCode { get; set; } = default!;

    [JsonPropertyName("safe_area_code")]
    public string SafeAreaCode { get; set; } = default!;

    [JsonPropertyName("real_name")]
    public string RealName { get; set; } = default!;

    [JsonPropertyName("identity_code")]
    public string IdentityCode { get; set; } = default!;

    [JsonPropertyName("create_time")]
    public string CreateTime { get; set; } = default!;

    [JsonPropertyName("create_ip")]
    public string CreateIp { get; set; } = default!;

    [JsonPropertyName("change_pwd_time")]
    public string ChangePwdTime { get; set; } = default!;

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    [JsonPropertyName("user_icon_id")]
    public int UserIconId { get; set; }

    [JsonPropertyName("safe_level")]
    public int SafeLevel { get; set; }

    [JsonPropertyName("black_endtime")]
    public string BlackEndtime { get; set; } = default!;

    [JsonPropertyName("black_note")]
    public string BlackNote { get; set; } = default!;

    [JsonPropertyName("gender")]
    public int Gender { get; set; }

    [JsonPropertyName("real_stat")]
    public int RealStat { get; set; }

    [JsonPropertyName("apple_name")]
    public string AppleName { get; set; } = default!;

    [JsonPropertyName("sony_name")]
    public string SonyName { get; set; } = default!;

    [JsonPropertyName("tap_name")]
    public string TapName { get; set; } = default!;

    [JsonPropertyName("reactivate_ticket")]
    public string ReactivateTicket { get; set; } = default!;
}