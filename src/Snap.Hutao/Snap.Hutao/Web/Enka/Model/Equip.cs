// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.InterChange.Inventory;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 装备
/// </summary>
[HighQuality]
internal sealed class Equip : UIIFEquip
{
    /// <summary>
    /// 物品Id
    /// Equipment ID
    /// </summary>
    [JsonPropertyName("itemId")]
    public uint ItemId { get; set; }

    /// <summary>
    /// Detailed Info of Equipment
    /// </summary>
    [JsonPropertyName("flat")]
    public Flat Flat { get; set; } = default!;
}