// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.RoleCombat;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;
using Snap.Hutao.Web.Response;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.RoleCombat;

[Service(ServiceLifetime.Scoped, typeof(IRoleCombatService))]
internal sealed partial class RoleCombatService : IRoleCombatService
{
    private readonly IRoleCombatRepository roleCombatRepository;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ITaskContext taskContext;

    private readonly ConcurrentDictionary<PlayerUid, ObservableCollection<RoleCombatView>> roleCombatCollectionCache = [];
    private readonly AsyncLock collectionLock = new();

    [GeneratedConstructor]
    public partial RoleCombatService(IServiceProvider serviceProvider);

    public async ValueTask<ObservableCollection<RoleCombatView>> GetRoleCombatViewCollectionAsync(RoleCombatMetadataContext context, UserAndUid userAndUid)
    {
        using (await collectionLock.LockAsync().ConfigureAwait(false))
        {
            if (roleCombatCollectionCache.TryGetValue(userAndUid.Uid, out ObservableCollection<RoleCombatView>? collection))
            {
                return collection;
            }

            await taskContext.SwitchToBackgroundAsync();
            FrozenDictionary<uint, RoleCombatEntry> entryMap = roleCombatRepository.GetRoleCombatEntryMapByUid(userAndUid.Uid.Value);

            ObservableCollection<RoleCombatView> result = context.IdRoleCombatScheduleMap.Values
                .Select(schedule => RoleCombatView.Create(entryMap.GetValueOrDefault(schedule.Id), schedule, context))
                .OrderByDescending(e => e.ScheduleId)
                .ToObservableCollection();

            roleCombatCollectionCache.TryAdd(userAndUid.Uid, result);
            return result;
        }
    }

    public async ValueTask RefreshRoleCombatAsync(RoleCombatMetadataContext context, UserAndUid userAndUid)
    {
        Web.Hoyolab.Takumi.GameRecord.RoleCombat.RoleCombat? webRoleCombat;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            IOverseaSupportFactory<IGameRecordClient> gameRecordClientFactory = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>();
            IGameRecordClient gameRecordClient = gameRecordClientFactory.Create(userAndUid.IsOversea);

            // Request the index first
            Response<PlayerInfo> infoResponse = await gameRecordClient
                .GetPlayerInfoAsync(userAndUid)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(infoResponse, scope.ServiceProvider))
            {
                return;
            }

            Response<Web.Hoyolab.Takumi.GameRecord.RoleCombat.RoleCombat> response = await gameRecordClient
                .GetRoleCombatAsync(userAndUid)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out webRoleCombat))
            {
                return;
            }
        }

        foreach (RoleCombatData roleCombatData in webRoleCombat.Data)
        {
            if (!roleCombatData.HasData)
            {
                continue;
            }

            await PrivateRefreshRoleCombatAsync(context, userAndUid, roleCombatData).ConfigureAwait(false);
        }
    }

    private async ValueTask PrivateRefreshRoleCombatAsync(RoleCombatMetadataContext context, UserAndUid userAndUid, RoleCombatData roleCombatData)
    {
        if (!roleCombatCollectionCache.TryGetValue(userAndUid.Uid, out ObservableCollection<RoleCombatView>? roleCombats))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(context);

        int index = roleCombats.FirstIndexOf(s => s.ScheduleId == roleCombatData.Schedule.ScheduleId);
        if (index < 0)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();
        RoleCombatView view = roleCombats[index];

        RoleCombatEntry targetEntry;
        if (view.Entity is not null)
        {
            view.Entity.RoleCombatData = roleCombatData;
            roleCombatRepository.UpdateRoleCombatEntry(view.Entity);
            targetEntry = view.Entity;
        }
        else
        {
            RoleCombatEntry newEntry = RoleCombatEntry.Create(userAndUid.Uid.Value, roleCombatData);
            roleCombatRepository.AddRoleCombatEntry(newEntry);
            targetEntry = newEntry;
        }

        await taskContext.SwitchToMainThreadAsync();
        roleCombats.RemoveAt(index);
        roleCombats.Insert(index, RoleCombatView.Create(targetEntry, context));
    }
}