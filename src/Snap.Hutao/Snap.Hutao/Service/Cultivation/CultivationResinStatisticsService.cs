// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.ViewModel.Cultivation;
using System.Diagnostics;

namespace Snap.Hutao.Service.Cultivation;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(ICultivationResinStatisticsService))]
internal sealed partial class CultivationResinStatisticsService : ICultivationResinStatisticsService
{
    private readonly ITaskContext taskContext;

    public async ValueTask<ResinStatistics> GetResinStatisticsAsync(IEnumerable<StatisticsCultivateItem> statisticsCultivateItems, CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        ResinStatistics statistics = new();

        IEnumerable<IGrouping<uint, StatisticsCultivateItem>> groupedItems = statisticsCultivateItems
            .Where(i => i.Inner.IsResinItem())
            .GroupBy(i => i.Inner.Rank);

        foreach ((uint rank, IEnumerable<StatisticsCultivateItem> items) in groupedItems)
        {
            // 摩拉
            if (rank is 10U)
            {
                StatisticsCultivateItem item = items.Single();
                if (item.IsFinished)
                {
                    continue;
                }

                double times = GetStatisticsCultivateItemTimes(item);
                statistics.BlossomOfWealth.RawItemCount += times;
                continue;
            }

            // 经验
            if (rank is 100U)
            {
                StatisticsCultivateItem item = items.Single();
                Debug.Assert(item.Inner.RankLevel is QualityType.QUALITY_PURPLE, "经验书必须是紫色品质");
                if (item.IsFinished)
                {
                    continue;
                }

                double times = GetStatisticsCultivateItemTimes(item) * 20000D;
                statistics.BlossomOfRevelation.RawItemCount += times;
                continue;
            }

            // BOSS 掉落
            if (rank is 11101U)
            {
                foreach (StatisticsCultivateItem item in items)
                {
                    if (item.IsFinished)
                    {
                        continue;
                    }

                    double times = GetStatisticsCultivateItemTimes(item);
                    _ = item.Inner.RankLevel switch
                    {
                        QualityType.QUALITY_PURPLE => statistics.NormalBoss.RawItemCount += times,
                        QualityType.QUALITY_ORANGE => statistics.WeeklyBoss.RawItemCount += times,
                        _ => throw HutaoException.NotSupported(),
                    };
                }

                continue;
            }

            // 天赋书，武器突破材料
            // items.Count in [0..4]
            double greenItems = 0D;
            double blueItems = 0D;
            double purpleItems = 0D;
            double orangeItems = 0D;

            // ABCDE -> B
            ResinStatisticsItem targetStatisticsItem = ((rank / 1000) % 10) switch
            {
                3 => statistics.TalentAscension,
                5 => statistics.WeaponAscension,
                _ => throw HutaoException.NotSupported(),
            };

            foreach (StatisticsCultivateItem item in items)
            {
                double times = GetStatisticsCultivateItemTimes(item);
                _ = item.Inner.RankLevel switch
                {
                    QualityType.QUALITY_GREEN => greenItems += times,
                    QualityType.QUALITY_BLUE => blueItems += times,
                    QualityType.QUALITY_PURPLE => purpleItems += times,
                    QualityType.QUALITY_ORANGE => orangeItems += times,
                    _ => throw HutaoException.NotSupported(),
                };
            }

            targetStatisticsItem.RawItemCount += AlchemyCrafting.UnWeighted(orangeItems, purpleItems, blueItems, greenItems);
        }

        statistics.RefreshBlossomOfWealth();
        return statistics;
    }

    private static double GetStatisticsCultivateItemTimes(StatisticsCultivateItem item)
    {
        return item.Count - (long)item.Current;
    }
}