// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Primitive;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 成就数据库服务
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IAchievementDbService))]
internal sealed partial class AchievementDbService : IAchievementDbService
{
    private readonly IServiceProvider serviceProvider;

    public Dictionary<AchievementId, EntityAchievement> GetAchievementMapByArchiveId(Guid archiveId)
    {
        Dictionary<AchievementId, EntityAchievement> entities;
        try
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                entities = appDbContext.Achievements
                    .AsNoTracking()
                    .Where(a => a.ArchiveId == archiveId)
                    .ToDictionary(a => (AchievementId)a.Id);
            }
        }
        catch (ArgumentException ex)
        {
            throw ThrowHelper.DatabaseCorrupted(SH.ServiceAchievementUserdataCorruptedInnerIdNotUnique, ex);
        }

        return entities;
    }

    public async ValueTask<int> GetFinishedAchievementCountByArchiveIdAsync(Guid archiveId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.Achievements
                .AsNoTracking()
                .Where(a => a.ArchiveId == archiveId)
                .Where(a => a.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
                .CountAsync()
                .ConfigureAwait(false);
        }
    }

    public async ValueTask<List<EntityAchievement>> GetLatestFinishedAchievementListByArchiveIdAsync(Guid archiveId, int take)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.Achievements
                .AsNoTracking()
                .Where(a => a.ArchiveId == archiveId)
                .Where(a => a.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
                .OrderByDescending(a => a.Time.ToString())
                .Take(take)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }

    public void OverwriteAchievement(EntityAchievement achievement)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Delete exists one.
            appDbContext.Achievements.ExecuteDeleteWhere(e => e.InnerId == achievement.InnerId);
            if (achievement.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
            {
                appDbContext.Achievements.AddAndSave(achievement);
            }
        }
    }

    public ObservableCollection<AchievementArchive> GetAchievementArchiveCollection()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return appDbContext.AchievementArchives.AsNoTracking().ToObservableCollection();
        }
    }

    public async ValueTask DeleteAchievementArchiveAsync(AchievementArchive archive)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // It will cascade deleted the achievements.
            await appDbContext.AchievementArchives.RemoveAndSaveAsync(archive).ConfigureAwait(false);
        }
    }

    public List<EntityAchievement> GetAchievementListByArchiveId(Guid archiveId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return appDbContext.Achievements
                .AsNoTracking()
                .Where(i => i.ArchiveId == archiveId)
                .ToList();
        }
    }

    public List<AchievementArchive> GetAchievementArchiveList()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return appDbContext.AchievementArchives.AsNoTracking().ToList();
        }
    }
}