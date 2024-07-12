// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录导出服务
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IUIGFExportService))]
internal sealed partial class UIGFExportService : IUIGFExportService
{
    private readonly IGachaLogDbService gachaLogDbService;
    private readonly RuntimeOptions runtimeOptions;
    private readonly CultureOptions cultureOptions;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async ValueTask<LegacyUIGF> ExportAsync(GachaLogServiceMetadataContext context, GachaArchive archive)
    {
        await taskContext.SwitchToBackgroundAsync();
        List<GachaItem> entities = gachaLogDbService.GetGachaItemListByArchiveId(archive.InnerId);
        List<LegacyUIGFItem> list = entities.SelectList(i => LegacyUIGFItem.From(i, context.GetNameQualityByItemId(i.ItemId)));

        LegacyUIGF uigf = new()
        {
            Info = LegacyUIGFInfo.From(runtimeOptions, cultureOptions, archive.Uid),
            List = list,
        };

        return uigf;
    }
}