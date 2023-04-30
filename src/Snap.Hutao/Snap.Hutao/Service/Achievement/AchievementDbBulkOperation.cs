// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
[Injection(InjectAs.Scoped)]
internal sealed class AchievementDbBulkOperation
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// 构造一个新的成就数据库操作
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public AchievementDbBulkOperation(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 合并
    /// </summary>
    /// <param name="archiveId">成就id</param>
    /// <param name="items">待合并的项</param>
    /// <param name="aggressive">是否贪婪</param>
    /// <returns>导入结果</returns>
    [SuppressMessage("", "SH002")]
    public ImportResult Merge(Guid archiveId, IEnumerable<UIAFItem> items, bool aggressive)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            IOrderedQueryable<EntityAchievement> oldData = appDbContext.Achievements
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

                            if (entity == null && uiaf != null)
                            {
                                appDbContext.Achievements.AddAndSave(EntityAchievement.Create(archiveId, uiaf));
                                add++;
                                continue;
                            }
                            else if (entity != null && uiaf == null)
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
                                    appDbContext.Achievements.AddAndSave(EntityAchievement.Create(archiveId, uiaf));
                                    update++;
                                }
                            }
                            else
                            {
                                // entity.Id > uiaf.Id
                                moveEntity = false;
                                moveUIAF = true;

                                appDbContext.Achievements.AddAndSave(EntityAchievement.Create(archiveId, uiaf));
                                add++;
                            }
                        }
                    }
                }
            }

            return new(add, update, 0);
        }
    }

    /// <summary>
    /// 覆盖
    /// </summary>
    /// <param name="archiveId">成就id</param>
    /// <param name="items">待覆盖的项</param>
    /// <returns>导入结果</returns>
    [SuppressMessage("", "SH002")]
    public ImportResult Overwrite(Guid archiveId, IEnumerable<EntityAchievement> items)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            IOrderedQueryable<EntityAchievement> oldData = appDbContext.Achievements
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

                            if (oldEntity == null && newEntity != null)
                            {
                                appDbContext.Achievements.AddAndSave(newEntity);
                                add++;
                                continue;
                            }
                            else if (oldEntity != null && newEntity == null)
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

            return new(add, update, remove);
        }
    }
}