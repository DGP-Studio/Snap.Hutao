// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

    public static int Add<TEntity>(this IAppDbService<TEntity> service, TEntity entity)
        where TEntity : class
    {
        return service.Execute(dbset => dbset.AddAndSave(entity));
    }

    public static int AddRange<TEntity>(this IAppDbService<TEntity> service, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        return service.Execute(dbset => dbset.AddRangeAndSave(entities));
    }

    public static int Delete<TEntity>(this IAppDbService<TEntity> service)
        where TEntity : class
    {
        return service.Execute(dbset => dbset.ExecuteDelete());
    }

    public static int Delete<TEntity>(this IAppDbService<TEntity> service, TEntity entity)
        where TEntity : class
    {
        return service.Execute(dbset => dbset.RemoveAndSave(entity));
    }

    public static int Delete<TEntity>(this IAppDbService<TEntity> service, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return service.Execute(dbset => dbset.Where(predicate).ExecuteDelete());
    }

    public static TResult Query<TEntity, TResult>(this IAppDbService<TEntity> service, Func<IQueryable<TEntity>, TResult> func)
        where TEntity : class
    {
        return service.Execute(dbset => func(dbset.AsNoTracking()));
    }

    public static TEntity Single<TEntity>(this IAppDbService<TEntity> service, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return service.Query(query => query.Single(predicate));
    }

    public static TEntity? SingleOrDefault<TEntity>(this IAppDbService<TEntity> service, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return service.Query(query => query.SingleOrDefault(predicate));
    }

    public static void TransactionalExecute<TEntity>(this IAppDbService<TEntity> service, Action<DbSet<TEntity>> action)
        where TEntity : class
    {
        using (IServiceScope scope = service.ServiceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.GetAppDbContext();
            using (IDbContextTransaction transaction = appDbContext.Database.BeginTransaction())
            {
                action(appDbContext.Set<TEntity>());
                transaction.Commit();
            }
        }
    }

    public static TResult TransactionalExecute<TEntity, TResult>(this IAppDbService<TEntity> service, Func<DbSet<TEntity>, TResult> func)
        where TEntity : class
    {
        using (IServiceScope scope = service.ServiceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.GetAppDbContext();
            using (IDbContextTransaction transaction = appDbContext.Database.BeginTransaction())
            {
                TResult result = func(appDbContext.Set<TEntity>());
                transaction.Commit();
                return result;
            }
        }
    }

    public static int Update<TEntity>(this IAppDbService<TEntity> service, TEntity entity)
        where TEntity : class
    {
        return service.Execute(dbset => dbset.UpdateAndSave(entity));
    }
}