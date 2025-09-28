// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Complex;

[ConstructorGenerated]
[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class HutaoSpiralAbyssDatabaseViewModel : Abstraction.ViewModel
{
    private readonly IHutaoSpiralAbyssStatisticsCache hutaoCache;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    public ImmutableArray<AvatarRankView> AvatarUsageRanks { get; set => SetProperty(ref field, value); }

    public AvatarRankView? SelectedAvatarUsageRank { get; set => SetProperty(ref field, value); }

    public ImmutableArray<AvatarRankView> AvatarAppearanceRanks { get; set => SetProperty(ref field, value); }

    public AvatarRankView? SelectedAvatarAppearanceRank { get; set => SetProperty(ref field, value); }

    public ImmutableArray<AvatarConstellationInfoView> AvatarConstellationInfos { get; set => SetProperty(ref field, value); }

    public ImmutableArray<TeamAppearanceView> TeamAppearances { get; set => SetProperty(ref field, value); }

    public TeamAppearanceView? SelectedTeamAppearance { get; set => SetProperty(ref field, value); }

    public Overview? Overview { get; set => SetProperty(ref field, value); }

    protected override async Task LoadAsync()
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return;
        }

        HutaoSpiralAbyssStatisticsMetadataContext context = await metadataService.GetContextAsync<HutaoSpiralAbyssStatisticsMetadataContext>().ConfigureAwait(false);
        await hutaoCache.InitializeForSpiralAbyssViewAsync(context).ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();
        AvatarAppearanceRanks = hutaoCache.AvatarAppearanceRanks;
        SelectedAvatarAppearanceRank = AvatarAppearanceRanks.FirstOrDefault();

        AvatarUsageRanks = hutaoCache.AvatarUsageRanks;
        SelectedAvatarUsageRank = AvatarUsageRanks.FirstOrDefault();

        TeamAppearances = hutaoCache.TeamAppearances;
        SelectedTeamAppearance = TeamAppearances.FirstOrDefault();

        AvatarConstellationInfos = hutaoCache.AvatarConstellationInfos;
        Overview = hutaoCache.Overview;
    }
}