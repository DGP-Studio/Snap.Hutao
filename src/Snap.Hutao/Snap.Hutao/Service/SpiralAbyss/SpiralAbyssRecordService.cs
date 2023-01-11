// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Response;
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

    private string? uid;
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
    public async Task<ObservableCollection<SpiralAbyssEntry>> GetSpiralAbyssCollectionAsync(UserAndUid userAndUid)
    {
        if (uid != userAndUid.Uid.Value)
        {
            spiralAbysses = null;
        }

        uid = userAndUid.Uid.Value;
        if (spiralAbysses == null)
        {
            List<SpiralAbyssEntry> entries = await appDbContext.SpiralAbysses
                .AsNoTracking()
                .Where(s => s.Uid == userAndUid.Uid.Value)
                .OrderByDescending(s => s.ScheduleId)
                .ToListAsync()
                .ConfigureAwait(false);

            await ThreadHelper.SwitchToMainThreadAsync();
            spiralAbysses = new(entries);
        }

        return spiralAbysses;
    }

    /// <inheritdoc/>
    public async Task RefreshSpiralAbyssAsync(UserAndUid userAndUid)
    {
        Response<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss> lastResponse = await gameRecordClient
            .GetSpiralAbyssAsync(userAndUid, SpiralAbyssSchedule.Last)
            .ConfigureAwait(false);

        if (lastResponse.IsOk())
        {
            Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss last = lastResponse.Data;

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
                SpiralAbyssEntry entry = SpiralAbyssEntry.Create(userAndUid.Uid.Value, last);

                await ThreadHelper.SwitchToMainThreadAsync();
                spiralAbysses!.Insert(0, entry);

                await ThreadHelper.SwitchToBackgroundAsync();
                await appDbContext.SpiralAbysses.AddAndSaveAsync(entry).ConfigureAwait(false);
            }
        }

        Response<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss> currentResponse = await gameRecordClient
            .GetSpiralAbyssAsync(userAndUid, SpiralAbyssSchedule.Current)
            .ConfigureAwait(false);

        if (currentResponse.IsOk())
        {
            Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss current = currentResponse.Data;

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
                SpiralAbyssEntry entry = SpiralAbyssEntry.Create(userAndUid.Uid.Value, current);

                await ThreadHelper.SwitchToMainThreadAsync();
                spiralAbysses!.Insert(0, entry);

                await ThreadHelper.SwitchToBackgroundAsync();
                await appDbContext.SpiralAbysses.AddAndSaveAsync(entry).ConfigureAwait(false);
            }
        }
    }
}