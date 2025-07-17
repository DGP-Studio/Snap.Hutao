// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableArray;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataReliquary = Snap.Hutao.Model.Metadata.Reliquary.Reliquary;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

internal sealed class SummaryFactoryMetadataContext : IMetadataContext,
    IMetadataDictionaryIdAvatarSource,
    IMetadataDictionaryIdWeaponSource,
    IMetadataDictionaryIdReliquaryMainPropertySource,
    IMetadataDictionaryIdReliquarySetSource,
    IMetadataDictionaryIdReliquarySubAffixSource,
    IMetadataDictionaryIdReliquarySource,
    IMetadataArrayReliquaryMainAffixLevelSource,
    IMetadataDictionaryLevelWeaponGrowCurveSource,
    IMetadataDictionaryIdDictionaryLevelWeaponPromoteSource
{
    public ImmutableDictionary<AvatarId, MetadataAvatar> IdAvatarMap { get; set; } = default!;

    public ImmutableDictionary<WeaponId, MetadataWeapon> IdWeaponMap { get; set; } = default!;

    public ImmutableDictionary<ReliquaryMainAffixId, FightProperty> IdReliquaryMainPropertyMap { get; set; } = default!;

    public ImmutableDictionary<ReliquarySubAffixId, ReliquarySubAffix> IdReliquarySubAffixMap { get; set; } = default!;

    public ImmutableDictionary<ReliquarySetId, ReliquarySet> IdReliquarySetMap { get; set; } = default!;

    public ImmutableArray<ReliquaryMainAffixLevel> ReliquaryMainAffixLevels { get; set; } = default!;

    public ImmutableDictionary<ReliquaryId, MetadataReliquary> IdReliquaryMap { get; set; } = default!;

    public ImmutableDictionary<Level, TypeValueCollection<GrowCurveType, float>> LevelDictionaryWeaponGrowCurveMap { get; set; } = default!;

    public ImmutableDictionary<PromoteId, ImmutableDictionary<PromoteLevel, Promote>> IdDictionaryWeaponLevelPromoteMap { get; set; } = default!;
}