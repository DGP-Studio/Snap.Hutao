// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.UI.Xaml.View.Page;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Achievement;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class AchievementViewModelSlim : Abstraction.ViewModelSlim<AchievementPage>
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial AchievementViewModelSlim(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial ImmutableArray<AchievementStatistics> StatisticsList { get; set; } = [];

    protected override async Task LoadAsync()
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
            IMetadataService metadataService = scope.ServiceProvider.GetRequiredService<IMetadataService>();

            if (!await metadataService.InitializeAsync().ConfigureAwait(false))
            {
                return;
            }

            AchievementServiceMetadataContext context = await metadataService
                .GetContextAsync<AchievementServiceMetadataContext>()
                .ConfigureAwait(false);
            ImmutableArray<AchievementStatistics> array = await scope.ServiceProvider
                .GetRequiredService<IAchievementStatisticsService>()
                .GetAchievementStatisticsAsync(context)
                .ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            StatisticsList = array;
            IsInitialized = true;
        }
    }
}