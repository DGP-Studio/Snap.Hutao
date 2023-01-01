// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.SpiralAbyss;

/// <summary>
/// 深渊记录服务
/// </summary>
[Injection(InjectAs.Scoped, typeof(ISpiralAbyssRecordService))]
internal class SpiralAbyssRecordService : ISpiralAbyssRecordService
{
    private readonly AppDbContext appDbContext;
    private readonly GameRecordClient gameRecordClient;

    private ObservableCollection<SpiralAbyssEntry>? spiralAbysses;

    /// <summary>
    /// 构造一个新的深渊记录服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="gameRecordClient">游戏记录客户端</param>
    public SpiralAbyssRecordService(AppDbContext appDbContext, GameRecordClient gameRecordClient)
    {
        this.appDbContext = appDbContext;
        this.gameRecordClient = gameRecordClient;
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<SpiralAbyssEntry>> GetSpiralAbyssCollectionAsync()
    {
        if (spiralAbysses == null)
        {
            List<SpiralAbyssEntry> entries = await appDbContext.SpiralAbysses
                .AsNoTracking()
                .OrderByDescending(s => s.ScheduleId)
                .ToListAsync()
                .ConfigureAwait(false);

            await ThreadHelper.SwitchToMainThreadAsync();
            spiralAbysses = new(entries);
        }

        return spiralAbysses;
    }

    /// <inheritdoc/>
    public async Task RefreshSpiralAbyssAsync(UserAndRole userAndRole)
    {
        Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss? last = await gameRecordClient
            .GetSpiralAbyssAsync(userAndRole, SpiralAbyssSchedule.Last)
            .ConfigureAwait(false);

        if (last != null)
        {
            SpiralAbyssEntry? lastEntry = spiralAbysses!.SingleOrDefault(s => s.ScheduleId == last.ScheduleId);
            if (lastEntry != null)
            {
                await ThreadHelper.SwitchToMainThreadAsync();
                lastEntry.UpdateSpiralAbyss(last);

                await ThreadHelper.SwitchToBackgroundAsync();
                await appDbContext.SpiralAbysses.UpdateAndSaveAsync(lastEntry).ConfigureAwait(false);
            }
            else
            {
                SpiralAbyssEntry entry = SpiralAbyssEntry.Create(userAndRole.Role.GameUid, last);

                await appDbContext.SpiralAbysses.AddAndSaveAsync(entry).ConfigureAwait(false);

                await ThreadHelper.SwitchToMainThreadAsync();
                spiralAbysses!.Insert(0, entry);
            }
        }

        Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss? current = await gameRecordClient
            .GetSpiralAbyssAsync(userAndRole, SpiralAbyssSchedule.Current)
            .ConfigureAwait(false);

        if (current != null)
        {
            SpiralAbyssEntry? currentEntry = spiralAbysses!.SingleOrDefault(s => s.ScheduleId == current.ScheduleId);
            if (currentEntry != null)
            {
                await ThreadHelper.SwitchToMainThreadAsync();
                currentEntry.UpdateSpiralAbyss(current);

                await ThreadHelper.SwitchToBackgroundAsync();
                await appDbContext.SpiralAbysses.UpdateAndSaveAsync(currentEntry).ConfigureAwait(false);
            }
            else
            {
                SpiralAbyssEntry entry = SpiralAbyssEntry.Create(userAndRole.Role.GameUid, current);

                await appDbContext.SpiralAbysses.AddAndSaveAsync(entry).ConfigureAwait(false);

                await ThreadHelper.SwitchToMainThreadAsync();
                spiralAbysses!.Insert(0, entry);
            }
        }
    }
}