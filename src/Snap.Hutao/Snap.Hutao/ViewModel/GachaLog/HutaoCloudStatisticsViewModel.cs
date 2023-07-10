// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Hutao;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 胡桃云服务统计视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal sealed class HutaoCloudStatisticsViewModel : Abstraction.ViewModelSlim
{
    private HutaoStatistics? statistics;

    /// <summary>
    /// 构造一个新的胡桃云服务统计视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public HutaoCloudStatisticsViewModel(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        Options = serviceProvider.GetRequiredService<HutaoUserOptions>();
    }

    public HutaoUserOptions Options { get; }

    public HutaoStatistics? Statistics { get => statistics; set => SetProperty(ref statistics, value); }

    protected override async Task OpenUIAsync()
    {
        IHutaoCloudService hutaoCloudService = ServiceProvider.GetRequiredService<IHutaoCloudService>();
        (bool isOk, HutaoStatistics statistics) = await hutaoCloudService.GetCurrentEventStatisticsAsync().ConfigureAwait(false);
        if (isOk)
        {
            await ServiceProvider.GetRequiredService<ITaskContext>().SwitchToMainThreadAsync();
            Statistics = statistics;
        }
    }
}