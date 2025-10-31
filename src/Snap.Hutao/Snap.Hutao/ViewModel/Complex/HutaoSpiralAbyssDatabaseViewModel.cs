// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Web.Hutao.SpiralAbyss;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Complex;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class HutaoSpiralAbyssDatabaseViewModel : Abstraction.ViewModel
{
    private readonly IHutaoSpiralAbyssStatisticsCache hutaoCache;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial HutaoSpiralAbyssDatabaseViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial ImmutableArray<AvatarRankView> AvatarUsageRanks { get; set; }

    [ObservableProperty]
    public partial AvatarRankView? SelectedAvatarUsageRank { get; set; }

    [ObservableProperty]
    public partial ImmutableArray<AvatarRankView> AvatarAppearanceRanks { get; set; }

    [ObservableProperty]
    public partial AvatarRankView? SelectedAvatarAppearanceRank { get; set; }

    [ObservableProperty]
    public partial ImmutableArray<AvatarConstellationInfoView> AvatarConstellationInfos { get; set; }

    [ObservableProperty]
    public partial ImmutableArray<TeamAppearanceView> TeamAppearances { get; set; }

    [ObservableProperty]
    public partial TeamAppearanceView? SelectedTeamAppearance { get; set; }

    [ObservableProperty]
    public partial Overview? Overview { get; set; }

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