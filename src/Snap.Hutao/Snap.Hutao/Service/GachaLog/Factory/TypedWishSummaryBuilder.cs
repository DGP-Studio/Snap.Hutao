// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 类型化祈愿统计信息构建器
/// </summary>
[HighQuality]
internal sealed class TypedWishSummaryBuilder
{
    /// <summary>
    /// 常驻祈愿
    /// </summary>
    public static readonly Func<GachaConfigType, bool> IsStandardWish = type => type is GachaConfigType.StandardWish;

    /// <summary>
    /// 角色活动
    /// </summary>
    public static readonly Func<GachaConfigType, bool> IsAvatarEventWish = type => type is GachaConfigType.AvatarEventWish or GachaConfigType.AvatarEventWish2;

    /// <summary>
    /// 武器活动
    /// </summary>
    public static readonly Func<GachaConfigType, bool> IsWeaponEventWish = type => type is GachaConfigType.WeaponEventWish;

    private readonly IServiceProvider serviceProvider;
    private readonly string name;
    private readonly int guaranteeOrangeThreshold;
    private readonly int guaranteePurpleThreshold;
    private readonly Func<GachaConfigType, bool> typeEvaluator;
    private readonly Web.Hutao.GachaLog.GachaDistributionType distributionType;

    private readonly List<int> averageOrangePullTracker = new();
    private readonly List<int> averageUpOrangePullTracker = new();
    private readonly List<SummaryItem> summaryItems = new();

    private int maxOrangePullTracker;
    private int minOrangePullTracker;
    private int lastOrangePullTracker;
    private int lastUpOrangePullTracker;
    private int lastPurplePullTracker;
    private int totalCountTracker;
    private int totalOrangePullTracker;
    private int totalPurplePullTracker;
    private int totalBluePullTracker;

    private DateTimeOffset fromTimeTracker = DateTimeOffset.MaxValue;
    private DateTimeOffset toTimeTracker = DateTimeOffset.MinValue;

    /// <summary>
    /// 构造一个新的类型化祈愿统计信息构建器
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="name">祈愿配置</param>
    /// <param name="typeEvaluator">祈愿类型判断器</param>
    /// <param name="distributionType">分布类型</param>
    /// <param name="guaranteeOrangeThreshold">五星保底</param>
    /// <param name="guaranteePurpleThreshold">四星保底</param>
    public TypedWishSummaryBuilder(
        IServiceProvider serviceProvider,
        string name,
        Func<GachaConfigType, bool> typeEvaluator,
        Web.Hutao.GachaLog.GachaDistributionType distributionType,
        int guaranteeOrangeThreshold,
        int guaranteePurpleThreshold)
    {
        this.serviceProvider = serviceProvider;
        this.name = name;
        this.typeEvaluator = typeEvaluator;
        this.guaranteeOrangeThreshold = guaranteeOrangeThreshold;
        this.guaranteePurpleThreshold = guaranteePurpleThreshold;
        this.distributionType = distributionType;
    }

    /// <summary>
    /// 追踪物品
    /// </summary>
    /// <param name="item">祈愿物品</param>
    /// <param name="source">对应武器</param>
    /// <param name="isUp">是否为Up物品</param>
    public void Track(GachaItem item, ISummaryItemSource source, bool isUp)
    {
        if (typeEvaluator(item.GachaType))
        {
            ++lastOrangePullTracker;
            ++lastPurplePullTracker;
            ++lastUpOrangePullTracker;

            // track total pulls
            ++totalCountTracker;
            TrackFromToTime(item.Time);

            switch (source.Quality)
            {
                case QualityType.QUALITY_ORANGE:
                    {
                        TrackMinMaxOrangePull(lastOrangePullTracker);
                        averageOrangePullTracker.Add(lastOrangePullTracker);

                        if (isUp)
                        {
                            averageUpOrangePullTracker.Add(lastUpOrangePullTracker);
                            lastUpOrangePullTracker = 0;
                        }

                        summaryItems.Add(source.ToSummaryItem(lastOrangePullTracker, item.Time, isUp));

                        lastOrangePullTracker = 0;
                        ++totalOrangePullTracker;
                        break;
                    }

                case QualityType.QUALITY_PURPLE:
                    {
                        lastPurplePullTracker = 0;
                        ++totalPurplePullTracker;
                        break;
                    }

                case QualityType.QUALITY_BLUE:
                    {
                        ++totalBluePullTracker;
                        break;
                    }

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 转换到类型化祈愿统计信息
    /// </summary>
    /// <returns>类型化祈愿统计信息</returns>
    public TypedWishSummary ToTypedWishSummary()
    {
        summaryItems.CompleteAdding();
        double totalCount = totalCountTracker;

        TypedWishSummary summary = new()
        {
            // base
            Name = name,
            From = fromTimeTracker,
            To = toTimeTracker,
            TotalCount = totalCountTracker,

            // TypedWishSummary
            MaxOrangePull = maxOrangePullTracker,
            MinOrangePull = minOrangePullTracker,
            LastOrangePull = lastOrangePullTracker,
            GuaranteeOrangeThreshold = guaranteeOrangeThreshold,
            LastPurplePull = lastPurplePullTracker,
            GuaranteePurpleThreshold = guaranteePurpleThreshold,
            TotalOrangePull = totalOrangePullTracker,
            TotalPurplePull = totalPurplePullTracker,
            TotalBluePull = totalBluePullTracker,
            TotalOrangePercent = totalOrangePullTracker / totalCount,
            TotalPurplePercent = totalPurplePullTracker / totalCount,
            TotalBluePercent = totalBluePullTracker / totalCount,
            AverageOrangePull = averageOrangePullTracker.SpanAverage(),
            AverageUpOrangePull = averageUpOrangePullTracker.SpanAverage(),
            OrangeList = summaryItems,
        };

        new PullPrediction(serviceProvider, summary, distributionType).PredictAsync().SafeForget();

        return summary;
    }

    private void TrackMinMaxOrangePull(int lastOrangePull)
    {
        if (lastOrangePull < minOrangePullTracker || minOrangePullTracker == 0)
        {
            minOrangePullTracker = lastOrangePull;
        }

        if (lastOrangePull > maxOrangePullTracker || maxOrangePullTracker == 0)
        {
            maxOrangePullTracker = lastOrangePull;
        }
    }

    private void TrackFromToTime(in DateTimeOffset time)
    {
        if (time < fromTimeTracker)
        {
            fromTimeTracker = time;
        }

        if (time > toTimeTracker)
        {
            toTimeTracker = time;
        }
    }
}