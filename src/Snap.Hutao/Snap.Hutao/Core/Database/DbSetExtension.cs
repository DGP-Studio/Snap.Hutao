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
    /// 异步添加并保存
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="entity">实体</param>
    /// <returns>影响条数</returns>
    public static async ValueTask<int> AddAndSaveAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Add(entity);
        return await dbSet.Context().SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// 添加列表并保存
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="entities">实体</param>
    /// <returns>影响条数</returns>
    public static int AddRangeAndSave<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        dbSet.AddRange(entities);
        return dbSet.Context().SaveChanges();
    }

    /// <summary>
    /// 异步添加列表并保存
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="entities">实体</param>
    /// <returns>影响条数</returns>
    public static async ValueTask<int> AddRangeAndSaveAsync<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        dbSet.AddRange(entities);
        return await dbSet.Context().SaveChangesAsync().ConfigureAwait(false);
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
    /// 异步移除并保存
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="entity">实体</param>
    /// <returns>影响条数</returns>
    public static async ValueTask<int> RemoveAndSaveAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Remove(entity);
        return await dbSet.Context().SaveChangesAsync().ConfigureAwait(false);
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

    /// <summary>
    /// 异步更新并保存
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="entity">实体</param>
    /// <returns>影响条数</returns>
    public static async ValueTask<int> UpdateAndSaveAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Update(entity);
        return await dbSet.Context().SaveChangesAsync().ConfigureAwait(false);
    }
}
