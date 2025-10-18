// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombat
{
    [JsonPropertyName("data")]
    public required ImmutableArray<RoleCombatData> Data { get; init; }

    [JsonPropertyName("is_unlock")]
    public required bool IsUnlock { get; init; }

    [JsonPropertyName("links")]
    public required RoleCombatLinks Links { get; init; }

    [JsonPropertyName("tarot_card_state")]
    public TarotCardState? TarotCardState { get; set; }
}