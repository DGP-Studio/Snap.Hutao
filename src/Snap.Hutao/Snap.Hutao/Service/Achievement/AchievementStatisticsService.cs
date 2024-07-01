// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.Achievement;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IAchievementStatisticsService))]
internal sealed partial class AchievementStatisticsService : IAchievementStatisticsService
{
    private const int AchievementCardTakeCount = 2;

    private readonly IAchievementDbService achievementDbService;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async ValueTask<List<AchievementStatistics>> GetAchievementStatisticsAsync(AchievementServiceMetadataContext context, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();

        List<AchievementStatistics> results = [];
        foreach (AchievementArchive archive in achievementDbService.GetAchievementArchiveList())
        {
            int finishedCount = achievementDbService.GetFinishedAchievementCountByArchiveId(archive.InnerId);
            int totalCount = context.IdAchievementMap.Count;
            List<EntityAchievement> achievements = achievementDbService.GetLatestFinishedAchievementListByArchiveId(archive.InnerId, AchievementCardTakeCount);

            results.Add(new()
            {
                DisplayName = archive.Name,
                FinishDescription = AchievementStatistics.Format(finishedCount, totalCount, out _),
                Achievements = achievements.SelectList(entity => new AchievementView(entity, context.IdAchievementMap[entity.Id])),
            });
        }

        return results;
    }
}