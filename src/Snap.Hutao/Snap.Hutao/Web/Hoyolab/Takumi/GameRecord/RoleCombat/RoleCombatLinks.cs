// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatLinks
{
    [JsonPropertyName("lineup_link")]
    public string LineupLink { get; set; } = default!;

    [JsonPropertyName("lineup_link_pc")]
    public string LineupLinkPC { get; set; } = default!;

    [JsonPropertyName("strategy_link")]
    public string StrategyLink { get; set; } = default!;

    [JsonPropertyName("lineup_publish_link")]
    public string LineupPublishLink { get; set; } = default!;

    [JsonPropertyName("lineup_publish_link_pc")]
    public string LineupPublishLinkPC { get; set; } = default!;
}