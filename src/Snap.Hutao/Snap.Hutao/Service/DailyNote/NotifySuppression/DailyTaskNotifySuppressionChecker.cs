// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.DailyNote.NotifySuppression;

internal sealed class DailyTaskNotifySuppressionChecker : INotifySuppressionChecker
{
    public bool ShouldNotify(INotifySuppressionContext context)
    {
        return context.Entry is { DailyTaskNotify: true, DailyNote.IsExtraTaskRewardReceived: false };
    }

    public bool GetIsSuppressed(INotifySuppressionContext context)
    {
        return context.Entry.DailyTaskNotifySuppressed;
    }

    public void SetIsSuppressed(INotifySuppressionContext context, bool suppressed)
    {
        context.Entry.DailyTaskNotifySuppressed = suppressed;
    }

    public DailyNoteNotifyInfo NotifyInfo(INotifySuppressionContext context)
    {
        return new(SH.ServiceDailyNoteNotifierDailyTask, SH.ServiceDailyNoteNotifierDailyTaskHint, context.DailyNote.ExtraTaskRewardDescription);
    }
}