﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Hutao;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.ViewModel.Complex;

/// <summary>
/// 胡桃数据库视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class HutaoDatabaseViewModel : Abstraction.ViewModel
{
    private readonly IHutaoSpiralAbyssStatisticsCache hutaoCache;
    private readonly ITaskContext taskContext;

    private List<AvatarRankView>? avatarUsageRanks;
    private List<AvatarRankView>? avatarAppearanceRanks;
    private List<AvatarConstellationInfoView>? avatarConstellationInfos;
    private List<TeamAppearanceView>? teamAppearances;
    private Overview? overview;
    private AvatarRankView? selectedAvatarUsageRank;
    private AvatarRankView? selectedAvatarAppearanceRank;
    private TeamAppearanceView? selectedTeamAppearance;

    /// <summary>
    /// 角色使用率
    /// </summary>
    public List<AvatarRankView>? AvatarUsageRanks { get => avatarUsageRanks; set => SetProperty(ref avatarUsageRanks, value); }

    public AvatarRankView? SelectedAvatarUsageRank { get => selectedAvatarUsageRank; set => SetProperty(ref selectedAvatarUsageRank, value); }

    /// <summary>
    /// 角色上场率
    /// </summary>
    public List<AvatarRankView>? AvatarAppearanceRanks { get => avatarAppearanceRanks; set => SetProperty(ref avatarAppearanceRanks, value); }

    public AvatarRankView? SelectedAvatarAppearanceRank { get => selectedAvatarAppearanceRank; set => SetProperty(ref selectedAvatarAppearanceRank, value); }

    /// <summary>
    /// 角色命座信息
    /// </summary>
    public List<AvatarConstellationInfoView>? AvatarConstellationInfos { get => avatarConstellationInfos; set => SetProperty(ref avatarConstellationInfos, value); }

    /// <summary>
    /// 队伍出场
    /// </summary>
    public List<TeamAppearanceView>? TeamAppearances { get => teamAppearances; set => SetProperty(ref teamAppearances, value); }

    public TeamAppearanceView? SelectedTeamAppearance { get => selectedTeamAppearance; set => SetProperty(ref selectedTeamAppearance, value); }

    /// <summary>
    /// 总览数据
    /// </summary>
    public Overview? Overview { get => overview; set => SetProperty(ref overview, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        if (await hutaoCache.InitializeForSpiralAbyssViewAsync().ConfigureAwait(false))
        {
            await taskContext.SwitchToMainThreadAsync();
            AvatarAppearanceRanks = hutaoCache.AvatarAppearanceRanks;
            SelectedAvatarAppearanceRank = AvatarAppearanceRanks?.FirstOrDefault();

            AvatarUsageRanks = hutaoCache.AvatarUsageRanks;
            SelectedAvatarUsageRank = AvatarUsageRanks?.FirstOrDefault();

            TeamAppearances = hutaoCache.TeamAppearances;
            SelectedTeamAppearance = TeamAppearances?.FirstOrDefault();

            AvatarConstellationInfos = hutaoCache.AvatarConstellationInfos;
            Overview = hutaoCache.Overview;
        }
    }
}
