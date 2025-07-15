// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.HardChallenge;

internal sealed class HardChallengeMetadataContext : IMetadataContext,
    IMetadataDictionaryIdHardChallengeScheduleSource,
    IMetadataDictionaryIdAvatarWithPlayersSource
{
    public ImmutableDictionary<HardChallengeScheduleId, HardChallengeSchedule> IdHardChallengeScheduleMap { get; set; } = default!;

    public ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;
}