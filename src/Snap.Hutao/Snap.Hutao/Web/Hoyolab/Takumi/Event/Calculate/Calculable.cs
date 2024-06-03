// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 可计算的物品
/// </summary>
[HighQuality]
internal class Calculable : PromotionDelta
{
    /// <summary>
    /// 名称
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [JsonPropertyName("icon_url")]
    public Uri Icon { get; set; } = default!;

    /// <summary>
    /// 最大等级
    /// </summary>
    [JsonPropertyName("max_level")]
    public int MaxLevel { get; set; }

    /// <summary>
    /// 转化到提升差异
    /// </summary>
    /// <returns>提升差异</returns>
    public virtual PromotionDelta ToPromotionDelta()
    {
        return new()
        {
            LevelCurrent = LevelCurrent,
            LevelTarget = LevelTarget,
            Id = Id,
        };
    }
}