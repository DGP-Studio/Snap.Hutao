// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.Achievement;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Achievement;

internal interface IAchievementStatisticsService
{
    ValueTask<ImmutableArray<AchievementStatistics>> GetAchievementStatisticsAsync(AchievementServiceMetadataContext context, CancellationToken token = default);
}