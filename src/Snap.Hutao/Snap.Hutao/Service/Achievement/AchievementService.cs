// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Achievement;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 成就服务
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped, typeof(IAchievementService))]
internal sealed partial class AchievementService : IAchievementService
{
    private readonly ScopedDbCurrent<AchievementArchive, Message.AchievementArchiveChangedMessage> dbCurrent;
    private readonly AchievementDbBulkOperation achievementDbBulkOperation;
    private readonly ILogger<AchievementService> logger;
    private readonly ITaskContext taskContext;
    private readonly IServiceProvider serviceProvider;

    private ObservableCollection<AchievementArchive>? archiveCollection;

    /// <summary>
    /// 构造一个新的成就服务
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public AchievementService(IServiceProvider serviceProvider)
    {
        achievementDbBulkOperation = serviceProvider.GetRequiredService<AchievementDbBulkOperation>();
        logger = serviceProvider.GetRequiredService<ILogger<AchievementService>>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        this.serviceProvider = serviceProvider;

        dbCurrent = new(serviceProvider);
    }

    /// <inheritdoc/>
    public List<AchievementView> GetAchievements(AchievementArchive archive, List<MetadataAchievement> metadata)
    {
        Dictionary<int, EntityAchievement> entityMap;
        try
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                entityMap = appDbContext.Achievements
                    .Where(a => a.ArchiveId == archive.InnerId)
                    .AsEnumerable()
                    .ToDictionary(a => a.Id);
            }
        }
        catch (ArgumentException ex)
        {
            throw ThrowHelper.UserdataCorrupted(SH.ServiceAchievementUserdataCorruptedInnerIdNotUnique, ex);
        }

        return metadata.SelectList(meta =>
        {
            EntityAchievement? entity = entityMap.GetValueOrDefault(meta.Id) ?? EntityAchievement.Create(archive.InnerId, meta.Id);
            return new AchievementView(entity, meta);
        });
    }

    /// <inheritdoc/>
    public async Task<List<AchievementStatistics>> GetAchievementStatisticsAsync(Dictionary<AchievementId, MetadataAchievement> achievementMap)
    {
        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            List<AchievementStatistics> results = new();
            foreach (AchievementArchive archive in appDbContext.AchievementArchives)
            {
                int finished = await appDbContext.Achievements
                    .Where(a => a.ArchiveId == archive.InnerId)

                    // We already filtered out incompleted achievements when save them.
                    // .Where(a => (int)a.Status >= (int)Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
                    .CountAsync()
                    .ConfigureAwait(false);
                int totalCount = achievementMap.Count;

                List<EntityAchievement> achievements = await appDbContext.Achievements
                    .Where(a => a.ArchiveId == archive.InnerId)
                    .OrderByDescending(a => a.Time.ToString())
                    .Take(2)
                    .ToListAsync()
                    .ConfigureAwait(false);

                results.Add(new()
                {
                    DisplayName = archive.Name,
                    FinishDescription = AchievementStatistics.Format(finished, totalCount, out _),
                    Achievements = achievements.SelectList(entity => new AchievementView(entity, achievementMap[entity.Id])),
                });
            }

            return results;
        }
    }

    /// <inheritdoc/>
    public void SaveAchievements(AchievementArchive archive, List<AchievementView> achievements)
    {
        using (ValueStopwatch.MeasureExecution(logger))
        {
            IEnumerable<EntityAchievement> data = achievements
                .Where(a => a.IsChecked)
                .Select(a => a.Entity)
                .OrderBy(a => a.Id); // Overwrite operation requires ordered achievements.
            ImportResult result = achievementDbBulkOperation.Overwrite(archive.InnerId, data);

            logger.LogInformation("SaveAchievements: {result}", result);
        }
    }

    /// <inheritdoc/>
    public void SaveAchievement(AchievementView achievement)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Delete exists one.
            appDbContext.Achievements.ExecuteDeleteWhere(e => e.InnerId == achievement.Entity.InnerId);
            if (achievement.IsChecked)
            {
                appDbContext.Achievements.AddAndSave(achievement.Entity);
            }
        }
    }
}