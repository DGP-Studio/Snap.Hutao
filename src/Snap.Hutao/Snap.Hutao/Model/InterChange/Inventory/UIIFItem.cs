// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.InterChange.Inventory;

/// <summary>
/// UIIF物品
/// </summary>
internal class UIIFItem
{
    /// <summary>
    /// 物品Id
    /// </summary>
    [JsonPropertyName("itemId")]
    public int ItemId { get; set; }

    /// <summary>
    /// 物品Id
    /// </summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>
    /// 等级
    /// Reliquary/Weapon
    /// </summary>
    [JsonPropertyName("level")]
    public int? Level { get; set; }

    /// <summary>
    /// 副属性列表
    /// Reliquary
    /// </summary>
    [JsonPropertyName("appendPropIdList")]
    public List<int>? AppendPropIdList { get; set; } = default!;

    /// <summary>
    /// 精炼等级 0-4
    /// Weapon
    /// </summary>
    [JsonPropertyName("promoteLevel")]
    public int? PromoteLevel { get; set; }
}