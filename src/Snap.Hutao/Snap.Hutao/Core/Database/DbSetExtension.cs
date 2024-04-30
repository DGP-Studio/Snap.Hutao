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
    public static int AddAndSave<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Add(entity);
        return dbSet.SaveChangesAndClearChangeTracker();
    }

    public static ValueTask<int> AddAndSaveAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity, CancellationToken token = default)
        where TEntity : class
    {
        dbSet.Add(entity);
        return dbSet.SaveChangesAndClearChangeTrackerAsync(token);
    }

    public static int AddRangeAndSave<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        dbSet.AddRange(entities);
        return dbSet.SaveChangesAndClearChangeTracker();
    }

    public static ValueTask<int> AddRangeAndSaveAsync<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities, CancellationToken token = default)
        where TEntity : class
    {
        dbSet.AddRange(entities);
        return dbSet.SaveChangesAndClearChangeTrackerAsync(token);
    }

    public static int RemoveAndSave<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Remove(entity);
        return dbSet.SaveChangesAndClearChangeTracker();
    }

    public static ValueTask<int> RemoveAndSaveAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity, CancellationToken token = default)
        where TEntity : class
    {
        dbSet.Remove(entity);
        return dbSet.SaveChangesAndClearChangeTrackerAsync(token);
    }

    public static int UpdateAndSave<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class
    {
        dbSet.Update(entity);
        return dbSet.SaveChangesAndClearChangeTracker();
    }

    public static ValueTask<int> UpdateAndSaveAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity, CancellationToken token = default)
        where TEntity : class
    {
        dbSet.Update(entity);
        return dbSet.SaveChangesAndClearChangeTrackerAsync(token);
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
    private static async ValueTask<int> SaveChangesAndClearChangeTrackerAsync<TEntity>(this DbSet<TEntity> dbSet, CancellationToken token = default)
        where TEntity : class
    {
        DbContext dbContext = dbSet.Context();
        int count = await dbContext.SaveChangesAsync(token).ConfigureAwait(false);
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