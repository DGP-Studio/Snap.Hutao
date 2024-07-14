// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.GachaLog;
using System.IO;

namespace Snap.Hutao.Service.UIGF;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IUIGFExportService), Key = UIGFVersion.UIGF40)]
internal sealed partial class UIGF40ExportService : IUIGFExportService
{
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;

    public async ValueTask ExportAsync(UIGFExportOptions exportOptions, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();

        Model.InterChange.GachaLog.UIGF uigf = new()
        {
            Info = new()
            {
                ExportApp = "Snap Hutao",
                ExportAppVersion = $"{runtimeOptions.Version}",
                ExportTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Version = "v4.0",
            },
        };

        ExportGachaArchives(uigf, exportOptions.GachaArchiveIds);

        using (FileStream stream = File.Create(exportOptions.FilePath))
        {
            await JsonSerializer.SerializeAsync(stream, uigf, jsonOptions, token).ConfigureAwait(false);
        }
    }

    private void ExportGachaArchives(Model.InterChange.GachaLog.UIGF uigf, List<Guid> archiveIds)
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
}
