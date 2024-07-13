// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.UIGF;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IUIGFImportService), Key = UIGFVersion.UIGF40)]
internal sealed partial class UIGF40ImportService : IUIGFImportService
{
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;

    public async ValueTask<bool> ImportAsync(UIGFImportOptions importOptions, CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        ImportGachaArchives(importOptions.UIGF.Hk4e, importOptions.GachaArchiveUids);
        ImportAchievementArchives(importOptions.UIGF.HutaoReserved?.Achievement, importOptions.ReservedAchievementArchiveIdentities);

        return true;
    }

    private void ImportGachaArchives(List<UIGFEntry<Hk4eItem>>? entries, HashSet<string> uids)
    {
        if (entries.IsNullOrEmpty() || uids.IsNullOrEmpty())
        {
            return;
        }

        IGachaLogDbService gachaLogDbService = serviceProvider.GetRequiredService<IGachaLogDbService>();

        foreach (UIGFEntry<Hk4eItem> entry in entries)
        {
            if (!uids.Contains(entry.Uid))
            {
                continue;
            }

            GachaArchive? archive = gachaLogDbService.GetGachaArchiveByUid(entry.Uid);

            if (archive is null)
            {
                archive = GachaArchive.From(entry.Uid);
                gachaLogDbService.AddGachaArchive(archive);
            }

            Guid archiveId = archive.InnerId;

            List<GachaItem> fullItems = [];
            foreach (GachaType queryType in GachaLog.GachaLog.QueryTypes)
            {
                long trimId = gachaLogDbService.GetOldestGachaItemIdByArchiveIdAndQueryType(archiveId, queryType);
                List<GachaItem> currentTypedList = entry.List
                    .Where(item => item.UIGFGachaType == queryType && item.Id > trimId)
                    .OrderByDescending(item => item.Id)
                    .Select(item => GachaItem.From(archiveId, item, entry.TimeZone))
                    .ToList();

                fullItems.AddRange(currentTypedList);
            }

            gachaLogDbService.AddGachaItemRange(fullItems);
        }
    }

    private void ImportAchievementArchives(List<HutaoReservedEntry<HutaoReservedAchievement>>? entries, HashSet<string> identities)
    {
        if (entries.IsNullOrEmpty() || identities.IsNullOrEmpty())
        {
            return;
        }

        IAchievementDbService achievementDbService = serviceProvider.GetRequiredService<IAchievementDbService>();
        AchievementDbBulkOperation achievementDbBulkOperation = serviceProvider.GetRequiredService<AchievementDbBulkOperation>();

        foreach (HutaoReservedEntry<HutaoReservedAchievement> entry in entries)
        {
            if (!identities.Contains(entry.Identity))
            {
                continue;
            }

            AchievementArchive? archive = achievementDbService.GetAchievementArchiveByName(entry.Identity);

            if (archive is null)
            {
                archive = AchievementArchive.From(entry.Identity);
                achievementDbService.AddAchievementArchive(archive);
            }

            Guid archiveId = archive.InnerId;

            List<Model.Entity.Achievement> achievements = entry.List.SelectList(item => Model.Entity.Achievement.From(archiveId, item));
            achievementDbBulkOperation.Overwrite(archiveId, achievements);
        }
    }
}
