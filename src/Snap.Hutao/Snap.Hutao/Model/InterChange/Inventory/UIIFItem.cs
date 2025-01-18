// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using PlayerStoreItem = Snap.Hutao.Service.Yae.PlayerStore.Item;

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

    public static UIIFItem FromPlayerStoreItem(PlayerStoreItem item)
    {
        return new()
        {
            ItemId = item.ItemId,
            Material = item.Material is null ? null : new() { Count = item.Material.Count },
            Furniture = item.Furniture is null ? null : new() { Count = item.Furniture.Count },
            Equip = item.Equip is null ? null : UIIFEquip.FromPlayerStoreEquip(item.Equip),
        };
    }
}