// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 简化的祈愿记录视图模型
/// </summary>
[Injection(InjectAs.Transient)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class GachaLogViewModelSlim : Abstraction.ViewModelSlim<View.Page.GachaLogPage>
{
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    private List<GachaStatisticsSlim>? statisticsList;

    /// <summary>
    /// 统计列表
    /// </summary>
    public List<GachaStatisticsSlim>? StatisticsList { get => statisticsList; set => SetProperty(ref statisticsList, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
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