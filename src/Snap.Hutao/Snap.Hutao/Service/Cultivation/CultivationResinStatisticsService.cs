// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.ViewModel.Cultivation;

namespace Snap.Hutao.Service.Cultivation;

[Service(ServiceLifetime.Singleton, typeof(ICultivationResinStatisticsService))]
internal sealed partial class CultivationResinStatisticsService : ICultivationResinStatisticsService
{
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial CultivationResinStatisticsService(IServiceProvider serviceProvider);

    public async ValueTask<ResinStatistics> GetResinStatisticsAsync(IEnumerable<StatisticsCultivateItem> statisticsCultivateItems, CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        ResinStatistics statistics = new();

        ILookup<uint, StatisticsCultivateItem> itemsLookup = statisticsCultivateItems
            .Where(i => i.Inner.IsResinItem())
            .ToLookup(i => i.Inner.Rank);

        foreach ((uint rank, IEnumerable<StatisticsCultivateItem> items) in itemsLookup)
        {
            switch (rank)
            {
                case 10U: // 摩拉
                    {
                        StatisticsCultivateItem item = items.Single();
                        if (!item.IsFinished)
                        {
                            double times = GetStatisticsCultivateItemTimes(item);
                            statistics.BlossomOfWealth.RawItemCount += times;
                        }

                        break;
                    }

                case 100U: // 经验
                    {
                        // TODO: Reduce enumeration
                        if (items.SingleOrDefault(i => i.Inner.RankLevel is QualityType.QUALITY_PURPLE) is { IsFinished: false })
                        {
                            foreach (StatisticsCultivateItem item in items)
                            {
                                double experience = item.Inner.RankLevel switch
                                {
                                    QualityType.QUALITY_GREEN => 1000D,
                                    QualityType.QUALITY_BLUE => 5000D,
                                    QualityType.QUALITY_PURPLE => 20000D,
                                    _ => throw HutaoException.NotSupported(),
                                };
                                double times = GetStatisticsCultivateItemTimes(item) * experience;
                                statistics.BlossomOfRevelation.RawItemCount += times;
                            }
                        }

                        break;
                    }

                case 11101U: // BOSS 掉落
                    {
                        foreach (StatisticsCultivateItem item in items)
                        {
                            if (!item.IsFinished)
                            {
                                double times = GetStatisticsCultivateItemTimes(item);
                                _ = item.Inner.RankLevel switch
                                {
                                    QualityType.QUALITY_PURPLE => statistics.NormalBoss.RawItemCount += times,
                                    QualityType.QUALITY_ORANGE => statistics.WeeklyBoss.RawItemCount += times,
                                    _ => throw HutaoException.NotSupported(),
                                };
                            }
                        }

                        break;
                    }

                default:
                    {
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
                            // IsFinished is implicitly calculated in AlchemyCrafting
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
                        break;
                    }
            }
        }

        statistics.RefreshBlossomOfWealth();
        return statistics;
    }

    private static double GetStatisticsCultivateItemTimes(StatisticsCultivateItem item)
    {
        return item.Count - (long)item.Current;
    }
}