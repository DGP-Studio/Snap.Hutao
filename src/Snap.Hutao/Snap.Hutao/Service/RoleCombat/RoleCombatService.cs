// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.RoleCombat;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;
using Snap.Hutao.Web.Response;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.RoleCombat;

[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IRoleCombatService))]
internal sealed partial class RoleCombatService : IRoleCombatService
{
    private readonly IRoleCombatRepository roleCombatRepository;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    private readonly ConcurrentDictionary<string, ObservableCollection<RoleCombatView>> roleCombatCollectionCache = [];
    private RoleCombatMetadataContext? metadataContext;

    public async ValueTask<bool> InitializeAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            metadataContext = await metadataService.GetContextAsync<RoleCombatMetadataContext>().ConfigureAwait(false);
            return true;
        }

        return false;
    }

    public async ValueTask<ObservableCollection<RoleCombatView>> GetRoleCombatViewCollectionAsync(UserAndUid userAndUid)
    {
        if (roleCombatCollectionCache.TryGetValue(userAndUid.Uid.Value, out ObservableCollection<RoleCombatView>? collection))
        {
            return collection;
        }

        await taskContext.SwitchToBackgroundAsync();
        Dictionary<uint, RoleCombatEntry> entryMap = roleCombatRepository.GetRoleCombatEntryMapByUid(userAndUid.Uid.Value);

        ArgumentNullException.ThrowIfNull(metadataContext);
        ObservableCollection<RoleCombatView> result = metadataContext.IdRoleCombatScheduleMap.Values
            .Select(sch => RoleCombatView.From(entryMap.GetValueOrDefault(sch.Id), sch, metadataContext))
            .OrderByDescending(e => e.ScheduleId)
            .ToObservableCollection();

        roleCombatCollectionCache.TryAdd(userAndUid.Uid.Value, result);
        return result;
    }

    public async ValueTask RefreshRoleCombatAsync(UserAndUid userAndUid)
    {
        Web.Hoyolab.Takumi.GameRecord.RoleCombat.RoleCombat? webRoleCombat;
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            IOverseaSupportFactory<IGameRecordClient> gameRecordClientFactory = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>();

            // request the index first
            await gameRecordClientFactory
                .Create(userAndUid.User.IsOversea)
                .GetPlayerInfoAsync(userAndUid)
                .ConfigureAwait(false);

            Response<Web.Hoyolab.Takumi.GameRecord.RoleCombat.RoleCombat> response = await gameRecordClientFactory
                .Create(userAndUid.User.IsOversea)
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

            await RefreshRoleCombatCoreAsync(userAndUid, roleCombatData).ConfigureAwait(false);
        }
    }

    private async ValueTask RefreshRoleCombatCoreAsync(UserAndUid userAndUid, RoleCombatData roleCombatData)
    {
        if (!roleCombatCollectionCache.TryGetValue(userAndUid.Uid.Value, out ObservableCollection<RoleCombatView>? roleCombats))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(metadataContext);

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
            RoleCombatEntry newEntry = RoleCombatEntry.From(userAndUid.Uid.Value, roleCombatData);
            roleCombatRepository.AddRoleCombatEntry(newEntry);
            targetEntry = newEntry;
        }

        await taskContext.SwitchToMainThreadAsync();
        roleCombats.RemoveAt(index);
        roleCombats.Insert(index, RoleCombatView.From(targetEntry, metadataContext));
    }
}