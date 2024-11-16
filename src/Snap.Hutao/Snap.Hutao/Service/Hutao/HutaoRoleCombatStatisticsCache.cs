// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.Web.Hutao.RoleCombat;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Hutao;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IHutaoRoleCombatStatisticsCache))]
internal sealed partial class HutaoRoleCombatStatisticsCache : IHutaoRoleCombatStatisticsCache
{
    private readonly IMetadataService metadataService;
    private readonly IServiceProvider serviceProvider;

    private ImmutableDictionary<AvatarId, Avatar>? idAvatarExtendedMap;

    private TaskCompletionSource<bool>? databaseViewModelTaskSource;

    public int RecordTotal { get; set; }

    public List<AvatarView>? AvatarAppearances { get; set; }

    public async ValueTask<bool> InitializeForRoleCombatViewAsync()
    {
        if (databaseViewModelTaskSource is not null)
        {
            return await databaseViewModelTaskSource.Task.ConfigureAwait(false);
        }

        databaseViewModelTaskSource = new();
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            ImmutableDictionary<AvatarId, Avatar> idAvatarMap = await GetIdAvatarMapExtendedAsync().ConfigureAwait(false);
            await AvatarAppearancesAsync(idAvatarMap).ConfigureAwait(false);

            databaseViewModelTaskSource.TrySetResult(true);
            return true;
        }

        databaseViewModelTaskSource.TrySetResult(false);
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<TResult> CurrentLeftJoinLast<TElement, TKey, TResult>(IEnumerable<TElement> current, IEnumerable<TElement>? last, Func<TElement, TKey> keySelector, Func<TElement, TElement?, TResult> resultSelector)
        where TKey : notnull
    {
        if (last is null)
        {
            foreach (TElement element in current)
            {
                yield return resultSelector(element, default);
            }
        }
        else
        {
            Dictionary<TKey, TElement> lastMap = [];
            foreach (TElement element in last)
            {
                lastMap[keySelector(element)] = element;
            }

            foreach (TElement element in current)
            {
                yield return resultSelector(element, lastMap.GetValueOrDefault(keySelector(element)));
            }
        }
    }

    private async ValueTask<ImmutableDictionary<AvatarId, Avatar>> GetIdAvatarMapExtendedAsync()
    {
        if (idAvatarExtendedMap is null)
        {
            ImmutableDictionary<AvatarId, Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            idAvatarExtendedMap = AvatarIds.WithPlayers(idAvatarMap);
        }

        return idAvatarExtendedMap;
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarAppearancesAsync(ImmutableDictionary<AvatarId, Avatar> idAvatarMap)
    {
        RoleCombatStatisticsItem raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoRoleCombatService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoRoleCombatService>();
            raw = await hutaoService.GetRoleCombatStatisticsItemAsync(false).ConfigureAwait(false);
            rawLast = await hutaoService.GetRoleCombatStatisticsItemAsync(true).ConfigureAwait(false);
        }

        RecordTotal = raw.RecordTotal;
        AvatarAppearances = CurrentLeftJoinLast(raw.BackupAvatarRates.SortByDescending(ir => ir.Rate), rawLast?.BackupAvatarRates, data => data.Item, (data, dataLast) => new AvatarView(idAvatarMap[data.Item], data.Rate, dataLast?.Rate)).ToList();
    }
}