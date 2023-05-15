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
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async Task<UIGF> ExportAsync(GachaLogServiceContext context, GachaArchive archive)
    {
        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            List<UIGFItem> list = appDbContext.GachaItems
                .Where(i => i.ArchiveId == archive.InnerId)
                .AsEnumerable()
                .Select(i => i.ToUIGFItem(context.GetNameQualityByItemId(i.ItemId)))
                .ToList();

            UIGF uigf = new()
            {
                Info = UIGFInfo.Create(serviceProvider, archive.Uid),
                List = list,
            };

            return uigf;
        }
    }
}