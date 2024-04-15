// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Abstraction;

namespace Snap.Hutao.Service.Abstraction;

internal static class AppDbServiceAppDbEntityExtension
{
    public static int DeleteByInnerId<TEntity>(this IAppDbService<TEntity> service, TEntity entity)
        where TEntity : class, IAppDbEntity
    {
        return service.Execute(dbset => dbset.ExecuteDeleteWhere(e => e.InnerId == entity.InnerId));
    }

    public static ValueTask<int> DeleteByInnerIdAsync<TEntity>(this IAppDbService<TEntity> service, TEntity entity)
        where TEntity : class, IAppDbEntity
    {
        return service.ExecuteAsync(dbset => dbset.ExecuteDeleteWhereAsync(e => e.InnerId == entity.InnerId));
    }
}