// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.Inventory;

internal class UIIFEquip
{
    [JsonPropertyName("reliquary")]
    public UIIFReliquary? Reliquary { get; set; }

    [JsonPropertyName("weapon")]
    public UIIFWeapon? Weapon { get; set; }
}