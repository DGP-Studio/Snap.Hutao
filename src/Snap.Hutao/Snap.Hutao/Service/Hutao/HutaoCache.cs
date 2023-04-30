// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.Complex;
using Snap.Hutao.Web.Hutao.Model;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃 API 缓存
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton, typeof(IHutaoCache))]
internal sealed class HutaoCache : IHutaoCache
{
    private readonly IMetadataService metadataService;
    private readonly IServiceScopeFactory scopeFactory;

    private Dictionary<AvatarId, Avatar>? idAvatarExtendedMap;

    private TaskCompletionSource<bool>? databaseViewModelTaskSource;
    private TaskCompletionSource<bool>? wikiAvatarViewModelTaskSource;
    private TaskCompletionSource<bool>? wikiWeaponViewModelTaskSource;

    /// <summary>
    /// 构造一个新的胡桃 API 缓存
    /// </summary>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="scopeFactory">范围工厂</param>
    public HutaoCache(IMetadataService metadataService, IServiceScopeFactory scopeFactory)
    {
        this.metadataService = metadataService;
        this.scopeFactory = scopeFactory;
    }

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
    public List<AvatarCollocationView>? AvatarCollocations { get; set; }

    /// <inheritdoc/>
    public List<WeaponCollocationView>? WeaponCollocations { get; set; }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeForDatabaseViewModelAsync()
    {
        if (databaseViewModelTaskSource != null)
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
        if (wikiAvatarViewModelTaskSource != null)
        {
            return await wikiAvatarViewModelTaskSource.Task.ConfigureAwait(false);
        }

        wikiAvatarViewModelTaskSource = new();
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            Dictionary<AvatarId, Avatar> idAvatarMap = await GetIdAvatarMapExtendedAsync().ConfigureAwait(false);
            Dictionary<WeaponId, Weapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync().ConfigureAwait(false);
            Dictionary<EquipAffixId, Model.Metadata.Reliquary.ReliquarySet> idReliquarySetMap = await metadataService.GetEquipAffixIdToReliquarySetMapAsync().ConfigureAwait(false);

            List<AvatarCollocation> avatarCollocationsRaw;
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
                avatarCollocationsRaw = await hutaoService.GetAvatarCollocationsAsync().ConfigureAwait(false);
            }

            AvatarCollocations = avatarCollocationsRaw.Select(co => new AvatarCollocationView()
            {
                AvatarId = co.AvatarId,
                Avatars = co.Avatars.Select(a => new AvatarView(idAvatarMap[a.Item], a.Rate)).ToList(),
                Weapons = co.Weapons.Select(w => new WeaponView(idWeaponMap[w.Item], w.Rate)).ToList(),
                ReliquarySets = co.Reliquaries.Select(r => new ReliquarySetView(r, idReliquarySetMap)).ToList(),
            }).ToList();

            wikiAvatarViewModelTaskSource.TrySetResult(true);
            return true;
        }

        wikiAvatarViewModelTaskSource.TrySetResult(false);
        return false;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeForWikiWeaponViewModelAsync()
    {
        if (wikiWeaponViewModelTaskSource != null)
        {
            return await wikiWeaponViewModelTaskSource.Task.ConfigureAwait(false);
        }

        wikiWeaponViewModelTaskSource = new();
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            Dictionary<AvatarId, Avatar> idAvatarMap = await GetIdAvatarMapExtendedAsync().ConfigureAwait(false);

            List<WeaponCollocation> weaponCollocationsRaw;
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
                weaponCollocationsRaw = await hutaoService.GetWeaponCollocationsAsync().ConfigureAwait(false);
            }

            WeaponCollocations = weaponCollocationsRaw.Select(co => new WeaponCollocationView()
            {
                WeaponId = co.WeaponId,
                Avatars = co.Avatars.Select(a => new AvatarView(idAvatarMap[a.Item], a.Rate)).ToList(),
            }).ToList();

            wikiWeaponViewModelTaskSource.TrySetResult(true);
            return true;
        }

        wikiWeaponViewModelTaskSource.TrySetResult(false);
        return false;
    }

    private async ValueTask<Dictionary<AvatarId, Avatar>> GetIdAvatarMapExtendedAsync()
    {
        if (idAvatarExtendedMap == null)
        {
            Dictionary<AvatarId, Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            idAvatarExtendedMap = AvatarIds.InsertPlayers(idAvatarMap);
        }

        return idAvatarExtendedMap;
    }

    private async Task AvatarAppearanceRankAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<AvatarAppearanceRank> avatarAppearanceRanksRaw;
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
            avatarAppearanceRanksRaw = await hutaoService.GetAvatarAppearanceRanksAsync().ConfigureAwait(false);
        }

        AvatarAppearanceRanks = avatarAppearanceRanksRaw.OrderByDescending(r => r.Floor).Select(rank => new AvatarRankView
        {
            Floor = string.Format(SH.ModelBindingHutaoComplexRankFloor, rank.Floor),
            Avatars = rank.Ranks.OrderByDescending(r => r.Rate).Select(rank => new AvatarView(idAvatarMap[rank.Item], rank.Rate)).ToList(),
        }).ToList();
    }

    private async Task AvatarUsageRanksAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<AvatarUsageRank> avatarUsageRanksRaw;
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
            avatarUsageRanksRaw = await hutaoService.GetAvatarUsageRanksAsync().ConfigureAwait(false);
        }

        AvatarUsageRanks = avatarUsageRanksRaw.OrderByDescending(r => r.Floor).Select(rank => new AvatarRankView
        {
            Floor = string.Format(SH.ModelBindingHutaoComplexRankFloor, rank.Floor),
            Avatars = rank.Ranks.OrderByDescending(r => r.Rate).Select(rank => new AvatarView(idAvatarMap[rank.Item], rank.Rate)).ToList(),
        }).ToList();
    }

    private async Task AvatarConstellationInfosAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<AvatarConstellationInfo> avatarConstellationInfosRaw;
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
            avatarConstellationInfosRaw = await hutaoService.GetAvatarConstellationInfosAsync().ConfigureAwait(false);
        }

        AvatarConstellationInfos = avatarConstellationInfosRaw.OrderBy(i => i.HoldingRate).Select(info =>
        {
            return new AvatarConstellationInfoView(idAvatarMap[info.AvatarId], info.HoldingRate, info.Constellations.SelectList(x => x.Rate));
        }).ToList();
    }

    private async Task TeamAppearancesAsync(Dictionary<AvatarId, Avatar> idAvatarMap)
    {
        List<TeamAppearance> teamAppearancesRaw;
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
            teamAppearancesRaw = await hutaoService.GetTeamAppearancesAsync().ConfigureAwait(false);
        }

        TeamAppearances = teamAppearancesRaw.OrderByDescending(t => t.Floor).Select(team => new TeamAppearanceView(team, idAvatarMap)).ToList();
    }

    private async Task OverviewAsync()
    {
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
            Overview = await hutaoService.GetOverviewAsync().ConfigureAwait(false);
        }
    }
}