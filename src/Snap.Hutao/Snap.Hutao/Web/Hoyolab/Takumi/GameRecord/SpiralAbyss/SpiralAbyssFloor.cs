// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyssFloor
{
    [JsonPropertyName("index")]
    public required uint Index { get; init; }

    [JsonPropertyName("icon")]
    public required string Icon { get; init; }

    [JsonPropertyName("is_unlock")]
    public required bool IsUnlock { get; init; }

    [JsonPropertyName("settle_time")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public required long SettleTime { get; init; }

    [JsonPropertyName("star")]
    public required int Star { get; init; }

    [JsonPropertyName("max_star")]
    public required int MaxStar { get; init; }

    [JsonPropertyName("levels")]
    public required ImmutableArray<SpiralAbyssLevel> Levels { get; init; }

    [JsonPropertyName("settle_date_time")]
    public DateTime? SettleDateTime { get; init; }

    [JsonPropertyName("ley_line_disorder")]
    public ImmutableArray<string>? LeyLineDisorder { get; init; }
}