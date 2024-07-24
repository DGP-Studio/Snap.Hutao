// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Hutao;

[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IHutaoSpiralAbyssStatisticsCache))]
internal sealed partial class HutaoSpiralAbyssStatisticsCache : IHutaoSpiralAbyssStatisticsCache
{
    private readonly IMetadataService metadataService;
    private readonly IServiceProvider serviceProvider;

    private Dictionary<AvatarId, Avatar>? idAvatarExtendedMap;

    private TaskCompletionSource<bool>? databaseViewModelTaskSource;
    private TaskCompletionSource<bool>? wikiAvatarViewModelTaskSource;
    private TaskCompletionSource<bool>? wikiWeaponViewModelTaskSource;

    public List<AvatarRankView>? AvatarUsageRanks { get; set; }

    public List<AvatarRankView>? AvatarAppearanceRanks { get; set; }

    public List<AvatarConstellationInfoView>? AvatarConstellationInfos { get; set; }

    public List<TeamAppearanceView>? TeamAppearances { get; set; }

    public Overview? Overview { get; set; }

    public Dictionary<AvatarId, AvatarCollocationView>? AvatarCollocations { get; set; }

    public Dictionary<WeaponId, WeaponCollocationView>? WeaponCollocations { get; set; }

    public async ValueTask<bool> InitializeForSpiralAbyssViewAsync()
    {
        if (databaseViewModelTaskSource is not null)
        {
            return await databaseViewModelTaskSource.Task.ConfigureAwait(false);
        }

        databaseViewModelTaskSource = new();
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            Dictionary<AvatarId, Avatar> idAvatarMap = await GetIdAvatarMapExtendedAsync().ConfigureAwait(false);
            List<Task> tasks =
            [
                AvatarAppearanceRankAsync(idAvatarMap),
                AvatarUsageRanksAsync(idAvatarMap),
                AvatarConstellationInfosAsync(idAvatarMap),
                TeamAppearancesAsync(idAvatarMap),
                OverviewAsync(),
            ];

            await Task.WhenAll(tasks).ConfigureAwait(false);

            databaseViewModelTaskSource.TrySetResult(true);
            return true;
        }

        databaseViewModelTaskSource.TrySetResult(false);
        return false;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeForWikiAvatarViewAsync()
    {
        if (wikiAvatarViewModelTaskSource is not null)
        {
            return await wikiAvatarViewModelTaskSource.Task.ConfigureAwait(false);
        }

        wikiAvatarViewModelTaskSource = new();
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            Dictionary<AvatarId, Avatar> idAvatarMap = await GetIdAvatarMapExtendedAsync().ConfigureAwait(false);
            Dictionary<WeaponId, Weapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync().ConfigureAwait(false);
            Dictionary<ExtendedEquipAffixId, Model.Metadata.Reliquary.ReliquarySet> idReliquarySetMap = await metadataService.GetExtendedEquipAffixIdToReliquarySetMapAsync().ConfigureAwait(false);
            await AvatarCollocationsAsync(idAvatarMap, idWeaponMap, idReliquarySetMap).ConfigureAwait(false);

            wikiAvatarViewModelTaskSource.TrySetResult(true);
            return true;
        }

        wikiAvatarViewModelTaskSource.TrySetResult(false);
        return false;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeForWikiWeaponViewAsync()
    {
        if (wikiWeaponViewModelTaskSource is not null)
        {
            return await wikiWeaponViewModelTaskSource.Task.ConfigureAwait(false);
        }

        wikiWeaponViewModelTaskSource = new();
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            Dictionary<AvatarId, Avatar> idAvatarMap = await GetIdAvatarMapExtendedAsync().ConfigureAwait(false);
            await WeaponCollocationsAsync(idAvatarMap).ConfigureAwait(false);

            wikiWeaponViewModelTaskSource.TrySetResult(true);
            return true;
        }

        wikiWeaponViewModelTaskSource.TrySetResult(false);
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

    private async ValueTask<Dictionary<AvatarId, Avatar>> GetIdAvatarMapExtendedAsync()
    {
        if (idAvatarExtendedMap is null)
        {
            Dictionary<AvatarId, Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            idAvatarExtendedMap = AvatarIds.WithPlayers(idAvatarMap);
        }

        return idAvatarExtendedMap;
    }

    private async ValueTask AvatarCollocationsAsync(Dictionary<AvatarId, Avatar> idAvatarMap, Dictionary<WeaponId, Weapon> idWeaponMap, Dictionary<ExtendedEquipAffixId, Model.Metadata.Reliquary.ReliquarySet> idReliquarySetMap)
    {
        List<AvatarCollocation> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            raw = await hutaoService.GetAvatarCollocationsAsync(false).ConfigureAwait(false);
            rawLast = await hutaoService.GetAvatarCollocationsAsync(true).ConfigureAwait(false);
        }

