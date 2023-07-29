// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.InterChange.GachaLog;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录导出服务
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IUIGFExportService))]
internal sealed partial class UIGFExportService : IUIGFExportService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IGachaLogDbService gachaLogDbService;
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
            Info = UIGFInfo.From(serviceProvider, archive.Uid),
            List = list,
        };

        return uigf;
    }
}