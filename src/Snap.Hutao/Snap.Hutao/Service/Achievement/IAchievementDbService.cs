// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

internal interface IAchievementDbService : IAppDbService<Model.Entity.AchievementArchive>, IAppDbService<EntityAchievement>
{
    ObservableCollection<Model.Entity.AchievementArchive> GetAchievementArchiveCollection();

    List<Model.Entity.AchievementArchive> GetAchievementArchiveList();

    List<EntityAchievement> GetAchievementListByArchiveId(Guid archiveId);

    Dictionary<AchievementId, EntityAchievement> GetAchievementMapByArchiveId(Guid archiveId);

    int GetFinishedAchievementCountByArchiveId(Guid archiveId);

    List<EntityAchievement> GetLatestFinishedAchievementListByArchiveId(Guid archiveId, int take);

    void OverwriteAchievement(EntityAchievement achievement);

    void RemoveAchievementArchive(Model.Entity.AchievementArchive archive);
}