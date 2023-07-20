// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Achievement;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 成就服务
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IAchievementService))]
internal sealed partial class AchievementService : IAchievementService
{
    private readonly ScopedDbCurrent<AchievementArchive, Message.AchievementArchiveChangedMessage> dbCurrent;
    private readonly AchievementDbBulkOperation achievementDbBulkOperation;
    private readonly IAchievementDbService achievementDbService;
    private readonly ILogger<AchievementService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public List<AchievementView> GetAchievements(AchievementArchive archive, List<MetadataAchievement> metadata)
    {
        Dictionary<AchievementId, EntityAchievement> entities = achievementDbService.GetAchievementMapByArchiveId(archive.InnerId);

        return metadata.SelectList(meta =>
        {
            EntityAchievement entity = entities.GetValueOrDefault(meta.Id) ?? EntityAchievement.From(archive.InnerId, meta.Id);
            return new AchievementView(entity, meta);
        });
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