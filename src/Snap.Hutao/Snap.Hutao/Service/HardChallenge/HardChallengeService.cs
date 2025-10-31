// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.HardChallenge;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;
using Snap.Hutao.Web.Response;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.HardChallenge;

[Service(ServiceLifetime.Scoped, typeof(IHardChallengeService))]
internal sealed partial class HardChallengeService : IHardChallengeService
{
    private readonly IHardChallengeRepository hardChallengeRepository;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ITaskContext taskContext;

    private readonly ConcurrentDictionary<PlayerUid, ObservableCollection<HardChallengeView>> hardChallengeCollectionCache = [];
    private readonly AsyncLock collectionLock = new();

    [GeneratedConstructor]
    public partial HardChallengeService(IServiceProvider serviceProvider);

    public async ValueTask<ObservableCollection<HardChallengeView>> GetHardChallengeViewCollectionAsync(HardChallengeMetadataContext context, UserAndUid userAndUid)
    {
        using (await collectionLock.LockAsync().ConfigureAwait(false))
        {
            if (hardChallengeCollectionCache.TryGetValue(userAndUid.Uid, out ObservableCollection<HardChallengeView>? collection))
            {
                return collection;
            }

            await taskContext.SwitchToBackgroundAsync();
            FrozenDictionary<uint, HardChallengeEntry> entryMap = hardChallengeRepository.GetHardChallengeMapByUid(userAndUid.Uid.Value);

            ObservableCollection<HardChallengeView> result = context.IdHardChallengeScheduleMap.Values
                .Select(sch => HardChallengeView.Create(entryMap.GetValueOrDefault(sch.Id), sch, context))
                .OrderByDescending(e => e.ScheduleId)
                .ToObservableCollection();

            hardChallengeCollectionCache.TryAdd(userAndUid.Uid, result);
            return result;
        }
    }

    public async ValueTask<ImmutableArray<AvatarView>> GetBlingAvatarsAsync(HardChallengeMetadataContext context, UserAndUid userAndUid)
    {
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            IOverseaSupportFactory<IGameRecordClient> gameRecordClientFactory = scope.ServiceProvider.GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>();
            IGameRecordClient gameRecordClient = gameRecordClientFactory.Create(userAndUid.IsOversea);

            Response<HardChallengePopularity> response = await gameRecordClient
                .GetHardChallengePopularityAsync(userAndUid)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out HardChallengePopularity? webHardChallengePopularity))
            {
                return [];
            }

            return webHardChallengePopularity.AvatarList.SelectAsArray(AvatarView.Create, context);
        }
    }

    public async ValueTask RefreshHardChallengeAsync(HardChallengeMetadataContext context, UserAndUid userAndUid)
    {
        Web.Hoyolab.Takumi.GameRecord.HardChallenge.HardChallenge? webHardChallenge;
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

            Response<Web.Hoyolab.Takumi.GameRecord.HardChallenge.HardChallenge> response = await gameRecordClient
                .GetHardChallengeAsync(userAndUid)
                .ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out webHardChallenge))
            {
                return;
            }
        }

        foreach (HardChallengeData hardChallengeData in webHardChallenge.Data)
        {
            if (!hardChallengeData.SinglePlayer.HasData && !hardChallengeData.MultiPlayer.HasData)
            {
                continue;
            }

            await PrivateRefreshHardChallengeAsync(context, userAndUid, hardChallengeData).ConfigureAwait(false);
        }
    }

    private async ValueTask PrivateRefreshHardChallengeAsync(HardChallengeMetadataContext context, UserAndUid userAndUid, HardChallengeData hardChallengeData)
    {
        if (!hardChallengeCollectionCache.TryGetValue(userAndUid.Uid, out ObservableCollection<HardChallengeView>? hardChallenges))
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(context);

        int index = hardChallenges.FirstIndexOf(s => s.ScheduleId == hardChallengeData.Schedule.ScheduleId);
        if (index < 0)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();
        HardChallengeView view = hardChallenges[index];

        HardChallengeEntry targetEntry;
        if (view.Entity is not null)
        {
            view.Entity.HardChallengeData = hardChallengeData;
            hardChallengeRepository.UpdateHardChallengeEntry(view.Entity);
            targetEntry = view.Entity;
        }
        else
        {
            HardChallengeEntry newEntry = HardChallengeEntry.Create(userAndUid.Uid.Value, hardChallengeData);
            hardChallengeRepository.AddHardChallengeEntry(newEntry);
            targetEntry = newEntry;
        }

        await taskContext.SwitchToMainThreadAsync();
        hardChallenges.RemoveAt(index);
        hardChallenges.Insert(index, HardChallengeView.Create(targetEntry, context));
    }
}