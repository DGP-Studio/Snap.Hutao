// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Achievement;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;
using MetadataAchievement = Snap.Hutao.Model.Metadata.Achievement.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 成就服务
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IAchievementService))]
internal sealed partial class AchievementService : IAchievementService
{
    private readonly ScopedDbCurrent<AchievementArchive, Message.AchievementArchiveChangedMessage> dbCurrent;
    private readonly AchievementDbBulkOperation achievementDbBulkOperation;
    private readonly IAchievementDbService achievementDbService;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public List<AchievementView> GetAchievementViewList(AchievementArchive archive, List<MetadataAchievement> metadata)
    {
        Dictionary<AchievementId, EntityAchievement> entities = achievementDbService.GetAchievementMapByArchiveId(archive.InnerId);

        return metadata.SelectList(meta =>
        {
            EntityAchievement entity = entities.GetValueOrDefault(meta.Id) ?? EntityAchievement.From(archive.InnerId, meta.Id);
            return new AchievementView(entity, meta);
        });
    }

    /// <inheritdoc/>
    public void SaveAchievement(AchievementView achievement)
    {
        achievementDbService.OverwriteAchievement(achievement.Entity);
    }
}