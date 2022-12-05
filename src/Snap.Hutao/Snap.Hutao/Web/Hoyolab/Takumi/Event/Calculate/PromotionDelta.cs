// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 计算信息
/// </summary>
public class PromotionDelta
{
    /// <summary>
    /// 物品Id
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 当前等级
    /// </summary>
    [JsonPropertyName("level_current")]
    public int LevelCurrent { get; set; }

    /// <summary>
    /// 目标等级
    /// </summary>
    [JsonPropertyName("level_target")]
    public int LevelTarget { get; set; }
}