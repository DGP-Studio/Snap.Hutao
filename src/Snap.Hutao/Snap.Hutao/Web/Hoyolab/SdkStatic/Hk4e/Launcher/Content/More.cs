// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher.Content;

internal sealed class More
{
    [JsonPropertyName("activity_link")]
    public string ActivityLink { get; set; } = default!;

    [JsonPropertyName("announce_link")]
    public string AnnounceLink { get; set; } = default!;

    [JsonPropertyName("info_link")]
    public string InfoLink { get; set; } = default!;

    [JsonPropertyName("news_link")]
    public string NewsLink { get; set; } = default!;

    [JsonPropertyName("trends_link")]
    public string TrendsLink { get; set; } = default!;

    [JsonPropertyName("supply_link")]
    public string SupplyLink { get; set; } = default!;

    [JsonPropertyName("tools_link")]
    public string ToolsLink { get; set; } = default!;
}