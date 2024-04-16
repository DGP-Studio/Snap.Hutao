// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;

namespace Snap.Hutao.Service.Achievement;

internal sealed class AchievementServiceMetadataContext : IMetadataContext,
    IMetadataListAchievementSource,
    IMetadataDictionaryIdAchievementSource
{
    public List<MetadataAchievement> Achievements { get; set; } = default!;

    public Dictionary<AchievementId, MetadataAchievement> IdAchievementMap { get; set; } = default!;
}