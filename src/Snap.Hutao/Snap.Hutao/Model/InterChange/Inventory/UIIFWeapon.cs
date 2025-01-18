// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;
using PlayerStoreWeapon = Snap.Hutao.Service.Yae.PlayerStore.Weapon;

namespace Snap.Hutao.Model.InterChange.Inventory;

// ReSharper disable once InconsistentNaming
internal sealed class UIIFWeapon
{
    [JsonPropertyName("level")]
    public Level Level { get; set; }

    [JsonPropertyName("promoteLevel")]
    public Level PromoteLevel { get; set; }

    [JsonPropertyName("affixMap")]
    public ImmutableDictionary<EquipAffixId, WeaponAffixLevel>? AffixMap { get; set; }

    public static UIIFWeapon FromPlayerStoreWeapon(PlayerStoreWeapon weapon)
    {
        return new()
        {
            Level = weapon.Level,
            PromoteLevel = weapon.PromoteLevel,
            AffixMap = weapon.AffixMap is null
                ? null
                : weapon.AffixMap.ToImmutableDictionary(kv => (EquipAffixId)kv.Key, kv => (WeaponAffixLevel)kv.Value),
        };
    }
}