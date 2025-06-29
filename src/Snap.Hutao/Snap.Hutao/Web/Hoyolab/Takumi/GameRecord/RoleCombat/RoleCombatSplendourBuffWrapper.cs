// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatSplendourBuffWrapper
{
    [JsonPropertyName("summary")]
    public required RoleCombatSplendourBuffSummary Summary { get; init; }

    [JsonPropertyName("buffs")]
    public required ImmutableArray<RoleCombatSplendourBuff> Buffs { get; init; }
}