// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Calendar;

internal sealed class CalendarMetadataContext : IMetadataContext,
    ICultivationMetadataContext,
    IMetadataArrayAvatarSource,
    IMetadataArrayWeaponSource,
    IMetadataDictionaryIdMaterialSource
{
    public ImmutableArray<Avatar> Avatars { get; set; }

    public ImmutableArray<Weapon> Weapons { get; set; }

    public ImmutableDictionary<MaterialId, Material> IdMaterialMap { get; set; } = default!;

    public ImmutableArray<Material> Materials { get; set; }

    public ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;

    public ImmutableDictionary<WeaponId, Weapon> IdWeaponMap { get; set; } = default!;
}