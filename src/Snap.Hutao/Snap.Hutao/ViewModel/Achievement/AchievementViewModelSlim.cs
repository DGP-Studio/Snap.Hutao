// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Metadata;

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 简化的成就视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal sealed class AchievementViewModelSlim : Abstraction.ViewModelSlim<View.Page.AchievementPage>
{
    private List<AchievementStatistics>? statisticsList;

    /// <summary>
    /// 构造一个新的简化的成就视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public AchievementViewModelSlim(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    /// <summary>
    /// 统计列表
    /// </summary>
    public List<AchievementStatistics>? StatisticsList { get => statisticsList; set => SetProperty(ref statisticsList, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
            IMetadataService metadataService = scope.ServiceProvider.GetRequiredService<IMetadataService>();

            if (await metadataService.InitializeAsync().ConfigureAwait(false))
            {
                Dictionary<AchievementId, Model.Metadata.Achievement.Achievement> achievementMap = await metadataService
                    .GetIdToAchievementMapAsync()
                    .ConfigureAwait(false);
                List<AchievementStatistics> list = await scope.ServiceProvider
                    .GetRequiredService<IAchievementService>()
                    .GetAchievementStatisticsAsync(achievementMap)
                    .ConfigureAwait(false);

                await taskContext.SwitchToMainThreadAsync();
                StatisticsList = list;
                IsInitialized = true;
            }
        }
    }
}