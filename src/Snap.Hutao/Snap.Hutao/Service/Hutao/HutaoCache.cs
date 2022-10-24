// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Hutao.Model;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃 API 缓存
/// </summary>
[Injection(InjectAs.Singleton, typeof(IHtaoCache))]
internal class HutaoCache : IHtaoCache
{
    private readonly IHutaoService hutaoService;
    private readonly IMetadataService metadataService;

    private Dictionary<int, Avatar>? idAvatarExtendedMap;

    /// <summary>
    /// 构造一个新的胡桃 API 缓存
    /// </summary>
    /// <param name="hutaoService">胡桃服务</param>
    /// <param name="metadataService">元数据服务</param>
    public HutaoCache(IHutaoService hutaoService, IMetadataService metadataService)
    {
        this.hutaoService = hutaoService;
        this.metadataService = metadataService;
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
    public async ValueTask<bool> InitializeForDatabaseViewModelAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            Dictionary<int, Avatar> idAvatarMap = await GetIdAvatarMapExtendedAsync().ConfigureAwait(false);

            Task avatarAppearanceRankTask = AvatarAppearanceRankAsync(idAvatarMap);
            Task avatarUsageRank = AvatarUsageRanksAsync(idAvatarMap);
            Task avatarConstellationInfoTask = AvatarConstellationInfosAsync(idAvatarMap);
            Task teamAppearanceTask = TeamAppearancesAsync(idAvatarMap);
            Task ovewviewTask = OverviewAsync();

            await Task.WhenAll(
                avatarAppearanceRankTask,
                avatarUsageRank,
                avatarConstellationInfoTask,
                teamAppearanceTask,
                ovewviewTask)
                .ConfigureAwait(false);

            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeForWikiAvatarViewModelAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            Dictionary<int, Avatar> idAvatarMap = await GetIdAvatarMapExtendedAsync().ConfigureAwait(false);
            Dictionary<int, Weapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync().ConfigureAwait(false);
            Dictionary<int, Model.Metadata.Reliquary.ReliquarySet> idReliquarySetMap = await metadataService.GetEquipAffixIdToReliquarySetMapAsync().ConfigureAwait(false);

            // AvatarCollocation
            List<AvatarCollocation> avatarCollocationsRaw = await hutaoService.GetAvatarCollocationsAsync().ConfigureAwait(false);
            AvatarCollocations = avatarCollocationsRaw.Select(co =>
            {
                return new ComplexAvatarCollocation(idAvatarMap[co.AvatarId])
                {
                    Avatars = co.Avatars.Select(a => new ComplexAvatar(idAvatarMap[a.Item], a.Rate)).ToList(),
                    Weapons = co.Weapons.Select(w => new ComplexWeapon(idWeaponMap[w.Item], w.Rate)).ToList(),
                    ReliquarySets = co.Reliquaries.Select(r => new ComplexReliquarySet(r, idReliquarySetMap)).ToList(),
                };
            }).ToList();

            return true;
        }

        return false;
    }

    private async ValueTask<Dictionary<int, Avatar>> GetIdAvatarMapExtendedAsync()
    {
        if (idAvatarExtendedMap == null)
        {
            Dictionary<int, Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            idAvatarExtendedMap = new(idAvatarMap)
            {
                [10000005] = new() { Name = "旅行者", Icon = "UI_AvatarIcon_PlayerBoy", Quality = Model.Intrinsic.ItemQuality.QUALITY_ORANGE },
                [10000007] = new() { Name = "旅行者", Icon = "UI_AvatarIcon_PlayerGirl", Quality = Model.Intrinsic.ItemQuality.QUALITY_ORANGE },
            };
        }

        return idAvatarExtendedMap;
    }

    private async Task AvatarAppearanceRankAsync(Dictionary<int, Avatar> idAvatarMap)
    {
        List<AvatarAppearanceRank> avatarAppearanceRanksRaw = await hutaoService.GetAvatarAppearanceRanksAsync().ConfigureAwait(false);
        AvatarAppearanceRanks = avatarAppearanceRanksRaw.OrderByDescending(r => r.Floor).Select(rank => new ComplexAvatarRank
        {
            Floor = $"第 {rank.Floor} 层",
            Avatars = rank.Ranks.OrderByDescending(r => r.Rate).Select(rank => new ComplexAvatar(idAvatarMap[rank.Item], rank.Rate)).ToList(),
        }).ToList();
    }

    private async Task AvatarUsageRanksAsync(Dictionary<int, Avatar> idAvatarMap)
    {
        List<AvatarUsageRank> avatarUsageRanksRaw = await hutaoService.GetAvatarUsageRanksAsync().ConfigureAwait(false);
        AvatarUsageRanks = avatarUsageRanksRaw.OrderByDescending(r => r.Floor).Select(rank => new ComplexAvatarRank
        {
            Floor = $"第 {rank.Floor} 层",
            Avatars = rank.Ranks.OrderByDescending(r => r.Rate).Select(rank => new ComplexAvatar(idAvatarMap[rank.Item], rank.Rate)).ToList(),
        }).ToList();
    }

    private async Task AvatarConstellationInfosAsync(Dictionary<int, Avatar> idAvatarMap)
    {
        List<AvatarConstellationInfo> avatarConstellationInfosRaw = await hutaoService.GetAvatarConstellationInfosAsync().ConfigureAwait(false);
        AvatarConstellationInfos = avatarConstellationInfosRaw.OrderBy(i => i.HoldingRate).Select(info =>
        {
            return new ComplexAvatarConstellationInfo(idAvatarMap[info.AvatarId], info.HoldingRate, info.Constellations.Select(x => x.Rate));
        }).ToList();
    }

    private async Task TeamAppearancesAsync(Dictionary<int, Avatar> idAvatarMap)
    {
        List<TeamAppearance> teamAppearancesRaw = await hutaoService.GetTeamAppearancesAsync().ConfigureAwait(false);
        TeamAppearances = teamAppearancesRaw.OrderByDescending(t => t.Floor).Select(team => new ComplexTeamRank(team, idAvatarMap)).ToList();
    }

    private async Task OverviewAsync()
    {
        Overview = await hutaoService.GetOverviewAsync().ConfigureAwait(false);
    }
}
