// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Achievement;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;

namespace Snap.Hutao.Service.Achievement;

internal interface IAchievementStatisticsService
{
    Task<List<AchievementStatistics>> GetAchievementStatisticsAsync(Dictionary<AchievementId, MetadataAchievement> achievementMap);
}