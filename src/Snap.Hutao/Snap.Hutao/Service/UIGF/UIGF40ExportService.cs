// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.AvatarInfo;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.SpiralAbyss;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.UIGF;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IUIGFExportService), Key = UIGFVersion.UIGF40)]
internal sealed partial class UIGF40ExportService : IUIGFExportService
{
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;

    public async ValueTask<bool> ExportAsync(UIGFExportOptions exportOptions, CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();

        Model.InterChange.UIGF uigf = new()
        {
            Info = new()
            {
                ExportApp = "Snap Hutao",
                ExportAppVersion = $"{runtimeOptions.Version}",
                ExportTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Version = "v4.0",
            },
            HutaoReserved = new()
            {
                Version = 1,
            },
        };

        ExportGachaArchives(uigf, exportOptions.GachaArchiveIds);
        ExportAchievementArchives(uigf.HutaoReserved, exportOptions.ReservedAchievementArchiveIds);
        ExportAvatarInfoUids(uigf.HutaoReserved, exportOptions.ReservedAvatarInfoUids);
        ExportCultivationProjects(uigf.HutaoReserved, exportOptions.ReservedCultivationProjectIds);
        ExportSpialAbysses(uigf.HutaoReserved, exportOptions.ReservedSpiralAbyssUids);

        using (FileStream stream = File.Create(exportOptions.FilePath))
        {
            await JsonSerializer.SerializeAsync(stream, uigf, jsonOptions, token).ConfigureAwait(false);
        }

        return true;
    }

    private void ExportGachaArchives(Model.InterChange.UIGF uigf, List<Guid> archiveIds)
    {
        if (archiveIds.Count <= 0)
        {
            return;
        }

        IGachaLogDbService gachaLogDbService = serviceProvider.GetRequiredService<IGachaLogDbService>();

        List<UIGFEntry<Hk4eItem>> results = [];
        foreach (Guid archiveId in archiveIds)
        {
            GachaArchive? archive = gachaLogDbService.GetGachaArchiveById(archiveId);
            ArgumentNullException.ThrowIfNull(archive);
            List<GachaItem> dbItems = gachaLogDbService.GetGachaItemListByArchiveId(archiveId);
            UIGFEntry<Hk4eItem> entry = new()
            {
                Uid = archive.Uid,
                TimeZone = 0,
                List = dbItems.SelectList(Hk4eItem.From),
            };

            results.Add(entry);
        }

        uigf.Hk4e = results;
    }

    private void ExportAchievementArchives(HutaoReserved hutaoReserved, List<Guid> archiveIds)
    {
        if (archiveIds.Count <= 0)
        {
            return;
        }

        IAchievementDbService achievementDbService = serviceProvider.GetRequiredService<IAchievementDbService>();

        List<HutaoReservedEntry<HutaoReservedAchievement>> results = [];
        foreach (Guid archiveId in archiveIds)
        {
            AchievementArchive? archive = achievementDbService.GetAchievementArchiveById(archiveId);
            ArgumentNullException.ThrowIfNull(archive);
            List<Model.Entity.Achievement> dbItems = achievementDbService.GetAchievementListByArchiveId(archiveId);
            HutaoReservedEntry<HutaoReservedAchievement> entry = new()
            {
                Identity = archive.Name,
                List = dbItems.SelectList(HutaoReservedAchievement.From),
            };

            results.Add(entry);
        }

        hutaoReserved.Achievement = results;
    }

    private void ExportAvatarInfoUids(HutaoReserved hutaoReserved, List<string> uids)
    {
        if (uids.Count <= 0)
        {
            return;
        }

        IAvatarInfoDbService avatarInfoDbService = serviceProvider.GetRequiredService<IAvatarInfoDbService>();

        List<HutaoReservedEntry<Web.Enka.Model.AvatarInfo>> results = [];
        foreach (string uid in uids)
        {
            List<Model.Entity.AvatarInfo>? dbItems = avatarInfoDbService.GetAvatarInfoListByUid(uid);
            HutaoReservedEntry<Web.Enka.Model.AvatarInfo> entry = new()
            {
                Identity = uid,
                List = dbItems.SelectList(item => item.Info),
            };

            results.Add(entry);
        }

        hutaoReserved.AvatarInfo = results;
    }

