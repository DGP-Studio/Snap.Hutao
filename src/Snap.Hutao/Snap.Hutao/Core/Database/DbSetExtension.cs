// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Snap.Hutao.Core.Database;

/// <summary>
/// 数据库集合上下文
/// </summary>
public static class DbSetExtension
{
    /// <summary>
    /// 获取对应的数据库上下文
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <returns>对应的数据库上下文</returns>
    public static DbContext Context<TEntity>(this DbSet<TEntity> dbSet)
        where TEntity : class
    {
        return dbSet.GetService<ICurrentDbContext>().Context;
    }

    /// <summary>
    /// 获取或添加一个对应的实体
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="predicate">谓词</param>
    /// <param name="entityFactory">实体工厂</param>
    /// <param name="added">是否添加</param>
    /// <returns>实体</returns>
    public static TEntity SingleOrAdd<TEntity>(this DbSet<TEntity> dbSet, Func<TEntity, bool> predicate, Func<TEntity> entityFactory, out bool added)
        where TEntity : class
    {
        added = false;
        TEntity? entry = dbSet.SingleOrDefault(predicate);

        if (entry == null)
        {
            entry = entityFactory();
            dbSet.Add(entry);
            dbSet.Context().SaveChanges();

            added = true;
        }

        return entry;
    }

    /// <summary>
    /// 添加并保存
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="entity">实体</param>
    /// <returns>影响条数</returns>
    public static int AddAndSave<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Add(entity);
        return dbSet.Context().SaveChanges();
    }

    /// <summary>
    /// 移除并保存
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="entity">实体</param>
    /// <returns>影响条数</returns>
    public static int RemoveAndSave<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Remove(entity);
        return dbSet.Context().SaveChanges();
    }

    /// <summary>
    /// 更新并保存
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="entity">实体</param>
    /// <returns>影响条数</returns>
    public static int UpdateAndSave<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Update(entity);
        return dbSet.Context().SaveChanges();
    }
}
