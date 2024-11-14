// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.View.Page;

namespace Snap.Hutao.ViewModel.GachaLog;

[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class GachaLogViewModelSlim : Abstraction.ViewModelSlim<GachaLogPage>
{
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    public List<GachaStatisticsSlim>? StatisticsList { get; set => SetProperty(ref field, value); }

    protected override async Task LoadAsync()
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            IGachaLogService gachaLogService = scope.ServiceProvider.GetRequiredService<IGachaLogService>();

            if (await gachaLogService.InitializeAsync().ConfigureAwait(false))
            {
                try
                {
                    List<GachaStatisticsSlim> list = await gachaLogService.GetStatisticsSlimListAsync().ConfigureAwait(false);

                    await taskContext.SwitchToMainThreadAsync();
                    StatisticsList = list;
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