        AvatarCollocations = CurrentLeftJoinLast(raw, rawLast, data => data.AvatarId, (raw, rawLast) => new AvatarCollocationView()
        {
            AvatarId = raw.AvatarId,
            Avatars = CurrentLeftJoinLast(raw.Avatars, rawLast?.Avatars, data => data.Item, (avatar, avatarLast) => new AvatarView(idAvatarMap[avatar.Item], avatar.Rate, avatarLast?.Rate)).ToList(),
            Weapons = CurrentLeftJoinLast(raw.Weapons, rawLast?.Weapons, data => data.Item, (weapon, weaponLast) => new WeaponView(idWeaponMap[weapon.Item], weapon.Rate, weaponLast?.Rate)).ToList(),
            ReliquarySets = CurrentLeftJoinLast(raw.Reliquaries, rawLast?.Reliquaries, data => data.Item, (relic, relicLast) => new ReliquarySetView(idReliquarySetMap, relic, relicLast)).ToList(),
        }).ToDictionary(a => a.AvatarId);
    }

    private async ValueTask WeaponCollocationsAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<WeaponCollocation> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            raw = await hutaoService.GetWeaponCollocationsAsync(false).ConfigureAwait(false);
            rawLast = await hutaoService.GetWeaponCollocationsAsync(true).ConfigureAwait(false);
        }

        WeaponCollocations = CurrentLeftJoinLast(raw, rawLast, data => data.WeaponId, (raw, rawLast) => new WeaponCollocationView()
        {
            WeaponId = raw.WeaponId,
            Avatars = CurrentLeftJoinLast(raw.Avatars, rawLast?.Avatars, data => data.Item, (avatar, avatarLast) => new AvatarView(idAvatarMap[avatar.Item], avatar.Rate, avatarLast?.Rate)).ToList(),
        }).ToDictionary(w => w.WeaponId);
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarAppearanceRankAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<AvatarAppearanceRank> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            raw = await hutaoService.GetAvatarAppearanceRanksAsync(false).ConfigureAwait(false);
            rawLast = await hutaoService.GetAvatarAppearanceRanksAsync(true).ConfigureAwait(false);
        }

        AvatarAppearanceRanks = CurrentLeftJoinLast(raw.SortByDescending(r => r.Floor), rawLast, data => data.Floor, (raw, rawLast) => new AvatarRankView
        {
            Floor = SH.FormatModelBindingHutaoComplexRankFloor(raw.Floor),
            Avatars = CurrentLeftJoinLast(raw.Ranks.SortByDescending(r => r.Rate), rawLast?.Ranks, data => data.Item, (rank, rankLast) => new AvatarView(idAvatarMap[rank.Item], rank.Rate, rankLast?.Rate)).ToList(),
        }).ToList();
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarUsageRanksAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<AvatarUsageRank> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            raw = await hutaoService.GetAvatarUsageRanksAsync(false).ConfigureAwait(false);
            rawLast = await hutaoService.GetAvatarUsageRanksAsync(true).ConfigureAwait(false);
        }

        AvatarUsageRanks = CurrentLeftJoinLast(raw.SortByDescending(r => r.Floor), rawLast, data => data.Floor, (raw, rawLast) => new AvatarRankView
        {
            Floor = SH.FormatModelBindingHutaoComplexRankFloor(raw.Floor),
            Avatars = CurrentLeftJoinLast(raw.Ranks.SortByDescending(r => r.Rate), rawLast?.Ranks, data => data.Item, (rank, rankLast) => new AvatarView(idAvatarMap[rank.Item], rank.Rate, rankLast?.Rate)).ToList(),
        }).ToList();
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarConstellationInfosAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<AvatarConstellationInfo> raw, rawLast;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            raw = await hutaoService.GetAvatarConstellationInfosAsync(false).ConfigureAwait(false);
            rawLast = await hutaoService.GetAvatarConstellationInfosAsync(true).ConfigureAwait(false);
        }

        AvatarConstellationInfos = CurrentLeftJoinLast(raw.SortBy(i => i.HoldingRate), rawLast, data => data.AvatarId, (raw, rawLast) => new AvatarConstellationInfoView(idAvatarMap[raw.AvatarId], raw.HoldingRate, rawLast?.HoldingRate)
        {
            Rates = CurrentLeftJoinLast(raw.Constellations, rawLast?.Constellations, data => data.Item, (rate, rataLast) => new RateAndDelta(rate.Rate, rataLast?.Rate)).ToList(),
        }).ToList();
    }

    [SuppressMessage("", "SH003")]
    private async Task TeamAppearancesAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<TeamAppearance> teamAppearancesRaw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            teamAppearancesRaw = await hutaoService.GetTeamAppearancesAsync().ConfigureAwait(false);
        }

        TeamAppearances = teamAppearancesRaw.SortByDescending(t => t.Floor).SelectList(team => new TeamAppearanceView(team, idAvatarMap));
    }

    [SuppressMessage("", "SH003")]
    private async Task OverviewAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoSpiralAbyssService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoSpiralAbyssService>();
            Overview = await hutaoService.GetOverviewAsync().ConfigureAwait(false);
        }
    }
}