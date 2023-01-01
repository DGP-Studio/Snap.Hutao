// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Hutao.Model;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃 API 缓存
/// </summary>
[Injection(InjectAs.Singleton, typeof(IHutaoCache))]
internal class HutaoCache : IHutaoCache
{
    private readonly IMetadataService metadataService;
    private readonly IServiceScopeFactory scopeFactory;

    private Dictionary<AvatarId, Avatar>? idAvatarExtendedMap;

    private bool isDatabaseViewModelInitialized;
    private bool isWikiAvatarViewModelInitiaized;
    private bool isWikiWeaponViewModelInitiaized;

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
    public List<ComplexAvatarRank>? AvatarUsageRanks { get; set; }

    /// <inheritdoc/>
    public List<ComplexAvatarRank>? AvatarAppearanceRanks { get; set; }

    /// <inheritdoc/>
    public List<ComplexAvatarConstellationInfo>? AvatarConstellationInfos { get; set; }

    /// <inheritdoc/>
    public List<ComplexTeamRank>? TeamAppearances { get; set; }

    /// <inheritdoc/>
    public Overview? Overview { get; set; }

    /// <inheritdoc/>
    public List<ComplexAvatarCollocation>? AvatarCollocations { get; set; }

    /// <inheritdoc/>
    public List<ComplexWeaponCollocation>? WeaponCollocations { get; set; }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeForDatabaseViewModelAsync()
    {
        if (isDatabaseViewModelInitialized)
        {
            return true;
        }

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

            return isDatabaseViewModelInitialized = true;
        }

        return false;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeForWikiAvatarViewModelAsync()
    {
        if (isWikiAvatarViewModelInitiaized)
        {
            return true;
        }

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

            AvatarCollocations = avatarCollocationsRaw.Select(co => new ComplexAvatarCollocation()
            {
                AvatarId = co.AvatarId,
                Avatars = co.Avatars.Select(a => new ComplexAvatar(idAvatarMap[a.Item], a.Rate)).ToList(),
                Weapons = co.Weapons.Select(w => new ComplexWeapon(idWeaponMap[w.Item], w.Rate)).ToList(),
                ReliquarySets = co.Reliquaries.Select(r => new ComplexReliquarySet(r, idReliquarySetMap)).ToList(),
            }).ToList();

            isWikiAvatarViewModelInitiaized = true;
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeForWikiWeaponViewModelAsync()
    {
        if (isWikiWeaponViewModelInitiaized)
        {
            return true;
        }

        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            Dictionary<AvatarId, Avatar> idAvatarMap = await GetIdAvatarMapExtendedAsync().ConfigureAwait(false);

            List<WeaponCollocation> weaponCollocationsRaw;
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                IHutaoService hutaoService = scope.ServiceProvider.GetRequiredService<IHutaoService>();
                weaponCollocationsRaw = await hutaoService.GetWeaponCollocationsAsync().ConfigureAwait(false);
            }

            WeaponCollocations = weaponCollocationsRaw.Select(co => new ComplexWeaponCollocation()
            {
                WeaponId = co.WeaponId,
                Avatars = co.Avatars.Select(a => new ComplexAvatar(idAvatarMap[a.Item], a.Rate)).ToList(),
            }).ToList();

            isWikiWeaponViewModelInitiaized = true;
            return true;
        }

        return false;
    }

    private async ValueTask<Dictionary<AvatarId, Avatar>> GetIdAvatarMapExtendedAsync()
    {
        if (idAvatarExtendedMap == null)
        {
            Dictionary<AvatarId, Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            idAvatarExtendedMap = AvatarIds.ExtendAvatars(idAvatarMap);
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

        AvatarAppearanceRanks = avatarAppearanceRanksRaw.OrderByDescending(r => r.Floor).Select(rank => new ComplexAvatarRank
        {
            Floor = $"第 {rank.Floor} 层",
            Avatars = rank.Ranks.OrderByDescending(r => r.Rate).Select(rank => new ComplexAvatar(idAvatarMap[rank.Item], rank.Rate)).ToList(),
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

        AvatarUsageRanks = avatarUsageRanksRaw.OrderByDescending(r => r.Floor).Select(rank => new ComplexAvatarRank
        {
            Floor = $"第 {rank.Floor} 层",
            Avatars = rank.Ranks.OrderByDescending(r => r.Rate).Select(rank => new ComplexAvatar(idAvatarMap[rank.Item], rank.Rate)).ToList(),
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
            return new ComplexAvatarConstellationInfo(idAvatarMap[info.AvatarId], info.HoldingRate, info.Constellations.Select(x => x.Rate));
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

        TeamAppearances = teamAppearancesRaw.OrderByDescending(t => t.Floor).Select(team => new ComplexTeamRank(team, idAvatarMap)).ToList();
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