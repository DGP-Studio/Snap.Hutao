// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 类型化祈愿统计信息构建器
/// </summary>
internal class TypedWishSummaryBuilder
{
    /// <summary>
    /// 常驻祈愿
    /// </summary>
    public static readonly Func<GachaConfigType, bool> PermanentWish = type => type is GachaConfigType.PermanentWish;

    /// <summary>
    /// 角色活动
    /// </summary>
    public static readonly Func<GachaConfigType, bool> AvatarEventWish = type => type is GachaConfigType.AvatarEventWish or GachaConfigType.AvatarEventWish2;

    /// <summary>
    /// 武器活动
    /// </summary>
    public static readonly Func<GachaConfigType, bool> WeaponEventWish = type => type is GachaConfigType.WeaponEventWish;

    private readonly string name;
    private readonly int guarenteeOrangeThreshold;
    private readonly int guarenteePurpleThreshold;
    private readonly Func<GachaConfigType, bool> typeEvaluator;

    private readonly List<int> averageOrangePullTracker = new();
    private readonly List<int> averageUpOrangePullTracker = new();
    private readonly List<SummaryItem> summaryItemCache = new();

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
    /// <param name="name">祈愿配置</param>
    /// <param name="typeEvaluator">祈愿类型判断器</param>
    /// <param name="guarenteeOrangeThreshold">五星保底</param>
    /// <param name="guarenteePurpleThreshold">四星保底</param>
    public TypedWishSummaryBuilder(string name, Func<GachaConfigType, bool> typeEvaluator, int guarenteeOrangeThreshold, int guarenteePurpleThreshold)
    {
        this.name = name;
        this.typeEvaluator = typeEvaluator;
        this.guarenteeOrangeThreshold = guarenteeOrangeThreshold;
        this.guarenteePurpleThreshold = guarenteePurpleThreshold;
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
                case ItemQuality.QUALITY_ORANGE:
                    {
                        TrackMinMaxOrangePull(lastOrangePullTracker);
                        averageOrangePullTracker.Add(lastOrangePullTracker);

                        if (isUp)
                        {
                            averageUpOrangePullTracker.Add(lastUpOrangePullTracker);
                            lastUpOrangePullTracker = 0;
                        }

                        summaryItemCache.Add(source.ToSummaryItem(lastOrangePullTracker, item.Time, isUp));

                        lastOrangePullTracker = 0;
                        ++totalOrangePullTracker;
                        break;
                    }

                case ItemQuality.QUALITY_PURPLE:
                    {
                        lastPurplePullTracker = 0;
                        ++totalPurplePullTracker;
                        break;
                    }

                case ItemQuality.QUALITY_BLUE:
                    {
                        ++totalBluePullTracker;
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 转换到类型化祈愿统计信息
    /// </summary>
    /// <returns>类型化祈愿统计信息</returns>
    public TypedWishSummary ToTypedWishSummary()
    {
        summaryItemCache.CompleteAdding();
        double totalCountDouble = totalCountTracker;

        return new()
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
            GuarenteeOrangeThreshold = guarenteeOrangeThreshold,
            LastPurplePull = lastPurplePullTracker,
            GuarenteePurpleThreshold = guarenteePurpleThreshold,
            TotalOrangePull = totalOrangePullTracker,
            TotalPurplePull = totalPurplePullTracker,
            TotalBluePull = totalBluePullTracker,
            TotalOrangePercent = totalOrangePullTracker / totalCountDouble,
            TotalPurplePercent = totalPurplePullTracker / totalCountDouble,
            TotalBluePercent = totalBluePullTracker / totalCountDouble,
            AverageOrangePull = averageOrangePullTracker.AverageNoThrow(),
            AverageUpOrangePull = averageUpOrangePullTracker.AverageNoThrow(),
            OrangeList = summaryItemCache,
        };
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

    private void TrackFromToTime(DateTimeOffset time)
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