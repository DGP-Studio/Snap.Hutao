// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyssFloor
{
    [JsonPropertyName("index")]
    public uint Index { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    [JsonPropertyName("is_unlock")]
    public bool IsUnlock { get; set; } = default!;

    [JsonPropertyName("settle_time")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long SettleTime { get; set; } = default!;

    [JsonPropertyName("star")]
    public int Star { get; set; }

    [JsonPropertyName("max_star")]
    public int MaxStar { get; set; }

    [JsonPropertyName("levels")]
    public List<SpiralAbyssLevel> Levels { get; set; } = default!;

    [JsonPropertyName("settle_date_time")]
    public DateTime? SettleDateTime { get; set; }

    [JsonPropertyName("ley_line_disorder")]
    public List<string> LeyLineDisorder { get; set; } = default!;
}