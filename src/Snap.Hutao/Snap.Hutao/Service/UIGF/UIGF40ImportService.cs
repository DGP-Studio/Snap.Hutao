// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.UIGF;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IUIGFImportService), Key = UIGFVersion.UIGF40)]
internal sealed partial class UIGF40ImportService : IUIGFImportService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    public async ValueTask ImportAsync(UIGFImportOptions importOptions, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();
        ImportGachaArchives(importOptions.UIGF.Hk4e, importOptions.GachaArchiveUids);
    }

    private void ImportGachaArchives(List<UIGFEntry<Hk4eItem>>? entries, HashSet<uint> uids)
    {
        if (entries is null or [] || uids is null or { Count: 0 })
        {
            return;
        }

        IGachaLogRepository gachaLogRepository = serviceProvider.GetRequiredService<IGachaLogRepository>();

        foreach (UIGFEntry<Hk4eItem> entry in entries)
        {
            if (!uids.Contains(entry.Uid))
            {
                continue;
            }

            GachaArchive? archive = gachaLogRepository.GetGachaArchiveByUid($"{entry.Uid}");

            if (archive is null)
            {
                archive = GachaArchive.From($"{entry.Uid}");
                gachaLogRepository.AddGachaArchive(archive);
            }

            Guid archiveId = archive.InnerId;

            List<GachaItem> fullItems = [];
            foreach (GachaType queryType in GachaLog.GachaLog.QueryTypes)
            {
                long trimId = gachaLogRepository.GetNewestGachaItemIdByArchiveIdAndQueryType(archiveId, queryType);
                List<GachaItem> currentTypedList = entry.List
                    .Where(item => item.UIGFGachaType == queryType && item.Id > trimId)
                    .OrderByDescending(item => item.Id)
                    .Select(item => GachaItem.From(archiveId, item, entry.TimeZone))
                    .ToList();

                fullItems.AddRange(currentTypedList);
            }

            gachaLogRepository.AddGachaItemRange(fullItems);
        }
    }
}