// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Collection;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Service.Abstraction;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class AchievementRepositoryOperation
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<AchievementRepositoryOperation> logger;

    public ImportResult Merge(Guid archiveId, IEnumerable<UIAFItem> items, bool aggressive)
    {
        logger.LogInformation("Perform merge operation for [Archive: {Id}], [Aggressive: {Aggressive}]", archiveId, aggressive);
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.GetAppDbContext();

            IOrderedQueryable<EntityAchievement> oldData = appDbContext.Achievements
                .AsNoTracking()
                .Where(a => a.ArchiveId == archiveId)
                .OrderBy(a => a.Id);

            (int add, int update) = (0, 0);

            using (TwoEnumerbleEnumerator<EntityAchievement, UIAFItem> enumerator = new(oldData, items))
            {
                (bool moveEntity, bool moveUIAF) = (true, true);

                while (true)
                {
                    if (!enumerator.MoveNext(ref moveEntity, ref moveUIAF))
                    {
                        break;
                    }

                    (EntityAchievement? entity, UIAFItem? uiaf) = enumerator.Current;

                    switch (entity, uiaf)
                    {
                        case (null, not null):
                            appDbContext.Achievements.AddAndSave(EntityAchievement.From(archiveId, uiaf));
                            add++;
                            continue;
                        case (not null, null):
                            continue; // Skipped
                        default:
                            ArgumentNullException.ThrowIfNull(entity);
                            ArgumentNullException.ThrowIfNull(uiaf);

                            switch (entity.Id.CompareTo(uiaf.Id))
                            {
                                case < 0:
                                    (moveEntity, moveUIAF) = (true, false);
                                    break;
                                case 0:
                                    (moveEntity, moveUIAF) = (true, true);

                                    if (aggressive)
                                    {
                                        appDbContext.Achievements.RemoveAndSave(entity);
                                        appDbContext.Achievements.AddAndSave(EntityAchievement.From(archiveId, uiaf));
                                        update++;
                                    }

                                    break;
                                case > 0:
                                    (moveEntity, moveUIAF) = (false, true);

                                    appDbContext.Achievements.AddAndSave(EntityAchievement.From(archiveId, uiaf));
                                    add++;
                                    break;
                            }

                            break;
                    }
                }
            }

            logger.LogInformation("Merge operation complete, [Add: {Add}], [Update: {Update}]", add, update);
            return new(add, update, 0);
        }
    }

    public ImportResult Overwrite(Guid archiveId, IEnumerable<EntityAchievement> items)
    {
        logger.LogInformation("Perform Overwrite Operation for [Archive: {Id}]", archiveId);
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.GetAppDbContext();

            IOrderedQueryable<EntityAchievement> oldData = appDbContext.Achievements
                .AsNoTracking()
                .Where(a => a.ArchiveId == archiveId)
                .OrderBy(a => a.Id);

            (int add, int update, int remove) = (0, 0, 0);

            using (TwoEnumerbleEnumerator<EntityAchievement, EntityAchievement> enumerator = new(oldData, items))
            {
                (bool moveOld, bool moveNew) = (true, true);

                while (true)
                {
                    if (!enumerator.MoveNext(ref moveOld, ref moveNew))
                    {
                        break;
                    }

                    (EntityAchievement? oldEntity, EntityAchievement? newEntity) = enumerator.Current;

                    switch (oldEntity, newEntity)
                    {
                        case (null, not null):
                            appDbContext.Achievements.AddAndSave(newEntity);
                            add++;
                            continue;
                        case (not null, null):
                            appDbContext.Achievements.RemoveAndSave(oldEntity);
                            remove++;
                            continue;
                        default:
                            ArgumentNullException.ThrowIfNull(oldEntity);
                            ArgumentNullException.ThrowIfNull(newEntity);

                            switch (oldEntity.Id.CompareTo(newEntity.Id))
                            {
                                case < 0:
                                    (moveOld, moveNew) = (true, false);
                                    break;
                                case 0:
                                    (moveOld, moveNew) = (true, true);

                                    if (oldEntity.Equals(newEntity))
                                    {
                                        // Skip same entry, reduce write operation.
                                        continue;
                                    }
                                    else
                                    {
                                        appDbContext.Achievements.RemoveAndSave(oldEntity);
                                        appDbContext.Achievements.AddAndSave(newEntity);
                                        update++;
                                    }

                                    break;
                                case > 0:
                                    (moveOld, moveNew) = (false, true);

                                    appDbContext.Achievements.AddAndSave(newEntity);
                                    add++;
                                    break;
                            }

                            break;
                    }
                }
            }

            logger.LogInformation("Overwrite Operation Complete, Add: {Add}, Update: {Update}, Remove: {Remove}", add, update, remove);
            return new(add, update, remove);
        }
    }
}