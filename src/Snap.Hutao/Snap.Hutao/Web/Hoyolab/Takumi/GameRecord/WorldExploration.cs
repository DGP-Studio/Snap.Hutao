// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

internal sealed class WorldExploration
{
    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("exploration_percentage")]
    public int ExplorationPercentage { get; set; }

    [JsonPropertyName("icon")]
    public Uri Icon { get; set; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WorldExplorationType Type { get; set; } = default!;

    [JsonPropertyName("offerings")]
    public List<Offering> Offerings { get; set; } = default!;

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("parent_id")]
    public int ParentId { get; set; }

    [JsonPropertyName("map_url")]
    public Uri MapUrl { get; set; } = default!;

    [JsonPropertyName("strategy_url")]
    public string StrategyUrl { get; set; } = default!;

    [JsonPropertyName("background_image")]
    public Uri BackgroundImage { get; set; } = default!;

    [JsonPropertyName("inner_icon")]
    public Uri InnerIcon { get; set; } = default!;

    [JsonPropertyName("cover")]
    public Uri Cover { get; set; } = default!;

    public double ExplorationPercentageBy10
    {
        get => ExplorationPercentage / 1000.0;
    }
}