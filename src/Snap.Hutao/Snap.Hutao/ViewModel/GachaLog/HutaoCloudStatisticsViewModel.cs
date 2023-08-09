// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Hutao;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 胡桃云服务统计视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
[ConstructorGenerated(CallBaseConstructor = true)]
internal sealed partial class HutaoCloudStatisticsViewModel : Abstraction.ViewModelSlim
{
    private readonly IGachaLogHutaoCloudService hutaoCloudService;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly ITaskContext taskContext;

    private HutaoStatistics? statistics;

    public HutaoUserOptions Options { get => hutaoUserOptions; }

    public HutaoStatistics? Statistics { get => statistics; set => SetProperty(ref statistics, value); }

    protected override async Task OpenUIAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool isOk, HutaoStatistics statistics) = await hutaoCloudService.GetCurrentEventStatisticsAsync().ConfigureAwait(false);

        if (isOk)
        {
            await taskContext.SwitchToMainThreadAsync();
            Statistics = statistics;
            IsInitialized = true;
        }
    }
}