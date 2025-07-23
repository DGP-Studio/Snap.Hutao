// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class ActionTicketInfoRequestOversea
{
    [JsonPropertyName("action_type")]
    public string ActionType { get; set; } = "verify_for_component";

    [JsonPropertyName("action_ticket")]
    public string ActionTicket { get; set; } = default!;

    [JsonPropertyName("email_captcha")]
    public string? EmailCaptcha { get; set; }

    [JsonPropertyName("verify_method")]
    public int? VerifyMethod { get; set; }
}