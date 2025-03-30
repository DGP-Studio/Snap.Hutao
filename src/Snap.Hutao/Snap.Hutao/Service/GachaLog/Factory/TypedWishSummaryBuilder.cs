// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal sealed class TypedWishSummaryBuilder
{
    private readonly TypedWishSummaryBuilderContext context;

    private readonly List<int> averageOrangePullTracker = [];
    private readonly List<int> averageUpOrangePullTracker = [];
    private readonly List<SummaryItem> summaryItems = [];

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

    public TypedWishSummaryBuilder(TypedWishSummaryBuilderContext context)
    {
        this.context = context;
    }

    public void Track(GachaItem item, ISummaryItemConvertible source, bool isUp)
    {
        if (!context.TypeEvaluator(item.GachaType))
        {
            return;
        }

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
        }
    }

    public TypedWishSummary ToTypedWishSummary(AsyncBarrier barrier)
    {
        summaryItems.CompleteAdding(context.GuaranteeOrangeThreshold);
        double totalCount = totalCountTracker;

        TypedWishSummary summary = new()
        {
            // base
            Name = context.Name,
            TypeName = $"{context.DistributionType:D}",
            From = fromTimeTracker,
            To = toTimeTracker,
            TotalCount = totalCountTracker,

            // TypedWishSummary
            MaxOrangePull = maxOrangePullTracker,
            MinOrangePull = minOrangePullTracker,
            LastOrangePull = lastOrangePullTracker,
            GuaranteeOrangeThreshold = context.GuaranteeOrangeThreshold,
            LastPurplePull = lastPurplePullTracker,
            GuaranteePurpleThreshold = context.GuaranteePurpleThreshold,
            TotalOrangePull = totalOrangePullTracker,
            TotalPurplePull = totalPurplePullTracker,
            TotalBluePull = totalBluePullTracker,
            TotalOrangePercent = totalOrangePullTracker / totalCount,
            TotalPurplePercent = totalPurplePullTracker / totalCount,
            TotalBluePercent = totalBluePullTracker / totalCount,
            AverageOrangePull = averageOrangePullTracker.Average(),
            AverageUpOrangePull = averageUpOrangePullTracker.Average(),
            OrangeList = summaryItems,
        };

        new PullPrediction(summary, context).PredictAsync(barrier).SafeForget();

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