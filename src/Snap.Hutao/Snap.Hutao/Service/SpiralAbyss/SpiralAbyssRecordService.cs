// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Response;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.SpiralAbyss;

/// <summary>
/// 深渊记录服务
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(ISpiralAbyssRecordService))]
internal sealed partial class SpiralAbyssRecordService : ISpiralAbyssRecordService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly ISpiralAbyssRecordDbService spiralAbyssRecordDbService;

    private string? uid;
    private ObservableCollection<SpiralAbyssEntry>? spiralAbysses;

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<SpiralAbyssEntry>> GetSpiralAbyssCollectionAsync(UserAndUid userAndUid)
    {
        if (uid != userAndUid.Uid.Value)
        {
            spiralAbysses = null;
        }

        uid = userAndUid.Uid.Value;
        spiralAbysses ??= await spiralAbyssRecordDbService
            .GetSpiralAbyssEntryCollectionByUidAsync(userAndUid.Uid.Value)
            .ConfigureAwait(false);

        return spiralAbysses;
    }

    /// <inheritdoc/>
    public async ValueTask RefreshSpiralAbyssAsync(UserAndUid userAndUid)
    {
        await RefreshSpiralAbyssCoreAsync(userAndUid, SpiralAbyssSchedule.Last).ConfigureAwait(false);
        await RefreshSpiralAbyssCoreAsync(userAndUid, SpiralAbyssSchedule.Current).ConfigureAwait(false);
    }

    private async ValueTask RefreshSpiralAbyssCoreAsync(UserAndUid userAndUid, SpiralAbyssSchedule schedule)
    {
        Response<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss> response = await serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
            .Create(userAndUid.User.IsOversea)
            .GetSpiralAbyssAsync(userAndUid, schedule)
            .ConfigureAwait(false);

        if (response.IsOk())
        {
            Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss webSpiralAbyss = response.Data;

            if (spiralAbysses!.SingleOrDefault(s => s.ScheduleId == webSpiralAbyss.ScheduleId) is SpiralAbyssEntry existEntry)
            {
                await taskContext.SwitchToMainThreadAsync();
                existEntry.UpdateSpiralAbyss(webSpiralAbyss);

                await taskContext.SwitchToBackgroundAsync();
                await spiralAbyssRecordDbService.UpdateSpiralAbyssEntryAsync(existEntry).ConfigureAwait(false);
            }
            else
            {
                SpiralAbyssEntry newEntry = SpiralAbyssEntry.From(userAndUid.Uid.Value, webSpiralAbyss);

                await taskContext.SwitchToMainThreadAsync();
                spiralAbysses!.Insert(0, newEntry);

                await taskContext.SwitchToBackgroundAsync();
                await spiralAbyssRecordDbService.AddSpiralAbyssEntryAsync(newEntry).ConfigureAwait(false);
            }
        }
    }
}