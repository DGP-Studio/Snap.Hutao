// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

internal interface IAchievementDbService
{
    ValueTask RemoveAchievementArchiveAsync(Model.Entity.AchievementArchive archive);

    ObservableCollection<Model.Entity.AchievementArchive> GetAchievementArchiveCollection();

    List<Model.Entity.AchievementArchive> GetAchievementArchiveList();

    ValueTask<List<Model.Entity.AchievementArchive>> GetAchievementArchiveListAsync();

    List<EntityAchievement> GetAchievementListByArchiveId(Guid archiveId);

    ValueTask<List<EntityAchievement>> GetAchievementListByArchiveIdAsync(Guid archiveId);

    Dictionary<AchievementId, EntityAchievement> GetAchievementMapByArchiveId(Guid archiveId);

    ValueTask<int> GetFinishedAchievementCountByArchiveIdAsync(Guid archiveId);

    ValueTask<List<EntityAchievement>> GetLatestFinishedAchievementListByArchiveIdAsync(Guid archiveId, int take);

    void OverwriteAchievement(EntityAchievement achievement);

    ValueTask OverwriteAchievementAsync(EntityAchievement achievement);
}