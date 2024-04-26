// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity.Abstraction;

namespace Snap.Hutao.Service.Abstraction;

internal static class AppDbServiceAppDbEntityExtension
{
    public static int DeleteByInnerId<TEntity>(this IAppDbService<TEntity> service, TEntity entity)
        where TEntity : class, IAppDbEntity
    {
        return service.DeleteByInnerId(entity.InnerId);
    }

    public static int DeleteByInnerId<TEntity>(this IAppDbService<TEntity> service, Guid innerId)
        where TEntity : class, IAppDbEntity
    {
        return service.Delete(e => e.InnerId == innerId);
    }

    public static ValueTask<int> DeleteByInnerIdAsync<TEntity>(this IAppDbService<TEntity> service, TEntity entity, CancellationToken token = default)
        where TEntity : class, IAppDbEntity
    {
        return service.DeleteByInnerIdAsync(entity.InnerId, token);
    }

    public static ValueTask<int> DeleteByInnerIdAsync<TEntity>(this IAppDbService<TEntity> service, Guid innerId, CancellationToken token = default)
        where TEntity : class, IAppDbEntity
    {
        return service.DeleteAsync(e => e.InnerId == innerId, token);
    }

    public static List<TEntity> ListByArchiveId<TEntity>(this IAppDbService<TEntity> service, Guid archiveId)
        where TEntity : class, IAppDbEntityHasArchive
    {
        return service.Query(query => query.Where(e => e.ArchiveId == archiveId).ToList());
    }

    public static ValueTask<List<TEntity>> ListByArchiveIdAsync<TEntity>(this IAppDbService<TEntity> service, Guid archiveId, CancellationToken token = default)
        where TEntity : class, IAppDbEntityHasArchive
    {
        return service.QueryAsync((query, token) => query.Where(e => e.ArchiveId == archiveId).ToListAsync(token), token);
    }
}