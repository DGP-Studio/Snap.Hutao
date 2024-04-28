// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.DailyNote.NotifySuppression;

internal sealed class TransformerNotifySuppressionChecker : INotifySuppressionChecker
{
    public bool SuppressCondition(INotifySuppressionContext context)
    {
        return context.Entry is { TransformerNotify: true, DailyNote.Transformer: { Obtained: true, RecoveryTime.Reached: true } };
    }

    public bool GetSuppressed(INotifySuppressionContext context)
    {
        return context.Entry.TransformerNotifySuppressed;
    }

    public void SetSuppressed(INotifySuppressionContext context, bool suppressed)
    {
        context.Entry.TransformerNotifySuppressed = suppressed;
    }

    public DailyNoteNotifyInfo SuppressInfo(INotifySuppressionContext context)
    {
        return new(SH.ServiceDailyNoteNotifierTransformer, SH.ServiceDailyNoteNotifierTransformerAdaptiveHint, SH.ServiceDailyNoteNotifierTransformerHint);
    }
}