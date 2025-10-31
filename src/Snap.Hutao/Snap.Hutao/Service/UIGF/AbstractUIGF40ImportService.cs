// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.UIGF;

internal abstract partial class AbstractUIGF40ImportService : IUIGFImportService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial AbstractUIGF40ImportService(IServiceProvider serviceProvider);

    public async ValueTask ImportAsync(UIGFImportOptions importOptions, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();
        ImportGachaArchives(importOptions.UIGF.Hk4e, importOptions.GachaArchiveUids);
    }

    private void ImportGachaArchives(ImmutableArray<UIGFEntry<Hk4eItem>> entries, HashSet<uint> uids)
    {
        if (entries.IsDefaultOrEmpty || uids is null or { Count: 0 })
        {
            return;
        }

        IGachaLogRepository gachaLogRepository = serviceProvider.GetRequiredService<IGachaLogRepository>();

        foreach (ref readonly UIGFEntry<Hk4eItem> entry in entries.AsSpan())
        {
            if (!uids.Contains(entry.Uid))
            {
                continue;
            }

            foreach (Hk4eItem item in entry.List)
            {
                if (item.ItemId is 0)
                {
                    throw HutaoException.Throw(SH.FormatServiceUIGFImportInvalidItem(item.Id));
                }
            }

            GachaArchive? archive = gachaLogRepository.GetGachaArchiveByUid($"{entry.Uid}");

            if (archive is null)
            {
                archive = GachaArchive.Create($"{entry.Uid}");
                gachaLogRepository.AddGachaArchive(archive);
            }

            Guid archiveId = archive.InnerId;

            List<GachaItem> fullItems = [];
            int timeZone = entry.TimeZone;
            foreach (GachaType queryType in GachaLog.GachaLog.QueryTypes)
            {
                long trimId = gachaLogRepository.GetOldestGachaItemIdByArchiveIdAndQueryType(archiveId, queryType);
                List<GachaItem> currentTypedList = entry.List
                    .Where(item => item.UIGFGachaType == queryType && item.Id < trimId)
                    .OrderByDescending(item => item.Id)
                    .Select(item => GachaItem.From(archiveId, item, timeZone))
                    .ToList();

                fullItems.AddRange(currentTypedList);
            }

            gachaLogRepository.AddGachaItemRange(fullItems);
        }
    }
}