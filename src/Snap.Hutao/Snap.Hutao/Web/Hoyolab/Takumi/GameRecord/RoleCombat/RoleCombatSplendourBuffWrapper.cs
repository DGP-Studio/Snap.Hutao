// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatSplendourBuffWrapper
{
    [JsonPropertyName("buffs")]
    public List<RoleCombatSplendourBuff> Buffs { get; set; } = default!;

    [JsonPropertyName("summary")]
    public RoleCombatSplendourSummary Summary { get; set; } = default!;
}