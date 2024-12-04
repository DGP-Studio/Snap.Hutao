// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

internal sealed class Home
{
    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("visit_num")]
    public int VisitNum { get; set; }

    [JsonPropertyName("comfort_num")]
    public int ComfortNum { get; set; }

    [JsonPropertyName("item_num")]
    public int ItemNum { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    [JsonPropertyName("comfort_level_name")]
    public string ComfortLevelName { get; set; } = default!;

    [JsonPropertyName("comfort_level_icon")]
    public string ComfortLevelIcon { get; set; } = default!;
}