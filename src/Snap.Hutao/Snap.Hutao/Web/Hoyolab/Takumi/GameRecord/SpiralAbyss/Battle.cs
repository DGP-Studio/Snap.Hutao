// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

/// <summary>
/// 表示一次战斗
/// </summary>
[HighQuality]
internal sealed class Battle
{
    /// <summary>
    /// 索引
    /// </summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; } = default!;

    /// <summary>
    /// 参战角色
    /// </summary>
    [JsonPropertyName("avatars")]
    public List<Avatar> Avatars { get; set; } = default!;

    [JsonPropertyName("settle_date_time")]
    public SettleDateTime SettleDateTime { get; set; } = default!;
}
