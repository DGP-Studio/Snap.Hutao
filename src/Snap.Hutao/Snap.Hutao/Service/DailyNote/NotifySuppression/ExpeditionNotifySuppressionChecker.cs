// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;

namespace Snap.Hutao.Service.DailyNote.NotifySuppression;

internal sealed class ExpeditionNotifySuppressionChecker : INotifySuppressionChecker
{
    public bool ShouldNotify(INotifySuppressionContext context)
    {
        return context.Entry.ExpeditionNotify && context.DailyNote.Expeditions.All(e => e.Status == ExpeditionStatus.Finished);
    }

    public bool GetIsSuppressed(INotifySuppressionContext context)
    {
        return context.Entry.ExpeditionNotifySuppressed;
    }

    public void SetIsSuppressed(INotifySuppressionContext context, bool suppressed)
    {
        context.Entry.ExpeditionNotifySuppressed = suppressed;
    }

    public DailyNoteNotifyInfo NotifyInfo(INotifySuppressionContext context)
    {
        return new(SH.ServiceDailyNoteNotifierExpedition, SH.ServiceDailyNoteNotifierExpeditionAdaptiveHint, SH.ServiceDailyNoteNotifierExpeditionHint);
    }
}