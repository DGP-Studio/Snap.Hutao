// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Snap.Hutao.Service.Abstraction;

internal static class AppDbServiceCollectionExtension
{
    public static List<TEntity> List<TEntity>(this IAppDbService<TEntity> service)
        where TEntity : class
    {
        return service.Query(query => query.ToList());
    }

    public static List<TEntity> List<TEntity>(this IAppDbService<TEntity> service, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return service.Query(query => query.Where(predicate).ToList());
    }

    public static ValueTask<List<TEntity>> ListAsync<TEntity>(this IAppDbService<TEntity> service, CancellationToken token = default)
        where TEntity : class
    {
        return service.QueryAsync((query, token) => query.ToListAsync(token), token);
    }

    public static ValueTask<List<TEntity>> ListAsync<TEntity>(this IAppDbService<TEntity> service, Expression<Func<TEntity, bool>> predicate, CancellationToken token = default)
        where TEntity : class
    {
        return service.QueryAsync((query, token) => query.Where(predicate).ToListAsync(token), token);
    }

    public static ObservableCollection<TEntity> ObservableCollection<TEntity>(this IAppDbService<TEntity> service)
        where TEntity : class
    {
        return service.Query(query => query.ToObservableCollection());
    }

    public static ObservableCollection<TEntity> ObservableCollection<TEntity>(this IAppDbService<TEntity> service, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return service.Query(query => query.Where(predicate).ToObservableCollection());
    }
}