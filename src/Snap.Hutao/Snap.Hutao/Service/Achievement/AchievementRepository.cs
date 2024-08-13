// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IAchievementRepository))]
internal sealed partial class AchievementRepository : IAchievementRepository
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
            throw HutaoException.Throw(SH.ServiceAchievementUserdataCorruptedAchievementIdNotUnique, ex);
        }
    }

    public int GetFinishedAchievementCountByArchiveId(Guid archiveId)
    {
        return this.Query<EntityAchievement, int>(query => query
            .Where(a => a.ArchiveId == archiveId)
            .Where(a => a.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
            .Count());
    }

    public List<EntityAchievement> GetLatestFinishedAchievementListByArchiveId(Guid archiveId, int take)
    {
        return this.List<EntityAchievement, EntityAchievement>(query => query
            .Where(a => a.ArchiveId == archiveId)
            .Where(a => a.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
            .OrderByDescending(a => a.Time.ToString())
            .Take(take));
    }

    public void OverwriteAchievement(EntityAchievement achievement)
    {
        this.DeleteByInnerId(achievement);
        if (achievement.Status >= Model.Intrinsic.AchievementStatus.STATUS_FINISHED)
        {
            this.Add(achievement);
        }
    }

    public ObservableCollection<AchievementArchive> GetAchievementArchiveCollection()
    {
        return this.ObservableCollection<AchievementArchive>();
    }

    public void RemoveAchievementArchive(AchievementArchive archive)
    {
        // Cascade delete the achievements.
        this.Delete(archive);
    }

    public List<EntityAchievement> GetAchievementListByArchiveId(Guid archiveId)
    {
        return this.ListByArchiveId<EntityAchievement>(archiveId);
    }

    public List<AchievementArchive> GetAchievementArchiveList()
    {
        return this.List<AchievementArchive>();
    }

    public AchievementArchive? GetAchievementArchiveById(Guid archiveId)
    {
        return this.SingleOrDefault<AchievementArchive>(a => a.InnerId == archiveId);
    }

    public AchievementArchive? GetAchievementArchiveByName(string name)
    {
        return this.SingleOrDefault<AchievementArchive>(a => a.Name == name);
    }

    public void AddAchievementArchive(AchievementArchive archive)
    {
        this.Add(archive);
    }
}