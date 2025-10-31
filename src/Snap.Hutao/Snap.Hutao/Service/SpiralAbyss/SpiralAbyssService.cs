// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.SpiralAbyss;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Response;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.SpiralAbyss;

[Service(ServiceLifetime.Scoped, typeof(ISpiralAbyssService))]
internal sealed partial class SpiralAbyssService : ISpiralAbyssService
{
    private readonly ISpiralAbyssRepository spiralAbyssRepository;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ITaskContext taskContext;

    private readonly ConcurrentDictionary<PlayerUid, ObservableCollection<SpiralAbyssView>> spiralAbyssCollectionCache = [];
    private readonly AsyncLock collectionLock = new();

    [GeneratedConstructor]
    public partial SpiralAbyssService(IServiceProvider serviceProvider);

    public async ValueTask<ObservableCollection<SpiralAbyssView>> GetSpiralAbyssViewCollectionAsync(SpiralAbyssMetadataContext context, UserAndUid userAndUid)
    {
        using (await collectionLock.LockAsync().ConfigureAwait(false))
        {
            if (spiralAbyssCollectionCache.TryGetValue(userAndUid.Uid, out ObservableCollection<SpiralAbyssView>? collection))
            {
                return collection;
            }

            await taskContext.SwitchToBackgroundAsync();
            FrozenDictionary<uint, SpiralAbyssEntry> entryMap = spiralAbyssRepository.GetSpiralAbyssEntryMapByUid(userAndUid.Uid.Value);

            ObservableCollection<SpiralAbyssView> result = context.IdTowerScheduleMap.Values
                .Select(sch => SpiralAbyssView.From(entryMap.GetValueOrDefault(sch.Id), sch, context))
                .OrderByDescending(e => e.ScheduleId)
                .ToObservableCollection();

            spiralAbyssCollectionCache.TryAdd(userAndUid.Uid, result);
            return result;
        }
    }

    public async ValueTask RefreshSpiralAbyssAsync(SpiralAbyssMetadataContext context, UserAndUid userAndUid)
    {
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            IMessenger messenger = scope.ServiceProvider.GetRequiredService<IMessenger>();
            IOverseaSupportFactory<IGameRecordClient> gameRecordClientFactory = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>();
            IGameRecordClient gameRecordClient = gameRecordClientFactory.Create(userAndUid.IsOversea);

            // request the index first
            Response<PlayerInfo> infoResponse = await gameRecordClient
                .GetPlayerInfoAsync(userAndUid)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(infoResponse, messenger))
            {
                return;
            }

            await PrivateRefreshSpiralAbyssAsync(messenger, gameRecordClient, context, userAndUid, ScheduleType.Last).ConfigureAwait(false);
            await PrivateRefreshSpiralAbyssAsync(messenger, gameRecordClient, context, userAndUid, ScheduleType.Current).ConfigureAwait(false);
        }
    }

    private async ValueTask PrivateRefreshSpiralAbyssAsync(IMessenger messenger, IGameRecordClient gameRecordClient, SpiralAbyssMetadataContext context, UserAndUid userAndUid, ScheduleType schedule)
    {
        if (!spiralAbyssCollectionCache.TryGetValue(userAndUid.Uid, out ObservableCollection<SpiralAbyssView>? spiralAbysses))
        {
            return;
        }

        Response<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss> response = await gameRecordClient
            .GetSpiralAbyssAsync(userAndUid, schedule)
            .ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(response, messenger, out Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss? webSpiralAbyss))
        {
            return;
        }

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
            spiralAbyssRepository.UpdateSpiralAbyssEntry(view.Entity);
            targetEntry = view.Entity;
        }
        else
        {
            SpiralAbyssEntry newEntry = SpiralAbyssEntry.Create(userAndUid.Uid.Value, webSpiralAbyss);
            spiralAbyssRepository.AddSpiralAbyssEntry(newEntry);
            targetEntry = newEntry;
        }

        await taskContext.SwitchToMainThreadAsync();
        spiralAbysses.RemoveAt(index);
        spiralAbysses.Insert(index, SpiralAbyssView.From(targetEntry, context));
    }
}