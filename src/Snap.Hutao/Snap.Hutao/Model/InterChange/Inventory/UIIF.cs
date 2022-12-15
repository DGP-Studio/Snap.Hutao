// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.InterChange.Inventory;

/// <summary>
/// 统一可交换物品格式
/// </summary>
internal class UIIF
{
    /// <summary>
    /// 当前发行的版本
    /// </summary>
    public const string CurrentVersion = "v1.0";

    private static readonly ImmutableList<string> SupportedVersion = new List<string>()
    {
        CurrentVersion,
    }.ToImmutableList();

    /// <summary>
    /// 信息
    /// </summary>
    [JsonPropertyName("info")]
    public UIIFInfo Info { get; set; } = default!;

    /// <summary>
    /// 列表
    /// </summary>
    [JsonPropertyName("list")]
    public List<UIIFItem> List { get; set; } = default!;
}

/// <summary>
/// UIIF物品
/// </summary>
[JsonDerivedType(typeof(UIIFReliquary))]
[JsonDerivedType(typeof(UIIFWeapon))]
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
}

/// <summary>
/// UIIF圣遗物
/// </summary>
internal class UIIFReliquary : UIIFItem
{
    /// <summary>
    /// 物品Id
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 副属性列表
    /// </summary>
    [JsonPropertyName("appendPropIdList")]
    public List<int> AppendPropIdList { get; set; }
}

/// <summary>
/// UIIF武器
/// </summary>
internal class UIIFWeapon : UIIFItem
{
    /// <summary>
    /// 物品Id
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 精炼等级 0-4
    /// </summary>
    [JsonPropertyName("promoteLevel")]
    public int PromoteLevel { get; set; }
}