// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Service.DailyNote.NotifySuppression;

internal sealed class NotifySuppressionContext : INotifySuppressionContext
{
    private readonly DailyNoteEntry entry;
    private readonly List<DailyNoteNotifyInfo> infos;

    public NotifySuppressionContext(DailyNoteEntry entry, List<DailyNoteNotifyInfo> infos)
    {
        this.entry = entry;
        this.infos = infos;
    }

    public DailyNoteEntry Entry { get => entry; }

    [SuppressMessage("", "SH002")]
    public void Add(DailyNoteNotifyInfo info)
    {
        infos.Add(info);
    }
}