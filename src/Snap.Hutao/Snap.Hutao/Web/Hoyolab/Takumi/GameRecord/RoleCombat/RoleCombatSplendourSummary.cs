// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatSplendourSummary
{
    [JsonPropertyName("desc")]
    public string Description { get; set; } = default!;

    [JsonPropertyName("total_level")]
    public uint TotalLevel { get; set; }
}