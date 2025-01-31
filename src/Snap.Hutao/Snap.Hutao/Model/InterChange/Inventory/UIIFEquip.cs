// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using InGameEquip = Snap.Hutao.Service.Yae.PlayerStore.Equip;

namespace Snap.Hutao.Model.InterChange.Inventory;

internal class UIIFEquip
{
    [JsonPropertyName("reliquary")]
    public UIIFReliquary? Reliquary { get; init; }

    [JsonPropertyName("weapon")]
    public UIIFWeapon? Weapon { get; init; }

    public static UIIFEquip? FromInGameEquip(InGameEquip? equip)
    {
        if (equip is null)
        {
            return default;
        }

        return new()
        {
            Reliquary = equip.Reliquary is null ? null : UIIFReliquary.FromInGameReliquary(equip.Reliquary),
            Weapon = equip.Weapon is null ? null : UIIFWeapon.FromInGameWeapon(equip.Weapon),
        };
    }
}