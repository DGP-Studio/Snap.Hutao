// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.DailyNote.NotifySuppression;

internal sealed class ResinNotifySuppressionChecker : INotifySuppressionChecker
{
    public bool SuppressCondition(INotifySuppressionContext context)
    {
        return context.DailyNote.CurrentResin >= context.Entry.ResinNotifyThreshold;
    }

    public bool GetSuppressed(INotifySuppressionContext context)
    {
        return context.Entry.ResinNotifySuppressed;
    }

    public void SetSuppressed(INotifySuppressionContext context, bool suppressed)
    {
        context.Entry.ResinNotifySuppressed = suppressed;
    }

    public DailyNoteNotifyInfo SuppressInfo(INotifySuppressionContext context)
    {
        return new(SH.ServiceDailyNoteNotifierResin, $"{context.DailyNote.CurrentResin}", SH.FormatServiceDailyNoteNotifierResinCurrent(context.DailyNote.CurrentResin));
    }
}