// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.InterChange.Achievement;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 成就数据库操作
/// 双指针操作
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class AchievementDbBulkOperation
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<AchievementDbBulkOperation> logger;

    /// <summary>
    /// 合并
    /// </summary>
    /// <param name="archiveId">成就id</param>
    /// <param name="items">待合并的项</param>
    /// <param name="aggressive">是否贪婪</param>
    /// <returns>导入结果</returns>
    public ImportResult Merge(Guid archiveId, IEnumerable<UIAFItem> items, bool aggressive)
    {
        logger.LogInformation("Perform {Method} Operation for archive: {Id}, Aggressive: {Aggressive}", nameof(Merge), archiveId, aggressive);
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            IOrderedQueryable<EntityAchievement> oldData = appDbContext.Achievements
                .AsNoTracking()
                .Where(a => a.ArchiveId == archiveId)
                .OrderBy(a => a.Id);

            int add = 0;
            int update = 0;

            using (IEnumerator<EntityAchievement> entityEnumerator = oldData.GetEnumerator())
            {
                using (IEnumerator<UIAFItem> uiafEnumerator = items.GetEnumerator())
                {
                    bool moveEntity = true;
                    bool moveUIAF = true;

                    while (true)
                    {
                        bool moveEntityResult = moveEntity && entityEnumerator.MoveNext();
                        bool moveUIAFResult = moveUIAF && uiafEnumerator.MoveNext();

                        if (!(moveEntityResult || moveUIAFResult))
                        {
                            break;
                        }
                        else
                        {
                            EntityAchievement? entity = entityEnumerator.Current;
                            UIAFItem? uiaf = uiafEnumerator.Current;

                            if (entity is null && uiaf is not null)
                            {
                                appDbContext.Achievements.AddAndSave(EntityAchievement.From(archiveId, uiaf));
                                add++;
                                continue;
                            }
                            else if (entity is not null && uiaf is null)
                            {
                                // skip
                                continue;
                            }

                            if (entity!.Id < uiaf!.Id)
                            {
                                moveEntity = true;
                                moveUIAF = false;
                            }
                            else if (entity.Id == uiaf.Id)
                            {
                                moveEntity = true;
                                moveUIAF = true;

                                if (aggressive)
                                {
                                    appDbContext.Achievements.RemoveAndSave(entity);
                                    appDbContext.Achievements.AddAndSave(EntityAchievement.From(archiveId, uiaf));
                                    update++;
                                }
                            }
                            else
                            {
                                // entity.Id > uiaf.Id
                                moveEntity = false;
                                moveUIAF = true;

                                appDbContext.Achievements.AddAndSave(EntityAchievement.From(archiveId, uiaf));
                                add++;
                            }
                        }
                    }
                }
            }

            logger.LogInformation("{Method} Operation Complete, Add: {Add}, Update: {Update}", nameof(Merge), add, update);
            return new(add, update, 0);
        }
    }

    /// <summary>
    /// 覆盖
    /// </summary>
    /// <param name="archiveId">成就id</param>
    /// <param name="items">待覆盖的项</param>
    /// <returns>导入结果</returns>
    public ImportResult Overwrite(Guid archiveId, IEnumerable<EntityAchievement> items)
    {
        logger.LogInformation("Perform {Method} Operation for archive: {Id}", nameof(Overwrite), archiveId);
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            IOrderedQueryable<EntityAchievement> oldData = appDbContext.Achievements
                .AsNoTracking()
                .Where(a => a.ArchiveId == archiveId)
                .OrderBy(a => a.Id);

            int add = 0;
            int update = 0;
            int remove = 0;

            using (IEnumerator<EntityAchievement> oldDataEnumerator = oldData.GetEnumerator())
            {
                using (IEnumerator<EntityAchievement> newDataEnumerator = items.GetEnumerator())
                {
                    bool moveOld = true;
                    bool moveNew = true;

                    while (true)
                    {
                        bool moveOldResult = moveOld && oldDataEnumerator.MoveNext();
                        bool moveNewResult = moveNew && newDataEnumerator.MoveNext();

                        if (moveOldResult || moveNewResult)
                        {
                            EntityAchievement? oldEntity = oldDataEnumerator.Current;
                            EntityAchievement? newEntity = newDataEnumerator.Current;

                            if (oldEntity is null && newEntity is not null)
                            {
                                appDbContext.Achievements.AddAndSave(newEntity);
                                add++;
                                continue;
                            }
                            else if (oldEntity is not null && newEntity is null)
                            {
                                appDbContext.Achievements.RemoveAndSave(oldEntity);
                                remove++;
                                continue;
                            }

                            if (oldEntity!.Id < newEntity!.Id)
                            {
                                moveOld = true;
                                moveNew = false;
                                appDbContext.Achievements.RemoveAndSave(oldEntity);
                                remove++;
                            }
                            else if (oldEntity.Id == newEntity.Id)
                            {
                                moveOld = true;
                                moveNew = true;

                                if (oldEntity.Equals(newEntity))
                                {
                                    // skip same entry.
                                    continue;
                                }
                                else
                                {
                                    appDbContext.Achievements.RemoveAndSave(oldEntity);
                                    appDbContext.Achievements.AddAndSave(newEntity);
                                    update++;
                                }
                            }
                            else
                            {
                                // entity.Id > uiaf.Id
                                moveOld = false;
                                moveNew = true;
                                appDbContext.Achievements.AddAndSave(newEntity);
                                add++;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            logger.LogInformation("{Method} Operation Complete, Add: {Add}, Update: {Update}, Remove: {Remove}", nameof(Overwrite), add, update, remove);
            return new(add, update, remove);
        }
    }
}