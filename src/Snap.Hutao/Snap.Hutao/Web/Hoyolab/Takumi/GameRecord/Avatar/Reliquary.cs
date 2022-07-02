// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

/// <summary>
/// 圣遗物
/// </summary>
public class Reliquary
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
    /// 部位
    /// </summary>
    [JsonPropertyName("pos")]
    public ReliquaryPosition Position { get; set; }

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
    /// 圣遗物套装
    /// </summary>
    [JsonPropertyName("set")]
    public ReliquarySet ReliquarySet { get; set; } = default!;

    /// <summary>
    /// 部位名称
    /// </summary>
    [JsonPropertyName("pos_name")]
    public string PositionName { get; set; } = default!;
}
