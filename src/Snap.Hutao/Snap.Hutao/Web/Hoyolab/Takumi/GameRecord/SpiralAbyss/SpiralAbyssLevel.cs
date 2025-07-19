// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyssLevel
{
    [JsonPropertyName("index")]
    public required uint Index { get; init; }

    [JsonPropertyName("star")]
    public required int Star { get; init; }

    [JsonPropertyName("max_star")]
    public required int MaxStar { get; init; }

    [JsonPropertyName("battles")]
    public required ImmutableArray<SpiralAbyssBattle> Battles { get; init; }

    [JsonPropertyName("top_half_floor_monster")]
    public ImmutableArray<SpiralAbyssMonster>? TopHalfFloorMonster { get; init; }

    [JsonPropertyName("bottom_half_floor_monster")]
    public ImmutableArray<SpiralAbyssMonster>? BottomHalfFloorMonster { get; init; }
}