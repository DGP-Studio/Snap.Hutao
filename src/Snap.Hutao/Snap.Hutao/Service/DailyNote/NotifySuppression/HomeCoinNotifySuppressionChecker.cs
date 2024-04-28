// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.DailyNote.NotifySuppression;

internal sealed class HomeCoinNotifySuppressionChecker : INotifySuppressionChecker
{
    public bool SuppressCondition(INotifySuppressionContext context)
    {
        return context.DailyNote.CurrentHomeCoin >= context.Entry.HomeCoinNotifyThreshold;
    }

    public bool GetSuppressed(INotifySuppressionContext context)
    {
        return context.Entry.HomeCoinNotifySuppressed;
    }

    public void SetSuppressed(INotifySuppressionContext context, bool suppressed)
    {
        context.Entry.HomeCoinNotifySuppressed = suppressed;
    }

    public DailyNoteNotifyInfo SuppressInfo(INotifySuppressionContext context)
    {
        return new(SH.ServiceDailyNoteNotifierHomeCoin, $"{context.DailyNote.CurrentHomeCoin}", SH.FormatServiceDailyNoteNotifierHomeCoinCurrent(context.DailyNote.CurrentHomeCoin));
    }
}