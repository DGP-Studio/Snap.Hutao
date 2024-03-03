// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal sealed class HutaoStatisticsFactoryMetadataContext : IMetadataContext,
    IMetadataDictionaryIdAvatarSource,
    IMetadataDictionaryIdWeaponSource,
    IMetadataListGachaEventSource
{
    public Dictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;

    public Dictionary<WeaponId, Weapon> IdWeaponMap { get; set; } = default!;

    public List<GachaEvent> GachaEvents { get; set; } = default!;
}