// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class MobileCaptcha
{
    [JsonPropertyName("sent_new")]
    public bool SentNew { get; set; }

    [JsonPropertyName("countdown")]
    public int Countdown { get; set; }

    [JsonPropertyName("action_type")]
    public string ActionType { get; set; } = default!;
}