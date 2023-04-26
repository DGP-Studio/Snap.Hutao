// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Database;

/// <summary>
/// 数据库集合扩展
/// </summary>
[HighQuality]
internal static class DbSetExtension
{
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
        return dbSet.SaveChangesAndClearChangeTracker();
    }

    /// <summary>
    /// 异步添加并保存
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="entity">实体</param>
    /// <returns>影响条数</returns>
    public static ValueTask<int> AddAndSaveAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Add(entity);
        return dbSet.SaveChangesAndClearChangeTrackerAsync();
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
        return dbSet.SaveChangesAndClearChangeTracker();
    }

    /// <summary>
    /// 异步添加列表并保存
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="entities">实体</param>
    /// <returns>影响条数</returns>
    public static ValueTask<int> AddRangeAndSaveAsync<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        dbSet.AddRange(entities);
        return dbSet.SaveChangesAndClearChangeTrackerAsync();
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
        return dbSet.SaveChangesAndClearChangeTracker();
    }

    /// <summary>
    /// 异步移除并保存
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="entity">实体</param>
    /// <returns>影响条数</returns>
    public static ValueTask<int> RemoveAndSaveAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Remove(entity);
        return dbSet.SaveChangesAndClearChangeTrackerAsync();
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
        return dbSet.SaveChangesAndClearChangeTracker();
    }

    /// <summary>
    /// 异步更新并保存
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="dbSet">数据库集</param>
    /// <param name="entity">实体</param>
    /// <returns>影响条数</returns>
    public static ValueTask<int> UpdateAndSaveAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Update(entity);
        return dbSet.SaveChangesAndClearChangeTrackerAsync();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int SaveChangesAndClearChangeTracker<TEntity>(this DbSet<TEntity> dbSet)
        where TEntity : class
    {
        DbContext dbContext = dbSet.Context();
        int count = dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();
        return count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async ValueTask<int> SaveChangesAndClearChangeTrackerAsync<TEntity>(this DbSet<TEntity> dbSet)
        where TEntity : class
    {
        DbContext dbContext = dbSet.Context();
        int count = await dbContext.SaveChangesAsync().ConfigureAwait(false);
        dbContext.ChangeTracker.Clear();
        return count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DbContext Context<TEntity>(this DbSet<TEntity> dbSet)
        where TEntity : class
    {
        return dbSet.GetService<ICurrentDbContext>().Context;
    }
}