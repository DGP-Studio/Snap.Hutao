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
[HighQuality]
[Injection(InjectAs.Scoped, typeof(ISpiralAbyssRecordService))]
internal class SpiralAbyssRecordService : ISpiralAbyssRecordService
{
    private readonly AppDbContext appDbContext;
    private readonly GameRecordClient gameRecordClient;
    private readonly GameRecordClientOs gameRecordClientOs;

    private string? uid;
    private ObservableCollection<SpiralAbyssEntry>? spiralAbysses;

    /// <summary>
    /// 构造一个新的深渊记录服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="gameRecordClient">游戏记录客户端</param>
    public SpiralAbyssRecordService(AppDbContext appDbContext, GameRecordClient gameRecordClient, GameRecordClientOs gameRecordClientOs)
    {
        this.appDbContext = appDbContext;
        this.gameRecordClient = gameRecordClient;
        this.gameRecordClientOs = gameRecordClientOs;
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
        await RefreshSpiralAbyssCoreAsync(userAndUid, SpiralAbyssSchedule.Last).ConfigureAwait(false);
        await RefreshSpiralAbyssCoreAsync(userAndUid, SpiralAbyssSchedule.Current).ConfigureAwait(false);
    }

    private async Task RefreshSpiralAbyssCoreAsync(UserAndUid userAndUid, SpiralAbyssSchedule schedule)
    {
        Response<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss> response;

        // server determination
        if (userAndUid.Uid.Region == "cn_gf01" || userAndUid.Uid.Region == "cn_qd01")
        {
            response = await gameRecordClient
            .GetSpiralAbyssAsync(userAndUid, schedule)
            .ConfigureAwait(false);
        }
        else
        {
            response = await gameRecordClientOs
            .GetSpiralAbyssAsync(userAndUid, schedule)
            .ConfigureAwait(false);
        }

        if (response.IsOk())
        {
            Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss webSpiralAbyss = response.Data;

            SpiralAbyssEntry? existEntry = spiralAbysses!.SingleOrDefault(s => s.ScheduleId == webSpiralAbyss.ScheduleId);
            if (existEntry != null)
            {
                await ThreadHelper.SwitchToMainThreadAsync();
                existEntry.UpdateSpiralAbyss(webSpiralAbyss);

                await ThreadHelper.SwitchToBackgroundAsync();
                await appDbContext.SpiralAbysses.UpdateAndSaveAsync(existEntry).ConfigureAwait(false);
            }
            else
            {
                SpiralAbyssEntry newEntry = SpiralAbyssEntry.Create(userAndUid.Uid.Value, webSpiralAbyss);

                await ThreadHelper.SwitchToMainThreadAsync();
                spiralAbysses!.Insert(0, newEntry);

                await ThreadHelper.SwitchToBackgroundAsync();
                await appDbContext.SpiralAbysses.AddAndSaveAsync(newEntry).ConfigureAwait(false);
            }
        }
    }
}