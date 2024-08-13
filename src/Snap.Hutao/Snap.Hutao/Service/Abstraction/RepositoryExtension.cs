// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using System.Linq.Expressions;

namespace Snap.Hutao.Service.Abstraction;

internal static class RepositoryExtension
{
    public static TResult Execute<TEntity, TResult>(this IRepository<TEntity> repository, Func<DbSet<TEntity>, TResult> func)
        where TEntity : class
    {
        using (IServiceScope scope = repository.ServiceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.GetAppDbContext();
            return func(appDbContext.Set<TEntity>());
        }
    }

    public static int Add<TEntity>(this IRepository<TEntity> repository, TEntity entity)
        where TEntity : class
    {
        return repository.Execute(dbset => dbset.AddAndSave(entity));
    }

    public static int AddRange<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        return repository.Execute(dbset => dbset.AddRangeAndSave(entities));
    }

    public static int Delete<TEntity>(this IRepository<TEntity> repository)
        where TEntity : class
    {
        return repository.Execute(dbset => dbset.ExecuteDelete());
    }

    public static int Delete<TEntity>(this IRepository<TEntity> repository, TEntity entity)
        where TEntity : class
    {
        return repository.Execute(dbset => dbset.RemoveAndSave(entity));
    }

    public static int Delete<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return repository.Execute(dbset => dbset.Where(predicate).ExecuteDelete());
    }

    public static TResult Query<TEntity, TResult>(this IRepository<TEntity> repository, Func<IQueryable<TEntity>, TResult> func)
        where TEntity : class
    {
        return repository.Execute(dbset => func(dbset.AsNoTracking()));
    }

    public static TEntity Single<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return repository.Query(query => query.Single(predicate));
    }

    public static TEntity? SingleOrDefault<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return repository.Query(query => query.SingleOrDefault(predicate));
    }

    public static void TransactionalExecute<TEntity>(this IRepository<TEntity> repository, Action<DbSet<TEntity>> action)
        where TEntity : class
    {
        using (IServiceScope scope = repository.ServiceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.GetAppDbContext();
            using (IDbContextTransaction transaction = appDbContext.Database.BeginTransaction())
            {
                action(appDbContext.Set<TEntity>());
                transaction.Commit();
            }
        }
    }

    public static TResult TransactionalExecute<TEntity, TResult>(this IRepository<TEntity> repository, Func<DbSet<TEntity>, TResult> func)
        where TEntity : class
    {
        using (IServiceScope scope = repository.ServiceProvider.CreateScope())
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

    public static int Update<TEntity>(this IRepository<TEntity> repository, TEntity entity)
        where TEntity : class
    {
        return repository.Execute(dbset => dbset.UpdateAndSave(entity));
    }
}