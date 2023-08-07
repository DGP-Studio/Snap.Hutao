// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Metadata;

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 简化的成就视图模型
/// </summary>
[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Transient)]
internal sealed partial class AchievementViewModelSlim : Abstraction.ViewModelSlim<View.Page.AchievementPage>
{
    private List<AchievementStatistics>? statisticsList;

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
                    .GetRequiredService<IAchievementStatisticsService>()
                    .GetAchievementStatisticsAsync(achievementMap)
                    .ConfigureAwait(false);

                await taskContext.SwitchToMainThreadAsync();
                StatisticsList = list;
                IsInitialized = true;
            }
        }
    }
}