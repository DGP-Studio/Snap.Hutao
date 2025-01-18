// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.Inventory;

internal sealed class UIIFItem
{
    [JsonPropertyName("itemId")]
    public uint ItemId { get; set; }

    [JsonPropertyName("material")]
    public UIIFCountInfo? Material { get; set; }

    [JsonPropertyName("furniture")]
    public UIIFCountInfo? Furniture { get; set; }

    [JsonPropertyName("equip")]
    public UIIFEquip? Equip { get; set; }
}