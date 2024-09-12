// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Snap.Hutao.Service.Abstraction;

internal static class RepositoryCollectionExtension
{
    public static List<TEntity> List<TEntity>(this IRepository<TEntity> repository)
        where TEntity : class
    {
        return repository.Query(query => query.ToList());
    }

    public static List<TEntity> List<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return repository.List(query => query.Where(predicate));
    }

    public static List<TResult> List<TEntity, TResult>(this IRepository<TEntity> repository, Func<IQueryable<TEntity>, IQueryable<TResult>> query)
        where TEntity : class
    {
        return repository.Query(query1 => query(query1).ToList());
    }

    public static ObservableCollection<TEntity> ObservableCollection<TEntity>(this IRepository<TEntity> repository)
        where TEntity : class
    {
        return repository.Query(query => query.ToObservableCollection());
    }

    public static ObservableCollection<TEntity> ObservableCollection<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        return repository.Query(query => query.Where(predicate).ToObservableCollection());
    }
}