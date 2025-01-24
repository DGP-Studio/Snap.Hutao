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
            Material = UIIFItemDetail.Create(item.Material?.Count),
            Equip = UIIFEquip.FromInGameEquip(item.Equip),
            Furniture = UIIFItemDetail.Create(item.Furniture?.Count),
        };
    }

    public static UIIFItem From(uint itemId, uint count)
    {
        return new()
        {
            ItemId = itemId,
            Material = UIIFItemDetail.Create(count),
        };
    }
}