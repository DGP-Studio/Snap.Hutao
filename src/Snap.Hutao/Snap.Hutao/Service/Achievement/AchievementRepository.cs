// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

[Service(ServiceLifetime.Singleton, typeof(IAchievementRepository))]
internal sealed partial class AchievementRepository : IAchievementRepository
{
    [GeneratedConstructor]
    public partial AchievementRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public FrozenDictionary<AchievementId, EntityAchievement> GetAchievementMapByArchiveId(Guid archiveId)
    {
        try
        {
            return this.Query<EntityAchievement, FrozenDictionary<AchievementId, EntityAchievement>>(query => query
                .Where(a => a.ArchiveId == archiveId)
                .ToFrozenDictionary(a => (AchievementId)a.Id));
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
            .Count(a => a.Status >= AchievementStatus.STATUS_FINISHED));
    }

    public ImmutableArray<EntityAchievement> GetLatestFinishedAchievementImmutableArrayByArchiveId(Guid archiveId, int take)
    {
        // SQLite does not support expressions of type 'DateTimeOffset' in ORDER BY clauses.
        // Convert the values to a supported type, or use LINQ to Objects to order the results on the client side.
        return this.ImmutableArray<EntityAchievement, EntityAchievement>(query => query
            .Where(a => a.ArchiveId == archiveId)
            .Where(a => a.Status >= AchievementStatus.STATUS_FINISHED)
#pragma warning disable CA1305 // EF Core does not support IFormatProvider
            .OrderByDescending(a => a.Time.ToString()) // ORDER BY CAST("a"."Time" AS TEXT) DESC
#pragma warning restore CA1305
            .Take(take));
    }

    public void OverwriteAchievement(EntityAchievement achievement)
    {
        this.DeleteByInnerId(achievement);
        if (achievement.Status >= AchievementStatus.STATUS_FINISHED)
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

    public ImmutableArray<EntityAchievement> GetAchievementImmutableArrayByArchiveId(Guid archiveId)
    {
        return this.ImmutableArrayByArchiveId<EntityAchievement>(archiveId);
    }

    public ImmutableArray<AchievementArchive> GetAchievementArchiveImmutableArray()
    {
        return this.ImmutableArray<AchievementArchive>();
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