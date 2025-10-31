// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Page;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.GachaLog;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class GachaLogViewModelSlim : Abstraction.ViewModelSlim<GachaLogPage>
{
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial GachaLogViewModelSlim(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial ImmutableArray<GachaStatisticsSlim> StatisticsList { get; set; } = [];

    protected override async Task LoadAsync()
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            IGachaLogService gachaLogService = scope.ServiceProvider.GetRequiredService<IGachaLogService>();

            if (!await metadataService.InitializeAsync().ConfigureAwait(false))
            {
                return;
            }

            try
            {
                GachaLogServiceMetadataContext context = await metadataService.GetContextAsync<GachaLogServiceMetadataContext>().ConfigureAwait(false);
                ImmutableArray<GachaStatisticsSlim> array = await gachaLogService.GetStatisticsSlimImmutableArrayAsync(context).ConfigureAwait(false);

                await taskContext.SwitchToMainThreadAsync();
                StatisticsList = array;
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                messenger.Send(InfoBarMessage.Error(ex));
            }
        }
    }
}