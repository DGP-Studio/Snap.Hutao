// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.Metadata;

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
    private readonly MetadataOptions metadataOptions;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async ValueTask<UIGF> ExportAsync(GachaLogServiceMetadataContext context, GachaArchive archive)
    {
        await taskContext.SwitchToBackgroundAsync();

        List<UIGFItem> list = gachaLogDbService
            .GetGachaItemListByArchiveId(archive.InnerId)
            .SelectList(i => UIGFItem.From(i, context.GetNameQualityByItemId(i.ItemId)));

        UIGF uigf = new()
        {
            Info = UIGFInfo.From(runtimeOptions, metadataOptions, archive.Uid),
            List = list,
        };

        return uigf;
    }
}