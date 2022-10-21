// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Control;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Weapon;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Hutao.Model;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 胡桃数据库视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class HutaoDatabaseViewModel : ObservableObject, ISupportCancellation
{
    private readonly IHutaoService hutaoService;
    private readonly IMetadataService metadataService;

    private List<ComplexAvatarRank>? avatarUsageRanks;
    private List<ComplexAvatarRank>? avatarAppearanceRanks;
    private List<ComplexAvatarConstellationInfo>? avatarConstellationInfos;
    private List<ComplexTeamRank>? teamAppearances;

    /// <summary>
    /// 构造一个新的胡桃数据库视图模型
    /// </summary>
    /// <param name="hutaoService">胡桃服务</param>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public HutaoDatabaseViewModel(IHutaoService hutaoService, IMetadataService metadataService, IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.hutaoService = hutaoService;
        this.metadataService = metadataService;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 角色使用率
    /// </summary>
    public List<ComplexAvatarRank>? AvatarUsageRanks { get => avatarUsageRanks; set => SetProperty(ref avatarUsageRanks, value); }

    /// <summary>
    /// 角色上场率
    /// </summary>
    public List<ComplexAvatarRank>? AvatarAppearanceRanks { get => avatarAppearanceRanks; set => SetProperty(ref avatarAppearanceRanks, value); }

    /// <summary>
    /// 角色命座信息
    /// </summary>
    public List<ComplexAvatarConstellationInfo>? AvatarConstellationInfos { get => avatarConstellationInfos; set => avatarConstellationInfos = value; }

    /// <summary>
    /// 队伍出场
    /// </summary>
    public List<ComplexTeamRank>? TeamAppearances { get => teamAppearances; set => SetProperty(ref teamAppearances, value); }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    private async Task OpenUIAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            Dictionary<int, Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            idAvatarMap = new(idAvatarMap)
            {
                [10000005] = new() { Name = "旅行者", Icon = "UI_AvatarIcon_PlayerBoy", Quality = Model.Intrinsic.ItemQuality.QUALITY_ORANGE },
                [10000007] = new() { Name = "旅行者", Icon = "UI_AvatarIcon_PlayerGirl", Quality = Model.Intrinsic.ItemQuality.QUALITY_ORANGE },
            };

            Dictionary<int, Weapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync().ConfigureAwait(false);
            Dictionary<int, Model.Metadata.Reliquary.ReliquarySet> idReliquarySetMap = await metadataService.GetEquipAffixIdToReliquarySetMapAsync().ConfigureAwait(false);

            List<ComplexAvatarRank> avatarAppearanceRanksLocal = default!;
            List<ComplexAvatarRank> avatarUsageRanksLocal = default!;
            List<ComplexAvatarConstellationInfo> avatarConstellationInfosLocal = default!;
            List<ComplexTeamRank> teamAppearancesLocal = default!;

            Task avatarAppearanceRankTask = Task.Run(async () =>
            {
                // AvatarAppearanceRank
                List<AvatarAppearanceRank> avatarAppearanceRanksRaw = await hutaoService.GetAvatarAppearanceRanksAsync().ConfigureAwait(false);
                avatarAppearanceRanksLocal = avatarAppearanceRanksRaw.OrderByDescending(r => r.Floor).Select(rank => new ComplexAvatarRank
                {
                    Floor = $"第 {rank.Floor} 层",
                    Avatars = rank.Ranks.OrderByDescending(r => r.Rate).Select(rank => new ComplexAvatar(idAvatarMap[rank.Item], rank.Rate)).ToList(),
                }).ToList();
            });

            Task avatarUsageRank = Task.Run(async () =>
            {
                // AvatarUsageRank
                List<AvatarUsageRank> avatarUsageRanksRaw = await hutaoService.GetAvatarUsageRanksAsync().ConfigureAwait(false);
                avatarUsageRanksLocal = avatarUsageRanksRaw.OrderByDescending(r => r.Floor).Select(rank => new ComplexAvatarRank
                {
                    Floor = $"第 {rank.Floor} 层",
                    Avatars = rank.Ranks.OrderByDescending(r => r.Rate).Select(rank => new ComplexAvatar(idAvatarMap[rank.Item], rank.Rate)).ToList(),
                }).ToList();
            });

            Task avatarConstellationInfoTask = Task.Run(async () =>
            {
                // AvatarConstellationInfo
                List<AvatarConstellationInfo> avatarConstellationInfosRaw = await hutaoService.GetAvatarConstellationInfosAsync().ConfigureAwait(false);
                avatarConstellationInfosLocal = avatarConstellationInfosRaw.OrderBy(i => i.HoldingRate).Select(info =>
                {
                    return new ComplexAvatarConstellationInfo(idAvatarMap[info.AvatarId], info.HoldingRate, info.Constellations.Select(x => x.Rate));
                }).ToList();
            });

            Task teamAppearanceTask = Task.Run(async () =>
            {
                List<TeamAppearance> teamAppearancesRaw = await hutaoService.GetTeamAppearancesAsync().ConfigureAwait(false);
                teamAppearancesLocal = teamAppearancesRaw.OrderByDescending(t => t.Floor).Select(team => new ComplexTeamRank(team, idAvatarMap)).ToList();
            });

            await Task.WhenAll(avatarAppearanceRankTask, avatarUsageRank, avatarConstellationInfoTask, teamAppearanceTask).ConfigureAwait(false);

            await ThreadHelper.SwitchToMainThreadAsync();
            AvatarAppearanceRanks = avatarAppearanceRanksLocal;
            AvatarUsageRanks = avatarUsageRanksLocal;
            AvatarConstellationInfos = avatarConstellationInfosLocal;
            TeamAppearances = teamAppearancesLocal;

            //// AvatarCollocation
            //List<AvatarCollocation> avatarCollocationsRaw = await hutaoService.GetAvatarCollocationsAsync().ConfigureAwait(false);
            //List<ComplexAvatarCollocation> avatarCollocationsLocal = avatarCollocationsRaw.Select(co =>
            //{
            //    return new ComplexAvatarCollocation(idAvatarMap[co.AvatarId])
            //    {
            //        Avatars = co.Avatars.Select(a => new ComplexAvatar(idAvatarMap[a.Item], a.Rate)).ToList(),
            //        Weapons = co.Weapons.Select(w => new ComplexWeapon(idWeaponMap[w.Item], w.Rate)).ToList(),
            //        ReliquarySets = co.Reliquaries.Select(r => new ComplexReliquarySet(r, idReliquarySetMap)).ToList(),
            //    };
            //}).ToList();
        }
    }
}
