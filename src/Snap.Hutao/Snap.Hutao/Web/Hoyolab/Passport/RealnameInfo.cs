// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class RealnameInfo
{
    [JsonPropertyName("required")]
    public bool Required { get; set; }

    [JsonPropertyName("action_type")]
    public string ActionType { get; set; } = default!;

    [JsonPropertyName("action_ticket")]
    public string ActionTicket { get; set; } = default!;
}