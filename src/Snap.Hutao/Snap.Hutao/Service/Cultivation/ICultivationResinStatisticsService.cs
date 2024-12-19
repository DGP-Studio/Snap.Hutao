// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.Cultivation;

namespace Snap.Hutao.Service.Cultivation;

internal interface ICultivationResinStatisticsService
{
    ValueTask<ResinStatistics> GetResinStatisticsAsync(IEnumerable<StatisticsCultivateItem> statisticsCultivateItems, CancellationToken token);
}