// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Page;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.GachaLog;

[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class GachaLogViewModelSlim : Abstraction.ViewModelSlim<GachaLogPage>
{
    private readonly IMetadataService metadataService;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    public IReadOnlyList<GachaStatisticsSlim> StatisticsList { get; set => SetProperty(ref field, value); } = [];

    protected override async Task LoadAsync()
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            IGachaLogService gachaLogService = scope.ServiceProvider.GetRequiredService<IGachaLogService>();

            if (await metadataService.InitializeAsync().ConfigureAwait(false))
            {
                try
                {
                    GachaLogServiceMetadataContext context = await metadataService.GetContextAsync<GachaLogServiceMetadataContext>();
                    ImmutableArray<GachaStatisticsSlim> array = await gachaLogService.GetStatisticsSlimImmutableArrayAsync(context).ConfigureAwait(false);

                    await taskContext.SwitchToMainThreadAsync();
                    StatisticsList = array;
                    IsInitialized = true;
                }
                catch (Exception ex)
                {
                    infoBarService.Error(ex);
                }
            }
        }
    }
}