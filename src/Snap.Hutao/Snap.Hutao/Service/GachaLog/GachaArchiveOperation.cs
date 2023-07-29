// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿存档初始化上下文
/// </summary>
internal static class GachaArchiveOperation
{
    public static void GetOrAdd(IServiceProvider serviceProvider, string uid, ObservableCollection<GachaArchive> archives, [NotNull] out GachaArchive? archive)
    {
        archive = archives.SingleOrDefault(a => a.Uid == uid);

        if (archive == null)
        {
            GachaArchive created = GachaArchive.From(uid);
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                IGachaLogDbService gachaLogDbService = scope.ServiceProvider.GetRequiredService<IGachaLogDbService>();
                gachaLogDbService.AddGachaArchive(created);

                ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
                taskContext.InvokeOnMainThread(() => archives.Add(created));
            }

            archive = created;
        }
    }
}