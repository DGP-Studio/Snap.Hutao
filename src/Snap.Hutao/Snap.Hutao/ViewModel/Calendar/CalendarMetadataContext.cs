// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableArray;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Calendar;

internal sealed class CalendarMetadataContext : CultivationMetadataContext, IMetadataContext,
    ICultivationMetadataContext,
    IMetadataArrayAvatarSource,
    IMetadataArrayWeaponSource,
    IMetadataDictionaryIdMaterialSource
{
    public ImmutableArray<Avatar> Avatars { get; set; }

    public ImmutableArray<Weapon> Weapons { get; set; }
}