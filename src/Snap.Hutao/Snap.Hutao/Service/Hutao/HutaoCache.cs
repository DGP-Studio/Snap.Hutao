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

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃 API 缓存
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IHutaoCache))]
internal sealed partial class HutaoCache : IHutaoCache
{
    private readonly IMetadataService metadataService;
    private readonly IServiceProvider serviceProvider;

    private Dictionary<AvatarId, Avatar>? idAvatarExtendedMap;

    private TaskCompletionSource<bool>? databaseViewModelTaskSource;
    private TaskCompletionSource<bool>? wikiAvatarViewModelTaskSource;
    private TaskCompletionSource<bool>? wikiWeaponViewModelTaskSource;

    /// <inheritdoc/>
    public List<AvatarRankView>? AvatarUsageRanks { get; set; }

    /// <inheritdoc/>
    public List<AvatarRankView>? AvatarAppearanceRanks { get; set; }

    /// <inheritdoc/>
    public List<AvatarConstellationInfoView>? AvatarConstellationInfos { get; set; }

    /// <inheritdoc/>
    public List<TeamAppearanceView>? TeamAppearances { get; set; }

    /// <inheritdoc/>
    public Overview? Overview { get; set; }

    /// <inheritdoc/>
    public Dictionary<AvatarId, AvatarCollocationView>? AvatarCollocations { get; set; }

    /// <inheritdoc/>
    public Dictionary<WeaponId, WeaponCollocationView>? WeaponCollocations { get; set; }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeForDatabaseViewModelAsync()
    {
        if (databaseViewModelTaskSource is not null)
        {
            return await databaseViewModelTaskSource.Task.ConfigureAwait(false);
        }

        databaseViewModelTaskSource = new();
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            Dictionary<AvatarId, Avatar> idAvatarMap = await GetIdAvatarMapExtendedAsync().ConfigureAwait(false);
            List<Task> tasks = new(5)
            {
                AvatarAppearanceRankAsync(idAvatarMap),
                AvatarUsageRanksAsync(idAvatarMap),
                AvatarConstellationInfosAsync(idAvatarMap),
                TeamAppearancesAsync(idAvatarMap),
                OverviewAsync(),
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);

            databaseViewModelTaskSource.TrySetResult(true);
            return true;
        }

        databaseViewModelTaskSource.TrySetResult(false);
        return false;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeForWikiAvatarViewModelAsync()
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
    public async ValueTask<bool> InitializeForWikiWeaponViewModelAsync()
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
        List<AvatarCollocation> avatarCollocationsRaw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
            avatarCollocationsRaw = await hutaoService.GetAvatarCollocationsAsync().ConfigureAwait(false);
        }

        AvatarCollocations = avatarCollocationsRaw.SelectList(co => new AvatarCollocationView()
        {
            AvatarId = co.AvatarId,
            Avatars = co.Avatars.SelectList(a => new AvatarView(idAvatarMap[a.Item], a.Rate)),
            Weapons = co.Weapons.SelectList(w => new WeaponView(idWeaponMap[w.Item], w.Rate)),
            ReliquarySets = co.Reliquaries.SelectList(r => new ReliquarySetView(r, idReliquarySetMap)),
        }).ToDictionary(a => a.AvatarId);
    }

    private async ValueTask WeaponCollocationsAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<WeaponCollocation> weaponCollocationsRaw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
            weaponCollocationsRaw = await hutaoService.GetWeaponCollocationsAsync().ConfigureAwait(false);
        }

        WeaponCollocations = weaponCollocationsRaw.SelectList(co => new WeaponCollocationView()
        {
            WeaponId = co.WeaponId,
            Avatars = co.Avatars.SelectList(a => new AvatarView(idAvatarMap[a.Item], a.Rate)),
        }).ToDictionary(w => w.WeaponId);
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarAppearanceRankAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<AvatarAppearanceRank> avatarAppearanceRanksRaw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
            avatarAppearanceRanksRaw = await hutaoService.GetAvatarAppearanceRanksAsync().ConfigureAwait(false);
        }

        AvatarAppearanceRanks = avatarAppearanceRanksRaw.SortByDescending(r => r.Floor).SelectList(rank => new AvatarRankView
        {
            Floor = SH.ModelBindingHutaoComplexRankFloor.Format(rank.Floor),
            Avatars = rank.Ranks.SortByDescending(r => r.Rate).SelectList(rank => new AvatarView(idAvatarMap[rank.Item], rank.Rate)),
        });
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarUsageRanksAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<AvatarUsageRank> avatarUsageRanksRaw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
            avatarUsageRanksRaw = await hutaoService.GetAvatarUsageRanksAsync().ConfigureAwait(false);
        }

        AvatarUsageRanks = avatarUsageRanksRaw.SortByDescending(r => r.Floor).SelectList(rank => new AvatarRankView
        {
            Floor = SH.ModelBindingHutaoComplexRankFloor.Format(rank.Floor),
            Avatars = rank.Ranks.SortByDescending(r => r.Rate).SelectList(rank => new AvatarView(idAvatarMap[rank.Item], rank.Rate)),
        });
    }

    [SuppressMessage("", "SH003")]
    private async Task AvatarConstellationInfosAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<AvatarConstellationInfo> avatarConstellationInfosRaw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
            avatarConstellationInfosRaw = await hutaoService.GetAvatarConstellationInfosAsync().ConfigureAwait(false);
        }

        AvatarConstellationInfos = avatarConstellationInfosRaw.SortBy(i => i.HoldingRate).SelectList(info =>
        {
            return new AvatarConstellationInfoView(idAvatarMap[info.AvatarId], info.HoldingRate, info.Constellations.SelectList(x => x.Rate));
        });
    }

    [SuppressMessage("", "SH003")]
    private async Task TeamAppearancesAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<TeamAppearance> teamAppearancesRaw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
            teamAppearancesRaw = await hutaoService.GetTeamAppearancesAsync().ConfigureAwait(false);
        }

        TeamAppearances = teamAppearancesRaw.SortByDescending(t => t.Floor).SelectList(team => new TeamAppearanceView(team, idAvatarMap));
    }

    [SuppressMessage("", "SH003")]
    private async Task OverviewAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
            Overview = await hutaoService.GetOverviewAsync().ConfigureAwait(false);
        }
    }
}