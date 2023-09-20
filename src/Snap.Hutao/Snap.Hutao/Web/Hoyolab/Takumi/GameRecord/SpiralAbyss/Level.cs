// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

/// <summary>
/// 间
/// </summary>
[HighQuality]
internal sealed class Level
{
    /// <summary>
    /// 索引
    /// </summary>
    [JsonPropertyName("index")]
    public uint Index { get; set; }

    /// <summary>
    /// 星数
    /// </summary>
    [JsonPropertyName("star")]
    public int Star { get; set; }

    /// <summary>
    /// 最大星数
    /// </summary>
    [JsonPropertyName("max_star")]
    public int MaxStar { get; set; }

    /// <summary>
    /// 上下半
    /// </summary>
    [JsonPropertyName("battles")]
    public List<Battle> Battles { get; set; } = default!;

    [JsonPropertyName("top_half_floor_monster")]
    public List<Monster> TopHalfFloorMonster { get; set; } = default!;

    [JsonPropertyName("bottom_half_floor_monster")]
    public List<Monster> BottomHalfFloorMonster { get; set; } = default!;
}