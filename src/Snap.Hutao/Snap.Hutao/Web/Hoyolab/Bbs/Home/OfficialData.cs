// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class OfficialData
{
    [JsonPropertyName("post_id")]
    public required string PostId { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("date")]
    public required long Date { get; init; }

    [JsonPropertyName("label")]
    public required string Label { get; init; }

    [JsonPropertyName("is_top")]
    public required bool IsTop { get; init; }

    [JsonPropertyName("view_type")]
    public required int ViewType { get; init; }

    [JsonPropertyName("image_url")]
    public required string ImageUrl { get; init; }

    [JsonPropertyName("image")]
    public required OfficialDataImage Image { get; init; }
}