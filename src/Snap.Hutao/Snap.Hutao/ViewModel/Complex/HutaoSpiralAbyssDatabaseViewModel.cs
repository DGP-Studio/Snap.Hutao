// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.ViewModel.Complex;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class HutaoSpiralAbyssDatabaseViewModel : Abstraction.ViewModel
{
    private readonly IHutaoSpiralAbyssStatisticsCache hutaoCache;
    private readonly ITaskContext taskContext;

    public List<AvatarRankView>? AvatarUsageRanks { get; set => SetProperty(ref field, value); }

    public AvatarRankView? SelectedAvatarUsageRank { get; set => SetProperty(ref field, value); }

    public List<AvatarRankView>? AvatarAppearanceRanks { get; set => SetProperty(ref field, value); }

    public AvatarRankView? SelectedAvatarAppearanceRank { get; set => SetProperty(ref field, value); }

    public List<AvatarConstellationInfoView>? AvatarConstellationInfos { get; set => SetProperty(ref field, value); }

    public List<TeamAppearanceView>? TeamAppearances { get; set => SetProperty(ref field, value); }

    public TeamAppearanceView? SelectedTeamAppearance { get; set => SetProperty(ref field, value); }

    public Overview? Overview { get; set => SetProperty(ref field, value); }

    protected override async Task LoadAsync()
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
