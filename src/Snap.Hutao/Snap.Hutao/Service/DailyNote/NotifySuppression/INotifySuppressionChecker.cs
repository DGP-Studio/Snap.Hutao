// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.DailyNote.NotifySuppression;

internal interface INotifySuppressionChecker
{
    bool ShouldNotify(INotifySuppressionContext context);

    bool GetIsSuppressed(INotifySuppressionContext context);

    void SetIsSuppressed(INotifySuppressionContext context, bool suppressed);

    DailyNoteNotifyInfo NotifyInfo(INotifySuppressionContext context);
}