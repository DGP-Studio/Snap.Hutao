// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Hutao;

internal sealed class HutaoRoleCombatStatisticsMetadataContext : IMetadataContext,
    IMetadataDictionaryIdAvatarWithPlayersSource
{
    public ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;
}