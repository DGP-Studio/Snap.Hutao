// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyssBattle
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("timestamp")]
    public required long Timestamp { get; init; }

    [JsonPropertyName("avatars")]
    public required ImmutableArray<SpiralAbyssAvatar> Avatars { get; init; }

    [JsonPropertyName("settle_date_time")]
    public DateTime? SettleDateTime { get; init; }
}