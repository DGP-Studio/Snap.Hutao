// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.Achievement;
using System.Collections.Immutable;
using System.Data.Common;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IAchievementStatisticsService))]
internal sealed partial class AchievementStatisticsService : IAchievementStatisticsService
{
    private const int AchievementCardTakeCount = 2;

    private readonly IAchievementRepository achievementRepository;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async ValueTask<ImmutableArray<AchievementStatistics>> GetAchievementStatisticsAsync(AchievementServiceMetadataContext context, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();
        return SynchronizedGetAchievementStatistics(context);
    }

    private ImmutableArray<AchievementStatistics> SynchronizedGetAchievementStatistics(AchievementServiceMetadataContext context)
    {
        ImmutableArray<AchievementStatistics>.Builder results = ImmutableArray.CreateBuilder<AchievementStatistics>();
        ReadOnlySpan<AchievementArchive> span;
        try
        {
            span = achievementRepository.GetAchievementArchiveImmutableArray().AsSpan();
        }
        catch (DbException ex)
        {
            throw ExceptionHandlingSupport.KillProcessOnDbException(ex);
        }

        foreach (ref readonly AchievementArchive archive in span)
        {
            int finishedCount = achievementRepository.GetFinishedAchievementCountByArchiveId(archive.InnerId);
            int totalCount = context.IdAchievementMap.Count;
            ImmutableArray<EntityAchievement> achievements = achievementRepository.GetLatestFinishedAchievementImmutableArrayByArchiveId(archive.InnerId, AchievementCardTakeCount);

            results.Add(new()
            {
                DisplayName = archive.Name,
                FinishDescription = AchievementStatistics.Format(finishedCount, totalCount, out _),
                Achievements = achievements.SelectAsArray(static (entity, context) => AchievementView.Create(entity, context.IdAchievementMap[entity.Id]), context),
            });
        }

        return results.ToImmutable();
    }
}