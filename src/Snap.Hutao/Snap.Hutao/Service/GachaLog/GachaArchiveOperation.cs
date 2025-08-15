// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Service.GachaLog;

internal static class GachaArchiveOperation
{
    public static GachaArchive GetOrAdd(IGachaLogRepository repository, string uid, IAdvancedDbCollectionView<GachaArchive> archives)
    {
        GachaArchive? archive = archives.Source.SingleOrDefault(a => a.Uid == uid);

        if (archive is not null)
        {
            return archive;
        }

        GachaArchive created = GachaArchive.Create(uid);
        repository.AddGachaArchive(created);
        CollectionViewAddGachaArchive(archives, created, repository.ServiceProvider.GetRequiredService<ITaskContext>());
        return created;
    }

    private static void CollectionViewAddGachaArchive(IAdvancedDbCollectionView<GachaArchive> archives, GachaArchive archive, ITaskContext taskContext)
    {
        taskContext.InvokeOnMainThread(() => archives.Add(archive));
    }
}