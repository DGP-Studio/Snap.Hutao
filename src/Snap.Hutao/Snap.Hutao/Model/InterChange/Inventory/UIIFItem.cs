// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using InGameItem = Snap.Hutao.Service.Yae.PlayerStore.Item;

namespace Snap.Hutao.Model.InterChange.Inventory;

internal sealed class UIIFItem
{
    [JsonPropertyName("itemId")]
    public uint ItemId { get; init; }

    [JsonPropertyName("material")]
    public UIIFItemDetail? Material { get; init; }

    [JsonPropertyName("equip")]
    public UIIFEquip? Equip { get; init; }

    [JsonPropertyName("furniture")]
    public UIIFItemDetail? Furniture { get; init; }

    public static UIIFItem FromInGameItem(InGameItem item)
    {
        return new()
        {
            ItemId = item.ItemId,
            Material = item.Material is null ? null : new() { Count = item.Material.Count },
            Equip = item.Equip is null ? null : UIIFEquip.FromInGameEquip(item.Equip),
            Furniture = item.Furniture is null ? null : new() { Count = item.Furniture.Count },
        };
    }
}