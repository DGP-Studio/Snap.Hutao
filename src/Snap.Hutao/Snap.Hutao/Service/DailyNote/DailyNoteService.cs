// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

namespace Snap.Hutao.Service.DailyNote;

/// <summary>
/// 实时便笺服务
/// </summary>
[Injection(InjectAs.Singleton)]
internal class DailyNoteService
{
    private readonly AppDbContext appDbContext;

    /// <summary>
    /// 构造一个新的实时便笺服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    public DailyNoteService(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async ValueTask RefreshDailyNotesAndNotifyAsync()
    {
        GameRecordClient gameRecordClient = Ioc.Default.GetRequiredService<GameRecordClient>();

        foreach (Model.Entity.DailyNoteEntry entry in appDbContext.DailyNotes.Include(n => n.User))
        {
            entry.DailyNote = await gameRecordClient.GetDialyNoteAsync(entry.User, entry.Uid).ConfigureAwait(false);
        }

        await appDbContext.SaveChangesAsync().ConfigureAwait(false);

    }

    private async ValueTask NotifyDailyNoteAsync()
    {

    }
}
