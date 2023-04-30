// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Model.Metadata.Abstraction;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录导出服务
/// </summary>
[Injection(InjectAs.Scoped, typeof(IGachaLogExportService))]
internal sealed class GachaLogExportService : IGachaLogExportService
{
    private readonly ITaskContext taskContext;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// 构造一个新的祈愿记录导出服务
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public GachaLogExportService(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public async Task<UIGF> ExportToUIGFAsync(GachaLogServiceContext context, GachaArchive archive)
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