// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal sealed class NotifyDisable
{
    [JsonPropertyName("reply")]
    public bool Reply { get; set; }

    [JsonPropertyName("upvote")]
    public bool Upvote { get; set; }

    [JsonPropertyName("follow")]
    public bool Follow { get; set; }

    [JsonPropertyName("system")]
    public bool System { get; set; }

    [JsonPropertyName("chat")]
    public bool Chat { get; set; }
}
