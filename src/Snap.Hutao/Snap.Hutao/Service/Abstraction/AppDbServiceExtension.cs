// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using System.Linq.Expressions;

namespace Snap.Hutao.Service.Abstraction;

internal static class AppDbServiceExtension
{
    public static TResult Execute<TEntity, TResult>(this IAppDbService<TEntity> service, Func<DbSet<TEntity>, TResult> func)
        where TEntity : class
    {
        using (IServiceScope scope = service.ServiceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.GetAppDbContext();
            return func(appDbContext.Set<TEntity>());
        }
    }

    public static async ValueTask<TResult> ExecuteAsync<TEntity, TResult>(this IAppDbService<TEntity> service, Func<DbSet<TEntity>, ValueTask<TResult>> asyncFunc)
        where TEntity : class
    {
        using (IServiceScope scope = service.ServiceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.GetAppDbContext();
            return await asyncFunc(appDbContext.Set<TEntity>()).ConfigureAwait(false);
        }
    }

    public static async ValueTask<TResult> ExecuteAsync<TEntity, TResult>(this IAppDbService<TEntity> service, Func<DbSet<TEntity>, CancellationToken, ValueTask<TResult>> asyncFunc, CancellationToken token)
        where TEntity : class
    {
        using (IServiceScope scope = service.ServiceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.GetAppDbContext();
            return await asyncFunc(appDbContext.Set<TEntity>(), token).ConfigureAwait(false);
        }
    }

    public static async ValueTask<TResult> ExecuteAsync<TEntity, TResult>(this IAppDbService<TEntity> service, Func<DbSet<TEntity>, Task<TResult>> asyncFunc)
        where TEntity : class
    {
        using (IServiceScope scope = service.ServiceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.GetAppDbContext();
            return await asyncFunc(appDbContext.Set<TEntity>()).ConfigureAwait(false);
        }
    }

    public static async ValueTask<TResult> ExecuteAsync<TEntity, TResult>(this IAppDbService<TEntity> service, Func<DbSet<TEntity>, CancellationToken, Task<TResult>> asyncFunc, CancellationToken token)
        where TEntity : class
    {
        using (IServiceScope scope = service.ServiceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.GetAppDbContext();
            return await asyncFunc(appDbContext.Set<TEntity>(), token).ConfigureAwait(false);
        }
    }

    public static int Add<TEntity>(this IAppDbService<TEntity> service, TEntity entity)
        where TEntity : class
    {
        return service.Execute(dbset => dbset.AddAndSave(entity));
    }

    public static ValueTask<int> AddAsync<TEntity>(this IAppDbService<TEntity> service, TEntity entity, CancellationToken token = default)
        where TEntity : class
    {
        return service.ExecuteAsync((dbset, token) => dbset.AddAndSaveAsync(entity, token), token);
    }

    public static int AddRange<TEntity>(this IAppDbService<TEntity> service, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        return service.Execute(dbset => dbset.AddRangeAndSave(entities));
    }

    public static ValueTask<int> AddRangeAsync<TEntity>(this IAppDbService<TEntity> service, IEnumerable<TEntity> entities, CancellationToken token = default)
        where TEntity : class
    {
        return service.ExecuteAsync((dbset, token) => dbset.AddRangeAndSaveAsync(entities, token), token);
    }

    public static TResult Query<TEntity, TResult>(this IAppDbService<TEntity> service, Func<IQueryable<TEntity>, TResult> func)
        where TEntity : class
    {
        return service.Execute(dbset => func(dbset.AsNoTracking()));
    }

    public static ValueTask<TResult> QueryAsync<TEntity, TResult>(this IAppDbService<TEntity> service, Func<IQueryable<TEntity>, ValueTask<TResult>> func)
        where TEntity : class
    {
        return service.ExecuteAsync(dbset => func(dbset.AsNoTracking()));
    }

    public static ValueTask<TResult> QueryAsync<TEntity, TResult>(this IAppDbService<TEntity> service, Func<IQueryable<TEntity>, CancellationToken, ValueTask<TResult>> func, CancellationToken token = default)
        where TEntity : class
    {
        return service.ExecuteAsync((dbset, token) => func(dbset.AsNoTracking(), token), token);
    }

    public static ValueTask<TResult> QueryAsync<TEntity, TResult>(this IAppDbService<TEntity> service, Func<IQueryable<TEntity>, Task<TResult>> func)
        where TEntity : class
    {
        return service.ExecuteAsync(dbset => func(dbset.AsNoTracking()));
    }

    public static ValueTask<TResult> QueryAsync<TEntity, TResult>(this IAppDbService<TEntity> service, Func<IQueryable<TEntity>, CancellationToken, Task<TResult>> func, CancellationToken token = default)
        where TEntity : class
    {
        return service.ExecuteAsync((dbset, token) => func(dbset.AsNoTracking(), token), token);
    }

    public static TEntity Single<TEntity>(this IAppDbService<TEntity> service, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return service.Query(query => query.Single(predicate));
    }

    public static ValueTask<TEntity> SingleAsync<TEntity>(this IAppDbService<TEntity> service, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)
        where TEntity : class
    {
        return service.QueryAsync((query, token) => query.SingleAsync(predicate, token), token);
    }

    public static int Update<TEntity>(this IAppDbService<TEntity> service, TEntity entity)
        where TEntity : class
    {
        return service.Execute(dbset => dbset.UpdateAndSave(entity));
    }

    public static ValueTask<int> UpdateAsync<TEntity>(this IAppDbService<TEntity> service, TEntity entity, CancellationToken token = default)
        where TEntity : class
    {
        return service.ExecuteAsync((dbset, token) => dbset.UpdateAndSaveAsync(entity, token), token);
    }

    public static int Delete<TEntity>(this IAppDbService<TEntity> service, TEntity entity)
        where TEntity : class
    {
        return service.Execute(dbset => dbset.RemoveAndSave(entity));
    }

    public static ValueTask<int> DeleteAsync<TEntity>(this IAppDbService<TEntity> service, TEntity entity, CancellationToken token = default)
        where TEntity : class
    {
        return service.ExecuteAsync((dbset, token) => dbset.RemoveAndSaveAsync(entity, token), token);
    }
}