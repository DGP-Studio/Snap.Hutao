// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;
using EntityArchive = Snap.Hutao.Model.Entity.AchievementArchive;

namespace Snap.Hutao.Service.Achievement;

internal interface IAchievementRepository : IRepository<EntityArchive>, IRepository<EntityAchievement>
{
    void AddAchievementArchive(EntityArchive archive);

    ObservableCollection<EntityArchive> GetAchievementArchiveCollection();

    ImmutableArray<EntityArchive> GetAchievementArchiveImmutableArray();

    EntityArchive? GetAchievementArchiveById(Guid archiveId);

    EntityArchive? GetAchievementArchiveByName(string name);

    void RemoveAchievementArchive(EntityArchive archive);

    ImmutableArray<EntityAchievement> GetAchievementImmutableArrayByArchiveId(Guid archiveId);

    FrozenDictionary<AchievementId, EntityAchievement> GetAchievementMapByArchiveId(Guid archiveId);

    int GetFinishedAchievementCountByArchiveId(Guid archiveId);

    ImmutableArray<EntityAchievement> GetLatestFinishedAchievementImmutableArrayByArchiveId(Guid archiveId, int take);

    void OverwriteAchievement(EntityAchievement achievement);
}