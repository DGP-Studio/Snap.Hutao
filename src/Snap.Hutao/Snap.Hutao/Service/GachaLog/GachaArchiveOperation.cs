// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Service.GachaLog;

internal static class GachaArchiveOperation
{
    public static void GetOrAdd(IGachaLogRepository repository, ITaskContext taskContext, string uid, AdvancedDbCollectionView<GachaArchive> archives, [NotNull] out GachaArchive? archive)
    {
        archive = archives.SourceCollection.SingleOrDefault(a => a.Uid == uid);

        if (archive is not null)
        {
            return;
        }

        GachaArchive created = GachaArchive.From(uid);
        repository.AddGachaArchive(created);
        taskContext.InvokeOnMainThread(() => archives.Add(created));
        archive = created;
    }
}