// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.DailyNote.NotifySuppression;

internal sealed class HomeCoinNotifySuppressionChecker : INotifySuppressionChecker
{
    public bool ShouldNotify(INotifySuppressionContext context)
    {
        bool result = context.DailyNote.CurrentHomeCoin >= context.Entry.HomeCoinNotifyThreshold;
        context.Entry.HomeCoinDotVisible = result;
        return result;
    }

    public bool GetIsSuppressed(INotifySuppressionContext context)
    {
        return context.Entry.HomeCoinNotifySuppressed;
    }

    public void SetIsSuppressed(INotifySuppressionContext context, bool suppressed)
    {
        context.Entry.HomeCoinNotifySuppressed = suppressed;
    }

    public DailyNoteNotifyInfo NotifyInfo(INotifySuppressionContext context)
    {
        return new(SH.ServiceDailyNoteNotifierHomeCoin, $"{context.DailyNote.CurrentHomeCoin}", SH.FormatServiceDailyNoteNotifierHomeCoinCurrent(context.DailyNote.CurrentHomeCoin));
    }
}