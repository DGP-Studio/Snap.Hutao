// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Context.Database;
using Snap.Hutao.Model.InterChange.Achievement;
using System.Linq;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 成就数据库操作
/// </summary>
public class AchievementDbOperation
{
    private readonly AppDbContext appDbContext;

    /// <summary>
    /// 构造一个新的成就数据库操作
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    public AchievementDbOperation(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    /// <summary>
    /// 合并
    /// </summary>
    /// <param name="archiveId">成就id</param>
    /// <param name="items">待合并的项</param>
    /// <param name="aggressive">是否贪婪</param>
    /// <returns>导入结果</returns>
    public ImportResult Merge(Guid archiveId, IEnumerable<UIAFItem> items, bool aggressive)
    {
        IOrderedQueryable<EntityAchievement> oldData = appDbContext.Achievements
            .Where(a => a.ArchiveId == archiveId)
            .OrderBy(a => a.Id);

        int add = 0;
        int update = 0;
        int remove = 0;

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
                            AddEntity(EntityAchievement.Create(archiveId, uiaf));
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
                                RemoveEntity(entity);
                                AddEntity(EntityAchievement.Create(archiveId, uiaf));
                                update++;
                            }
                        }
                        else
                        {
                            // entity.Id > uiaf.Id
                            moveEntity = false;
                            moveUIAF = true;

                            AddEntity(EntityAchievement.Create(archiveId, uiaf));
                            add++;
                        }
                    }
                }
            }
        }

        return new(add, update, remove);
    }

    /// <summary>
    /// 覆盖
    /// </summary>
    /// <param name="archiveId">成就id</param>
    /// <param name="items">待覆盖的项</param>
    /// <returns>导入结果</returns>
    public ImportResult Overwrite(Guid archiveId, IEnumerable<EntityAchievement> items)
    {
        IQueryable<EntityAchievement> oldData = appDbContext.Achievements
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
                            AddEntity(newEntity);
                            add++;
                            continue;
                        }
                        else if (oldEntity != null && newEntity == null)
                        {
                            RemoveEntity(oldEntity);
                            remove++;
                            continue;
                        }

                        if (oldEntity!.Id < newEntity!.Id)
                        {
                            moveOld = true;
                            moveNew = false;
                            RemoveEntity(oldEntity);
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
                                RemoveEntity(oldEntity);
                                AddEntity(newEntity);
                                update++;
                            }
                        }
                        else
                        {
                            // entity.Id > uiaf.Id
                            moveOld = false;
                            moveNew = true;
                            AddEntity(newEntity);
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

    private void AddEntity(EntityAchievement entity)
    {
        appDbContext.Achievements.Add(entity);
        appDbContext.SaveChanges();
    }

    private void RemoveEntity(EntityAchievement entity)
    {
        appDbContext.Achievements.Remove(entity);
        appDbContext.SaveChanges();
    }
}