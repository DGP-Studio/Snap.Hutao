// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Cultivation;

internal class CultivationMetadataContext : ICultivationMetadataContext
{
    private readonly Dictionary<AvatarId, Item> avatarItemCache = [];
    private readonly Dictionary<WeaponId, Item> weaponItemCache = [];

    public ImmutableArray<Material> Materials { get; set; } = default!;

    public ImmutableDictionary<MaterialId, Material> IdMaterialMap { get; set; } = default!;

    public ImmutableDictionary<AvatarId, Model.Metadata.Avatar.Avatar> IdAvatarMap { get; set; } = default!;

    public ImmutableDictionary<WeaponId, Model.Metadata.Weapon.Weapon> IdWeaponMap { get; set; } = default!;

    public Item GetAvatarItem(AvatarId avatarId)
    {
        if (!avatarItemCache.TryGetValue(avatarId, out Item? item))
        {
            item = IdAvatarMap[avatarId].ToItem<Item>();
            avatarItemCache[avatarId] = item;
        }

        return item;
    }

    public Item GetWeaponItem(WeaponId weaponId)
    {
        if (!weaponItemCache.TryGetValue(weaponId, out Item? item))
        {
            item = IdWeaponMap[weaponId].ToItem<Item>();
            weaponItemCache[weaponId] = item;
        }

        return item;
    }
}