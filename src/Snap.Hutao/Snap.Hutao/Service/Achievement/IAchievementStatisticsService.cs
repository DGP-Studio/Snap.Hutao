// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.Achievement;

namespace Snap.Hutao.Service.Achievement;

internal interface IAchievementStatisticsService
{
    ValueTask<List<AchievementStatistics>> GetAchievementStatisticsAsync(AchievementServiceMetadataContext context, CancellationToken token = default);
}