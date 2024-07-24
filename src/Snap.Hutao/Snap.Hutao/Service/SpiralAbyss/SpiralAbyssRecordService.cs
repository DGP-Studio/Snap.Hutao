// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.SpiralAbyss;
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
    private readonly ISpiralAbyssRecordDbService spiralAbyssRecordDbService;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    private string? uid;
    private ObservableCollection<SpiralAbyssView>? spiralAbysses;
    private SpiralAbyssMetadataContext? metadataContext;

    public async ValueTask<bool> InitializeAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            metadataContext = await metadataService.GetContextAsync<SpiralAbyssMetadataContext>().ConfigureAwait(false);
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<SpiralAbyssView>> GetSpiralAbyssViewCollectionAsync(UserAndUid userAndUid)
    {
        if (uid != userAndUid.Uid.Value)
        {
            spiralAbysses = null;
        }

        uid = userAndUid.Uid.Value;
        if (spiralAbysses is null)
        {
            await taskContext.SwitchToBackgroundAsync();
            Dictionary<uint, SpiralAbyssEntry> entryMap = spiralAbyssRecordDbService.GetSpiralAbyssEntryMapByUid(userAndUid.Uid.Value);

            ArgumentNullException.ThrowIfNull(metadataContext);
            spiralAbysses = metadataContext.IdTowerScheduleMap.Values
                .Select(sch => SpiralAbyssView.From(entryMap.GetValueOrDefault(sch.Id), sch, metadataContext))
                .OrderByDescending(e => e.ScheduleId)
                .ToObservableCollection();
        }

        return spiralAbysses;
    }

    /// <inheritdoc/>
    public async ValueTask RefreshSpiralAbyssAsync(UserAndUid userAndUid)
    {
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            IOverseaSupportFactory<IGameRecordClient> gameRecordClientFactory = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>();

            // request the index first
            await gameRecordClientFactory
                .Create(userAndUid.User.IsOversea)
                .GetPlayerInfoAsync(userAndUid)
                .ConfigureAwait(false);

            await RefreshSpiralAbyssCoreAsync(userAndUid, ScheduleType.Last).ConfigureAwait(false);
            await RefreshSpiralAbyssCoreAsync(userAndUid, ScheduleType.Current).ConfigureAwait(false);
        }
    }

    private async ValueTask RefreshSpiralAbyssCoreAsync(UserAndUid userAndUid, ScheduleType schedule)
    {
        Response<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss> response;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            IOverseaSupportFactory<IGameRecordClient> gameRecordClientFactory = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>();

            response = await gameRecordClientFactory
                .Create(userAndUid.User.IsOversea)
                .GetSpiralAbyssAsync(userAndUid, schedule)
                .ConfigureAwait(false);
        }

        if (!response.IsOk())
        {
            return;
        }

        Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss webSpiralAbyss = response.Data;

        ArgumentNullException.ThrowIfNull(spiralAbysses);
        ArgumentNullException.ThrowIfNull(metadataContext);

        int index = spiralAbysses.FirstIndexOf(s => s.ScheduleId == webSpiralAbyss.ScheduleId);
        if (index < 0)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();
        SpiralAbyssView view = spiralAbysses[index];

        SpiralAbyssEntry targetEntry;
        if (view.Entity is not null)
        {
            view.Entity.SpiralAbyss = webSpiralAbyss;
            spiralAbyssRecordDbService.UpdateSpiralAbyssEntry(view.Entity);
            targetEntry = view.Entity;
        }
        else
        {
            SpiralAbyssEntry newEntry = SpiralAbyssEntry.From(userAndUid.Uid.Value, webSpiralAbyss);
            spiralAbyssRecordDbService.AddSpiralAbyssEntry(newEntry);
            targetEntry = newEntry;
        }

        await taskContext.SwitchToMainThreadAsync();
        spiralAbysses.RemoveAt(index);
        spiralAbysses.Insert(index, SpiralAbyssView.From(targetEntry, metadataContext));
    }
}