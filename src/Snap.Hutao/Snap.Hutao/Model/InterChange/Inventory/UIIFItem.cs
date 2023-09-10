// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.Inventory;

/// <summary>
/// UIIF物品
/// </summary>
[HighQuality]
internal sealed class UIIFItem
{
    /// <summary>
    /// 物品Id
    /// </summary>
    [JsonPropertyName("itemId")]
    public uint ItemId { get; set; }

    [JsonPropertyName("material")]
    public UIIFMaterial Material { get; set; } = default!;
}