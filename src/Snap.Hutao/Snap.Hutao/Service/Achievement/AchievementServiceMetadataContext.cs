// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableArray;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;

namespace Snap.Hutao.Service.Achievement;

internal sealed class AchievementServiceMetadataContext : IMetadataContext,
    IMetadataArrayAchievementSource,
    IMetadataDictionaryIdAchievementSource
{
    public ImmutableArray<MetadataAchievement> Achievements { get; set; } = default!;

    public ImmutableDictionary<AchievementId, MetadataAchievement> IdAchievementMap { get; set; } = default!;
}