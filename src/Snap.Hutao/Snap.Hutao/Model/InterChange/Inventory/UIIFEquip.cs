// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using PlayerStoreEquip = Snap.Hutao.Service.Yae.PlayerStore.Equip;

namespace Snap.Hutao.Model.InterChange.Inventory;

internal class UIIFEquip
{
    [JsonPropertyName("reliquary")]
    public UIIFReliquary? Reliquary { get; set; }

    [JsonPropertyName("weapon")]
    public UIIFWeapon? Weapon { get; set; }

    public static UIIFEquip FromPlayerStoreEquip(PlayerStoreEquip equip)
    {
        return new()
        {
            Reliquary = equip.Reliquary is null ? null : UIIFReliquary.FromPlayerStoreReliquary(equip.Reliquary),
            Weapon = equip.Weapon is null ? null : UIIFWeapon.FromPlayerStoreWeapon(equip.Weapon),
        };
    }
}