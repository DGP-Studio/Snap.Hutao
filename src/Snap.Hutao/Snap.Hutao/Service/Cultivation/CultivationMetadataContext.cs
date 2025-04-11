// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Cultivation;

internal class CultivationMetadataContext : ICultivationMetadataContext
{
    public ImmutableArray<Material> Materials { get; set; } = default!;

    public ImmutableDictionary<MaterialId, Material> IdMaterialMap { get; set; } = default!;

    public ImmutableDictionary<AvatarId, Model.Metadata.Avatar.Avatar> IdAvatarMap { get; set; } = default!;

    public ImmutableDictionary<WeaponId, Model.Metadata.Weapon.Weapon> IdWeaponMap { get; set; } = default!;

    public ImmutableDictionary<MaterialId, Combine> ResultMaterialIdCombineMap { get; set; } = default!;

    public Item GetAvatarItem(AvatarId avatarId)
    {
        return this.GetAvatar(avatarId).GetOrCreateItem();
    }

    public Item GetWeaponItem(WeaponId weaponId)
    {
        return this.GetWeapon(weaponId).GetOrCreateItem();
    }
}