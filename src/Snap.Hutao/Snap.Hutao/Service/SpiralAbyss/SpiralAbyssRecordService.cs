// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Service.Metadata;
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
    private readonly IOverseaSupportFactory<IGameRecordClient> gameRecordClientFactory;
    private readonly ISpiralAbyssRecordDbService spiralAbyssRecordDbService;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    private string? uid;
    private ObservableCollection<SpiralAbyssView>? spiralAbysses;
    private SpiralAbyssMetadataContext? metadataContext;

    public async ValueTask<bool> InitializeAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            metadataContext = new()
            {
                IdScheduleMap = await metadataService.GetIdToTowerScheduleMapAsync().ConfigureAwait(false),
                IdFloorMap = await metadataService.GetIdToTowerFloorMapAsync().ConfigureAwait(false),
                IdLevelGroupMap = await metadataService.GetGroupIdToTowerLevelGroupMapAsync().ConfigureAwait(false),
                IdMonsterMap = await metadataService.GetRelationshipIdToMonsterMapAsync().ConfigureAwait(false),
                IdAvatarMap = AvatarIds.WithPlayers(await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false)),
            };
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
            Dictionary<uint, SpiralAbyssEntry> entryMap = await spiralAbyssRecordDbService
                .GetSpiralAbyssEntryListByUidAsync(userAndUid.Uid.Value)
                .ConfigureAwait(false);

            ArgumentNullException.ThrowIfNull(metadataContext);
            spiralAbysses = metadataContext.IdScheduleMap.Values
                .Select(sch => SpiralAbyssView.From(entryMap.GetValueOrDefault(sch.Id), sch, metadataContext))
                .OrderByDescending(e => e.ScheduleId)
                .ToObservableCollection();
        }

        return spiralAbysses;
    }

    /// <inheritdoc/>
    public async ValueTask RefreshSpiralAbyssAsync(UserAndUid userAndUid)
    {
        // request the index first
        await gameRecordClientFactory
            .Create(userAndUid.User.IsOversea)
            .GetPlayerInfoAsync(userAndUid)
            .ConfigureAwait(false);

        await RefreshSpiralAbyssCoreAsync(userAndUid, SpiralAbyssSchedule.Last).ConfigureAwait(false);
        await RefreshSpiralAbyssCoreAsync(userAndUid, SpiralAbyssSchedule.Current).ConfigureAwait(false);
    }

    private async ValueTask RefreshSpiralAbyssCoreAsync(UserAndUid userAndUid, SpiralAbyssSchedule schedule)
    {
        Response<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss> response = await gameRecordClientFactory
            .Create(userAndUid.User.IsOversea)
            .GetSpiralAbyssAsync(userAndUid, schedule)
            .ConfigureAwait(false);

        if (response.IsOk())
        {
            Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss webSpiralAbyss = response.Data;

            ArgumentNullException.ThrowIfNull(spiralAbysses);
            ArgumentNullException.ThrowIfNull(metadataContext);

            int index = spiralAbysses.FirstIndexOf(s => s.ScheduleId == webSpiralAbyss.ScheduleId);
            if (index >= 0)
            {
                await taskContext.SwitchToBackgroundAsync();
                SpiralAbyssView view = spiralAbysses[index];

                SpiralAbyssEntry targetEntry;
                if (view.Entity is not null)
                {
                    view.Entity.SpiralAbyss = webSpiralAbyss;
                    await spiralAbyssRecordDbService.UpdateSpiralAbyssEntryAsync(view.Entity).ConfigureAwait(false);
                    targetEntry = view.Entity;
                }
                else
                {
                    SpiralAbyssEntry newEntry = SpiralAbyssEntry.From(userAndUid.Uid.Value, webSpiralAbyss);
                    await spiralAbyssRecordDbService.AddSpiralAbyssEntryAsync(newEntry).ConfigureAwait(false);
                    targetEntry = newEntry;
                }

                await taskContext.SwitchToMainThreadAsync();
                spiralAbysses.RemoveAt(index);
                spiralAbysses.Insert(index, SpiralAbyssView.From(targetEntry, metadataContext));
            }
        }
    }
}