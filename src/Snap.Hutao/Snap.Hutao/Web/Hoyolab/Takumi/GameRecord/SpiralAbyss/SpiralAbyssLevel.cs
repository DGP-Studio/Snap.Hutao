// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyssLevel
{
    [JsonPropertyName("index")]
    public uint Index { get; set; }

    [JsonPropertyName("star")]
    public int Star { get; set; }

    [JsonPropertyName("max_star")]
    public int MaxStar { get; set; }

    [JsonPropertyName("battles")]
    public List<SpiralAbyssBattle> Battles { get; set; } = default!;

    [JsonPropertyName("top_half_floor_monster")]
    public List<SpiralAbyssMonster> TopHalfFloorMonster { get; set; } = default!;

    [JsonPropertyName("bottom_half_floor_monster")]
    public List<SpiralAbyssMonster> BottomHalfFloorMonster { get; set; } = default!;
}