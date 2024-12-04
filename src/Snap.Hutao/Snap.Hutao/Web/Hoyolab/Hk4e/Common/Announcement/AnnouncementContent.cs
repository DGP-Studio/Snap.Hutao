// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

internal class AnnouncementContent
{
    [JsonPropertyName("ann_id")]
    public int AnnId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("subtitle")]
    public string Subtitle { get; set; } = default!;

    [JsonPropertyName("banner")]
    public string Banner { get; set; } = default!;

    [JsonPropertyName("content")]
    public string Content { get; set; } = default!;

    [JsonPropertyName("lang")]
    public string Lang { get; set; } = default!;
}