// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.DailyNote.NotifySuppression;

internal sealed class TransformerNotifySuppressionChecker : INotifySuppressionChecker
{
    public bool ShouldNotify(INotifySuppressionContext context)
    {
        bool result = context.Entry is { TransformerNotify: true, DailyNote.Transformer: { Obtained: true, RecoveryTime.Reached: true } };
        context.Entry.TransformerDotVisible = result;
        return result;
    }

    public bool GetIsSuppressed(INotifySuppressionContext context)
    {
        return context.Entry.TransformerNotifySuppressed;
    }

    public void SetIsSuppressed(INotifySuppressionContext context, bool suppressed)
    {
        context.Entry.TransformerNotifySuppressed = suppressed;
    }

    public DailyNoteNotifyInfo NotifyInfo(INotifySuppressionContext context)
    {
        return new(SH.ServiceDailyNoteNotifierTransformer, SH.ServiceDailyNoteNotifierTransformerAdaptiveHint, SH.ServiceDailyNoteNotifierTransformerHint);
    }
}