// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Request.Builder;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 成就数据库服务
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IAchievementDbService))]
internal sealed partial class AchievementDbService : IAchievementDbService
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public Dictionary<AchievementId, EntityAchievement> GetAchievementMapByArchiveId(Guid archiveId)
    {
        try
        {
            return this.Query<EntityAchievement, Dictionary<AchievementId, EntityAchievement>>(query => query
                .Where(a => a.ArchiveId == archiveId)
                .ToDictionary(a => (AchievementId)a.Id));
        }
        catch (ArgumentException ex)
        {
            throw HutaoException.Throw(HutaoExceptionKind.DatabaseCorrupted, SH.ServiceAchievementUserdataCorruptedInnerIdNotUnique, ex);
        }
    }

    public async ValueTask<int> GetFinishedAchievementCountByArchiveIdAsync(Guid archiveId)
    {
        return await this.QueryAsync<EntityAchievement, int>(query => query
                .Where(a => a.ArchiveId == archiveId)
                .Where(a => a.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
                .CountAsync())
            .ConfigureAwait(false);
    }

    [SuppressMessage("", "CA1305")]
    public async ValueTask<List<EntityAchievement>> GetLatestFinishedAchievementListByArchiveIdAsync(Guid archiveId, int take)
    {
        return await this.QueryAsync<EntityAchievement, List<EntityAchievement>>(query => query
                .Where(a => a.ArchiveId == archiveId)
                .Where(a => a.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
                .OrderByDescending(a => a.Time.ToString())
                .Take(take)
                .ToListAsync())
            .ConfigureAwait(false);
    }

    public void OverwriteAchievement(EntityAchievement achievement)
    {
        this.DeleteByInnerId(achievement);
        if (achievement.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
        {
            this.Add(achievement);
        }
    }

    public async ValueTask OverwriteAchievementAsync(EntityAchievement achievement)
    {
        await this.DeleteByInnerIdAsync(achievement).ConfigureAwait(false);
        if (achievement.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
        {
            await this.AddAsync(achievement).ConfigureAwait(false);
        }
    }

    public ObservableCollection<AchievementArchive> GetAchievementArchiveCollection()
    {
        return this.Query<AchievementArchive, ObservableCollection<AchievementArchive>>(query => query.ToObservableCollection());
    }

    public async ValueTask RemoveAchievementArchiveAsync(AchievementArchive archive)
    {
        // It will cascade deleted the achievements.
        await this.DeleteAsync(archive).ConfigureAwait(false);
    }

    public List<EntityAchievement> GetAchievementListByArchiveId(Guid archiveId)
    {
        return this.Query<EntityAchievement, List<EntityAchievement>>(query => [.. query.Where(a => a.ArchiveId == archiveId)]);
    }

    public async ValueTask<List<EntityAchievement>> GetAchievementListByArchiveIdAsync(Guid archiveId)
    {
        return await this.QueryAsync<EntityAchievement, List<EntityAchievement>>(query => query
                .Where(a => a.ArchiveId == archiveId)
                .ToListAsync())
            .ConfigureAwait(false);
    }

    public List<AchievementArchive> GetAchievementArchiveList()
    {
        return this.Query<AchievementArchive, List<AchievementArchive>>(query => [.. query]);
    }

    public async ValueTask<List<AchievementArchive>> GetAchievementArchiveListAsync()
    {
        return await this.QueryAsync<AchievementArchive, List<AchievementArchive>>(query => query.ToListAsync()).ConfigureAwait(false);
    }
}