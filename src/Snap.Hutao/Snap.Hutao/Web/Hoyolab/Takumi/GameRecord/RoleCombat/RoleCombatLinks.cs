// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatLinks
{
    [JsonPropertyName("lineup_link")]
    public required string LineupLink { get; init; }

    [JsonPropertyName("lineup_link_pc")]
    public required string LineupLinkPC { get; init; }

    [JsonPropertyName("strategy_link")]
    public required string StrategyLink { get; init; }

    [JsonPropertyName("lineup_publish_link")]
    public required string LineupPublishLink { get; init; }

    [JsonPropertyName("lineup_publish_link_pc")]
    public required string LineupPublishLinkPC { get; init; }
}