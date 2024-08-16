// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class NewHomeNewInfo
{
    [JsonPropertyName("navigator")]
    public List<AppNavigator> Navigator { get; set; } = default!;

    [JsonPropertyName("discussion")]
    public Discussion Discussion { get; set; } = default!;

    [JsonPropertyName("background")]
    public Background Background { get; set; } = default!;

    [JsonPropertyName("official")]
    public Official Official { get; set; } = default!;

    [JsonPropertyName("carousels")]
    public Carousels Carousels { get; set; } = default!;

    [JsonPropertyName("hot_topics")]
    public JsonElement HotTopics { get; set; } = default!;

    [JsonPropertyName("game_receptions")]
    public JsonElement GameReceptions { get; set; } = default!;

    [JsonPropertyName("posts")]
    public JsonElement Posts { get; set; } = default!;

    [JsonPropertyName("lives")]
    public JsonElement Lives { get; set; } = default!;

    [JsonPropertyName("recommend_villa")]
    public JsonElement RecommendVilla { get; set; }

    [JsonPropertyName("image_post_card")]
    public JsonElement ImagePostCard { get; set; } = default!;

    [JsonPropertyName("link_card")]
    public JsonElement LinkCard { get; set; } = default!;
}