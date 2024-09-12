// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class OfficialData
{
    [JsonPropertyName("post_id")]
    public string PostId { get; set; } = default!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("date")]
    public long Date { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; } = default!;

    [JsonPropertyName("is_top")]
    public bool IsTop { get; set; }

    [JsonPropertyName("view_type")]
    public int ViewType { get; set; } = default!;

    [JsonPropertyName("image_url")]
    public string ImageUrl { get; set; } = default!;
}
