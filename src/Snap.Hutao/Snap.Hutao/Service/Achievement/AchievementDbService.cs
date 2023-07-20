// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Primitive;
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

    public async Task<int> GetFinishedAchievementCountByArchiveIdAsync(Guid archiveId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.Achievements
                .Where(a => a.ArchiveId == archiveId)
                .Where(a => a.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
                .CountAsync()
                .ConfigureAwait(false);
        }
    }

    public async Task<List<EntityAchievement>> GetLatestFinishedAchievementListByArchiveIdAsync(Guid archiveId, int take)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.Achievements
                .Where(a => a.ArchiveId == archiveId)
                .Where(a => a.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
                .OrderByDescending(a => a.Time.ToString())
                .Take(take)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}