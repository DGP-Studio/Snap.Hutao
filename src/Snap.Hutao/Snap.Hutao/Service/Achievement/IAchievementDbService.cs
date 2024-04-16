// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

internal interface IAchievementDbService : IAppDbService<Model.Entity.AchievementArchive>, IAppDbService<EntityAchievement>
{
    ValueTask RemoveAchievementArchiveAsync(Model.Entity.AchievementArchive archive, CancellationToken token = default);

    ObservableCollection<Model.Entity.AchievementArchive> GetAchievementArchiveCollection();

    List<Model.Entity.AchievementArchive> GetAchievementArchiveList();

    ValueTask<List<Model.Entity.AchievementArchive>> GetAchievementArchiveListAsync(CancellationToken token = default);

    List<EntityAchievement> GetAchievementListByArchiveId(Guid archiveId);

    ValueTask<List<EntityAchievement>> GetAchievementListByArchiveIdAsync(Guid archiveId, CancellationToken token = default);

    Dictionary<AchievementId, EntityAchievement> GetAchievementMapByArchiveId(Guid archiveId);

    ValueTask<int> GetFinishedAchievementCountByArchiveIdAsync(Guid archiveId, CancellationToken token = default);

    ValueTask<List<EntityAchievement>> GetLatestFinishedAchievementListByArchiveIdAsync(Guid archiveId, int take, CancellationToken token = default);

    void OverwriteAchievement(EntityAchievement achievement);

    ValueTask OverwriteAchievementAsync(EntityAchievement achievement, CancellationToken token = default);
}