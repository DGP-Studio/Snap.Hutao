// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Service.GachaLog;

internal static class GachaArchiveOperation
{
    public static GachaArchive GetOrAdd(IGachaLogRepository repository, string uid, IAdvancedDbCollectionView<GachaArchive> archives)
    {
        GachaArchive? archive = archives.SourceCollection.SingleOrDefault(a => a.Uid == uid);

        if (archive is not null)
        {
            return archive;
        }

        GachaArchive created = GachaArchive.From(uid);
        repository.AddGachaArchive(created);
        repository.ServiceProvider.GetRequiredService<ITaskContext>().InvokeOnMainThread(() => archives.Add(created));
        return created;
    }
}