// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.GachaLog;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.Service.UIGF;

internal abstract partial class AbstractUIGF40ExportService : IUIGFExportService
{
    private readonly JsonSerializerOptions jsonOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial AbstractUIGF40ExportService(IServiceProvider serviceProvider);

    protected abstract string Version { get; }

    public async ValueTask ExportAsync(UIGFExportOptions exportOptions, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();

        Model.InterChange.GachaLog.UIGF uigf = new()
        {
            Info = new()
            {
                ExportApp = "Snap Hutao",
                ExportAppVersion = $"{HutaoRuntime.Version}",
                ExportTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Version = Version,
            },
        };

        ExportGachaArchives(uigf, exportOptions.GachaArchiveUids);

        using (FileStream stream = File.Create(exportOptions.FilePath))
        {
            await JsonSerializer.SerializeAsync(stream, uigf, jsonOptions, token).ConfigureAwait(false);
        }
    }

    private void ExportGachaArchives(Model.InterChange.GachaLog.UIGF uigf, ImmutableArray<uint> uids)
    {
        if (uids.Length <= 0)
        {
            return;
        }

        IGachaLogRepository gachaLogRepository = serviceProvider.GetRequiredService<IGachaLogRepository>();

        ImmutableArray<UIGFEntry<Hk4eItem>>.Builder results = ImmutableArray.CreateBuilder<UIGFEntry<Hk4eItem>>(uids.Length);
        foreach (ref readonly uint uid in uids.AsSpan())
        {
            GachaArchive? archive = gachaLogRepository.GetGachaArchiveByUid($"{uid}");
            ArgumentNullException.ThrowIfNull(archive);
            ImmutableArray<GachaItem> dbItems = gachaLogRepository.GetGachaItemImmutableArrayByArchiveId(archive.InnerId);
            UIGFEntry<Hk4eItem> entry = new()
            {
                Uid = uid,
                TimeZone = 0,
                List = dbItems.SelectAsArray(Hk4eItem.From),
            };

            results.Add(entry);
        }

        uigf.Hk4e = results.ToImmutable();
    }
}