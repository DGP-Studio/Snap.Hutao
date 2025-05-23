// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity.Abstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Abstraction;

internal static class RepositoryAppDbEntityExtension
{
    public static int DeleteByInnerId<TEntity>(this IRepository<TEntity> repository, TEntity entity)
        where TEntity : class, IAppDbEntity
    {
        return repository.DeleteByInnerId(entity.InnerId);
    }

    public static int DeleteByInnerId<TEntity>(this IRepository<TEntity> repository, Guid innerId)
        where TEntity : class, IAppDbEntity
    {
        return repository.Delete(e => e.InnerId == innerId);
    }

    public static ImmutableArray<TEntity> ImmutableArrayByArchiveId<TEntity>(this IRepository<TEntity> repository, Guid archiveId)
        where TEntity : class, IAppDbEntityHasArchive
    {
        return repository.Query(query => query.Where(e => e.ArchiveId == archiveId).ToImmutableArray());
    }
}