// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyssBattle
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; } = default!;

    [JsonPropertyName("avatars")]
    public List<SpiralAbyssAvatar> Avatars { get; set; } = default!;

    [JsonPropertyName("settle_date_time")]
    public DateTime SettleDateTime { get; set; } = default!;
}
