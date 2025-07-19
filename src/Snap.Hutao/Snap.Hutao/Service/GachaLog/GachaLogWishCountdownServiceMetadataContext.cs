// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableArray;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.GachaLog;

internal sealed class GachaLogWishCountdownServiceMetadataContext : IMetadataContext,
    IMetadataSupportInitialization,
    IMetadataArrayGachaEventSource,
    IMetadataDictionaryIdAvatarSource,
    IMetadataDictionaryIdWeaponSource
{
    public ImmutableArray<GachaEvent> GachaEvents { get; set; } = default!;

    public ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;

    public ImmutableDictionary<WeaponId, Weapon> IdWeaponMap { get; set; } = default!;

    public bool IsInitialized { get; set; }
}