    private void ExportCultivationProjects(HutaoReserved hutaoReserved, List<Guid> projectIds)
    {
        if (projectIds.Count <= 0)
        {
            return;
        }

        ICultivationDbService cultivationDbService = serviceProvider.GetRequiredService<ICultivationDbService>();

        List<HutaoReservedEntry<HutaoReservedCultivationEntry>> results = [];
        foreach (Guid projectId in projectIds)
        {
            CultivateProject? project = cultivationDbService.GetCultivateProjectById(projectId);
            ArgumentNullException.ThrowIfNull(project);
            List<CultivateEntry> entries = cultivationDbService.GetCultivateEntryListIncludingLevelInformationByProjectId(projectId);

            List<HutaoReservedCultivationEntry> innerResults = [];
            foreach (ref readonly CultivateEntry innerEntry in CollectionsMarshal.AsSpan(entries))
            {
                List<CultivateItem> items = cultivationDbService.GetCultivateItemListByEntryId(innerEntry.InnerId);

                HutaoReservedCultivationEntry innerResultEntry = new()
                {
                    AvatarLevelFrom = innerEntry.LevelInformation?.AvatarLevelFrom ?? 0,
                    AvatarLevelTo = innerEntry.LevelInformation?.AvatarLevelTo ?? 0,
                    SkillALevelFrom = innerEntry.LevelInformation?.SkillALevelFrom ?? 0,
                    SkillALevelTo = innerEntry.LevelInformation?.SkillALevelTo ?? 0,
                    SkillELevelFrom = innerEntry.LevelInformation?.SkillELevelFrom ?? 0,
                    SkillELevelTo = innerEntry.LevelInformation?.SkillELevelTo ?? 0,
                    SkillQLevelFrom = innerEntry.LevelInformation?.SkillQLevelFrom ?? 0,
                    SkillQLevelTo = innerEntry.LevelInformation?.SkillQLevelTo ?? 0,
                    WeaponLevelFrom = innerEntry.LevelInformation?.WeaponLevelFrom ?? 0,
                    WeaponLevelTo = innerEntry.LevelInformation?.WeaponLevelTo ?? 0,
                    Type = innerEntry.Type,
                    Id = innerEntry.Id,
                    Items = items.SelectList(HutaoReservedCultivationItem.From),
                };

                innerResults.Add(innerResultEntry);
            }

            HutaoReservedEntry<HutaoReservedCultivationEntry> outerEntry = new()
            {
                Identity = project.Name,
                List = innerResults,
            };

            results.Add(outerEntry);
        }

        hutaoReserved.Cultivation = results;
    }

    private void ExportSpialAbysses(HutaoReserved hutaoReserved, List<string> uids)
    {
        if (uids.Count <= 0)
        {
            return;
        }

        ISpiralAbyssRecordDbService spiralAbyssRecordDbService = serviceProvider.GetRequiredService<ISpiralAbyssRecordDbService>();

        List<HutaoReservedEntry<HutaoReservedSpiralAbyssEntry>> results = [];
        foreach (string uid in uids)
        {
            Dictionary<uint, SpiralAbyssEntry> dbMap = spiralAbyssRecordDbService.GetSpiralAbyssEntryMapByUid(uid);
            HutaoReservedEntry<HutaoReservedSpiralAbyssEntry> entry = new()
            {
                Identity = uid,
                List = dbMap.Select(item => new HutaoReservedSpiralAbyssEntry
                {
                    ScheduleId = item.Key,
                    SpiralAbyss = item.Value.SpiralAbyss,
                }).ToList(),
            };

            results.Add(entry);
        }

        hutaoReserved.SpiralAbyss = results;
    }
}

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IUIGFImportService), Key = UIGFVersion.UIGF40)]
internal sealed partial class UIGF40ImportService : IUIGFImportService
{
    public async ValueTask<bool> ImportAsync(UIGFImportOptions importOptions, CancellationToken token)
    {
        await Task.CompletedTask;

        return true;
    }
}

internal interface IUIGFImportService
{

}