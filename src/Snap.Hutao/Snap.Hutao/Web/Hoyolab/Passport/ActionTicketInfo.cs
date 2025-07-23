// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class ActionTicketInfo
{
    [JsonPropertyName("action_ticket")]
    public required string ActionTicket { get; set; }

    [JsonPropertyName("verify_info")]
    public VerifyInfo VerifyInfo { get; set; } = default!;

    [JsonPropertyName("user_info")]
    public UserInformation UserInfo { get; set; } = default!;

    [JsonPropertyName("captcha_sent")]
    public bool CaptchaSent { get; set; }
}