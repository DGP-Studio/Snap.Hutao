// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.InterChange.Inventory;

internal sealed class UIIFWeapon
{
    [JsonPropertyName("level")]
    public Level Level { get; set; }

    [JsonPropertyName("promoteLevel")]
    public Level PromoteLevel { get; set; }

    [MaybeNull]
    [JsonPropertyName("affixMap")]
    public ImmutableDictionary<EquipAffixId, WeaponAffixLevel> AffixMap { get; set; } = default!;
}