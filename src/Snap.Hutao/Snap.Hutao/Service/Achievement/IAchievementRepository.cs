// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;
using EntityArchive = Snap.Hutao.Model.Entity.AchievementArchive;

namespace Snap.Hutao.Service.Achievement;

internal interface IAchievementRepository : IRepository<EntityArchive>, IRepository<EntityAchievement>
{
    void AddAchievementArchive(EntityArchive archive);

    ObservableCollection<EntityArchive> GetAchievementArchiveCollection();

    List<EntityArchive> GetAchievementArchiveList();

    EntityArchive? GetAchievementArchiveById(Guid archiveId);

    EntityArchive? GetAchievementArchiveByName(string name);

    void RemoveAchievementArchive(EntityArchive archive);

    List<EntityAchievement> GetAchievementListByArchiveId(Guid archiveId);

    Dictionary<AchievementId, EntityAchievement> GetAchievementMapByArchiveId(Guid archiveId);

    int GetFinishedAchievementCountByArchiveId(Guid archiveId);

    List<EntityAchievement> GetLatestFinishedAchievementListByArchiveId(Guid archiveId, int take);

    void OverwriteAchievement(EntityAchievement achievement);
}