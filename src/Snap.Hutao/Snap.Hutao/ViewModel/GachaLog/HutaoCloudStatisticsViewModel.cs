// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Service.GachaLog;

namespace Snap.Hutao.ViewModel.GachaLog;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class HutaoCloudStatisticsViewModel : Abstraction.ViewModelSlim
{
    private readonly IGachaLogHutaoCloudService hutaoCloudService;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial HutaoCloudStatisticsViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial HutaoStatistics? Statistics { get; set; }

    protected override async Task LoadAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        try
        {
            (bool isOk, HutaoStatistics statistics) = await hutaoCloudService.GetCurrentEventStatisticsAsync().ConfigureAwait(false);

            if (isOk)
            {
                await taskContext.SwitchToMainThreadAsync();
                Statistics = statistics;
                IsInitialized = true;
            }
        }
        catch (ObjectDisposedException)
        {
            // Ignore
            // Parent view model has been disposed
        }
    }
}