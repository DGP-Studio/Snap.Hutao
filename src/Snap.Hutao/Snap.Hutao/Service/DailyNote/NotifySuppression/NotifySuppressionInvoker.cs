// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Service.DailyNote.NotifySuppression;

internal static class NotifySuppressionInvoker
{
    public static void Check(DailyNoteEntry entry, out List<DailyNoteNotifyInfo> infos)
    {
        infos = [];
        NotifySuppressionContext context = new(entry, infos);
        context.Invoke<ResinNotifySuppressionChecker>();
        context.Invoke<HomeCoinNotifySuppressionChecker>();
        context.Invoke<DailyTaskNotifySuppressionChecker>();
        context.Invoke<TransformerNotifySuppressionChecker>();
        context.Invoke<ExpeditionNotifySuppressionChecker>();
    }

    [SuppressMessage("", "CA1859")]
    private static void Invoke<T>(this INotifySuppressionContext context)
        where T : INotifySuppressionChecker, new()
    {
        T checker = new();

        // Reach the notify threshold
        if (checker.ShouldNotify(context))
        {
            // If the suppression status is not set, we need to append notify info
            if (!checker.GetIsSuppressed(context))
            {
                context.Add(checker.NotifyInfo(context));
                checker.SetIsSuppressed(context, true);
            }
        }
        else
        {
            // Reset suppression status
            checker.SetIsSuppressed(context, false);
        }
    }
}