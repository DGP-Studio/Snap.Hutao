// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange;
using Snap.Hutao.Service.GachaLog;
using System.IO;

namespace Snap.Hutao.Service.UIGF;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IUIGFExportService), Key = UIGFVersion.UIGF40)]
internal sealed partial class UIGF40ExportService : IUIGFExportService
{
    private readonly IGachaLogDbService gachaLogDbService;
    private readonly JsonSerializerOptions jsonOptions;
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

        using (FileStream stream = File.Create(exportOptions.FilePath))
        {
            await JsonSerializer.SerializeAsync(stream, uigf, jsonOptions, token).ConfigureAwait(false);
        }

        return true;
    }

    private void ExportGachaArchives(Model.InterChange.UIGF uigf, List<Guid> archiveIds)
    {
        List<UIGFEntry<Hk4eItem>> hk4eEntries = [];
        foreach (Guid archiveId in archiveIds)
        {
            GachaArchive? archive = gachaLogDbService.GetGachaArchiveById(archiveId);
            ArgumentNullException.ThrowIfNull(archive);
            List<GachaItem> dbItems = gachaLogDbService.GetGachaItemListByArchiveId(archiveId);
            UIGFEntry<Hk4eItem> entry = new()
            {
                Uid = archive.Uid,
                TimeZone = 0,
                List = [.. dbItems.Select(Hk4eItem.From)],
            };

            hk4eEntries.Add(entry);
        }

        uigf.Hk4e = hk4eEntries;
    }
}