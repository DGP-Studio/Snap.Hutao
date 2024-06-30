// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

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
            throw HutaoException.UserdataCorrupted(SH.ServiceAchievementUserdataCorruptedAchievementIdNotUnique, ex);
        }
    }

    public ValueTask<int> GetFinishedAchievementCountByArchiveIdAsync(Guid archiveId, CancellationToken token = default)
    {
        return this.QueryAsync<EntityAchievement, int>(
            (query, token) => query
                .Where(a => a.ArchiveId == archiveId)
                .Where(a => a.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
                .CountAsync(token),
            token);
    }

    [SuppressMessage("", "CA1305")]
    public ValueTask<List<EntityAchievement>> GetLatestFinishedAchievementListByArchiveIdAsync(Guid archiveId, int take, CancellationToken token = default)
    {
        return this.ListAsync<EntityAchievement, EntityAchievement>(
            query => query
                .Where(a => a.ArchiveId == archiveId)
                .Where(a => a.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
                .OrderByDescending(a => a.Time.ToString())
                .Take(take),
            token);
    }

    public void OverwriteAchievement(EntityAchievement achievement)
    {
        this.DeleteByInnerId(achievement);
        if (achievement.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
        {
            this.Add(achievement);
        }
    }

    public async ValueTask OverwriteAchievementAsync(EntityAchievement achievement, CancellationToken token = default)
    {
        await this.DeleteByInnerIdAsync(achievement, token).ConfigureAwait(false);
        if (achievement.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
        {
            await this.AddAsync(achievement, token).ConfigureAwait(false);
        }
    }

    public ObservableCollection<AchievementArchive> GetAchievementArchiveCollection()
    {
        return this.ObservableCollection<AchievementArchive>();
    }

    public async ValueTask RemoveAchievementArchiveAsync(AchievementArchive archive, CancellationToken token = default)
    {
        // It will cascade delete the achievements.
        await this.DeleteAsync(archive, token).ConfigureAwait(false);
    }

    public List<EntityAchievement> GetAchievementListByArchiveId(Guid archiveId)
    {
        return this.ListByArchiveId<EntityAchievement>(archiveId);
    }

    public ValueTask<List<EntityAchievement>> GetAchievementListByArchiveIdAsync(Guid archiveId, CancellationToken token = default)
    {
        return this.ListByArchiveIdAsync<EntityAchievement>(archiveId, token);
    }

    public List<AchievementArchive> GetAchievementArchiveList()
    {
        return this.List<AchievementArchive>();
    }

    public ValueTask<List<AchievementArchive>> GetAchievementArchiveListAsync(CancellationToken token = default)
    {
        return this.ListAsync<AchievementArchive>(token);
    }
}