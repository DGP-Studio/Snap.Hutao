// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Cultivation;

internal sealed class CultivationMetadataContext : ICultivationMetadataContext
{
    public List<Material> Materials { get; set; } = default!;

    public Dictionary<MaterialId, Material> IdMaterialMap { get; set; } = default!;

    public Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> IdAvatarMap { get; set; } = default!;

    public Dictionary<WeaponId, Model.Metadata.Weapon.Weapon> IdWeaponMap { get; set; } = default!;
}