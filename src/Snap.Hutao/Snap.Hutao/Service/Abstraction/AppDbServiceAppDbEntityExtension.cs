// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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

    public static List<TEntity> ListByArchiveId<TEntity>(this IAppDbService<TEntity> service, Guid archiveId)
        where TEntity : class, IAppDbEntityHasArchive
    {
        return service.Query(query => query.Where(e => e.ArchiveId == archiveId).ToList());
    }
}