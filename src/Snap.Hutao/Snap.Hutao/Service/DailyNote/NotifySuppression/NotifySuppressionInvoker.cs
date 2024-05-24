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
        if (checker.SuppressCondition(context))
        {
            if (!checker.GetSuppressed(context))
            {
                context.Add(checker.SuppressInfo(context));
                checker.SetSuppressed(context, true);
            }

            checker.SetSuppressed(context, false);
        }
    }
}