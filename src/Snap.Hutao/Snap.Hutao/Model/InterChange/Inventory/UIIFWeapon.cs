// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.InterChange.Inventory;

internal sealed class UIIFWeapon
{
    [JsonPropertyName("level")]
    public Level Level { get; set; }

    [JsonPropertyName("promoteLevel")]
    public Level PromoteLevel { get; set; }

    [MaybeNull]
    [JsonPropertyName("affixMap")]
    public Dictionary<EquipAffixId, WeaponAffixLevel> AffixMap { get; set; } = default!;
}