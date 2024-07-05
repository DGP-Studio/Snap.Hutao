// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
        return service.List(query => query.Where(predicate));
    }

    public static List<TResult> List<TEntity, TResult>(this IAppDbService<TEntity> service, Func<IQueryable<TEntity>, IQueryable<TResult>> query)
        where TEntity : class
    {
        return service.Query(query1 => query(query1).ToList());
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