// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

internal interface IAchievementDbService
{
    Dictionary<AchievementId, EntityAchievement> GetAchievementMapByArchiveId(Guid archiveId);
    Task<int> GetFinishedAchievementCountByArchiveIdAsync(Guid archiveId);
    Task<List<EntityAchievement>> GetLatestFinishedAchievementListByArchiveIdAsync(Guid archiveId, int take);
}