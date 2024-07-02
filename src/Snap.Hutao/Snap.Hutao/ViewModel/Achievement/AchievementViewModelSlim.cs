// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.View.Page;

namespace Snap.Hutao.ViewModel.Achievement;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Transient)]
internal sealed partial class AchievementViewModelSlim : Abstraction.ViewModelSlim<AchievementPage>
{
    private List<AchievementStatistics>? statisticsList;

    public List<AchievementStatistics>? StatisticsList { get => statisticsList; set => SetProperty(ref statisticsList, value); }

    protected override async Task LoadAsync()
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
            IMetadataService metadataService = scope.ServiceProvider.GetRequiredService<IMetadataService>();

            if (await metadataService.InitializeAsync().ConfigureAwait(false))
            {
                AchievementServiceMetadataContext context = await metadataService
                    .GetContextAsync<AchievementServiceMetadataContext>()
                    .ConfigureAwait(false);
                List<AchievementStatistics> list = await scope.ServiceProvider
                    .GetRequiredService<IAchievementStatisticsService>()
                    .GetAchievementStatisticsAsync(context)
                    .ConfigureAwait(false);

                await taskContext.SwitchToMainThreadAsync();
                StatisticsList = list;
                IsInitialized = true;
            }
        }
    }
}