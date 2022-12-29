// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Primitive;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataReliquary = Snap.Hutao.Model.Metadata.Reliquary.Reliquary;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

[SuppressMessage("", "SA1600")]
internal class SummaryMetadataContext
{
    public Dictionary<AvatarId, MetadataAvatar> IdAvatarMap { get; set; } = default!;

    public Dictionary<WeaponId, MetadataWeapon> IdWeaponMap { get; set; } = default!;

    public Dictionary<ReliquaryMainAffixId, FightProperty> IdRelicMainPropMap { get; set; } = default!;

    public Dictionary<ReliquaryAffixId, ReliquaryAffix> IdReliquaryAffixMap { get; set; } = default!;

    public List<ReliquaryLevel> ReliqueryLevels { get; set; } = default!;

    public List<MetadataReliquary> Reliquaries { get; set; } = default!;
}