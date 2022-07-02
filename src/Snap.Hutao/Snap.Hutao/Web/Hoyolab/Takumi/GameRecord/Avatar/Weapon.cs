// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

/// <summary>
/// 武器信息
/// </summary>
public class Weapon
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 类型
    /// </summary>
    [JsonPropertyName("type")]
    public WeaponType Type { get; set; }

    /// <summary>
    /// 稀有度
    /// </summary>
    [JsonPropertyName("rarity")]
    public ItemQuality Rarity { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 突破等级
    /// </summary>
    [JsonPropertyName("promote_level")]
    public int PromoteLevel { get; set; }

    /// <summary>
    /// 类型名称
    /// </summary>
    [JsonPropertyName("type_name")]
    public string TypeName { get; set; } = default!;

    /// <summary>
    /// 武器介绍
    /// </summary>
    [JsonPropertyName("desc")]
    public string Description { get; set; } = default!;

    /// <summary>
    /// 精炼等级
    /// </summary>
    [JsonPropertyName("affix_level")]
    public int AffixLevel { get; set; }
}
