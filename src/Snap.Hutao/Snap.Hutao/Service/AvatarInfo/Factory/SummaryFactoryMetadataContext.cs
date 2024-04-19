// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataReliquary = Snap.Hutao.Model.Metadata.Reliquary.Reliquary;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

[HighQuality]
internal sealed class SummaryFactoryMetadataContext : IMetadataContext,
    IMetadataDictionaryIdAvatarSource,
    IMetadataDictionaryIdWeaponSource,
    IMetadataDictionaryIdReliquaryAffixWeightSource,
    IMetadataDictionaryIdReliquaryMainPropertySource,
    IMetadataDictionaryIdReliquarySubAffixSource
{
    public Dictionary<AvatarId, MetadataAvatar> IdAvatarMap { get; set; } = default!;

    public Dictionary<WeaponId, MetadataWeapon> IdWeaponMap { get; set; } = default!;

    public Dictionary<AvatarId, ReliquaryAffixWeight> IdReliquaryAffixWeightMap { get; set; } = default!;

    public Dictionary<ReliquaryMainAffixId, FightProperty> IdReliquaryMainPropertyMap { get; set; } = default!;

    public Dictionary<ReliquarySubAffixId, ReliquarySubAffix> IdReliquarySubAffixMap { get; set; } = default!;

    public List<ReliquaryMainAffixLevel> ReliquaryLevels { get; set; } = default!;

    public List<MetadataReliquary> Reliquaries { get; set; } = default!;
}