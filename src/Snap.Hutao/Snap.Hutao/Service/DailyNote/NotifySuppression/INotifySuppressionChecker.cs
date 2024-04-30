// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.DailyNote.NotifySuppression;

internal interface INotifySuppressionChecker
{
    bool SuppressCondition(INotifySuppressionContext context);

    bool GetSuppressed(INotifySuppressionContext context);

    void SetSuppressed(INotifySuppressionContext context, bool suppressed);

    DailyNoteNotifyInfo SuppressInfo(INotifySuppressionContext context);
}