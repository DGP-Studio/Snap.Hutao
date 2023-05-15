// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.InterChange.GachaLog;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// v2.1 v2.2 祈愿记录导入服务
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IUIGFImportService))]
internal sealed partial class UIGFImportService : IUIGFImportService
{
    private readonly ILogger<UIGFImportService> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public async Task<GachaArchive> ImportAsync(GachaLogServiceContext context, UIGF uigf)
    {
        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            GachaArchiveInitializationContext initContext = new(taskContext, uigf.Info.Uid, appDbContext.GachaArchives, context.ArchiveCollection);
            GachaArchive.Init(initContext, out GachaArchive? archive);
            Guid archiveId = archive.InnerId;

            long trimId = appDbContext.GachaItems
                .Where(i => i.ArchiveId == archiveId)
                .OrderBy(i => i.Id)
                .FirstOrDefault()?.Id ?? long.MaxValue;

            logger.LogInformation("Last Id to trim with: [{id}]", trimId);

            _ = uigf.IsCurrentVersionSupported(out UIGFVersion version);

            IEnumerable<GachaItem> toAdd = version switch
            {
                UIGFVersion.Major2Minor3OrHigher => uigf.List
                    .OrderByDescending(i => i.Id)
                    .Where(i => i.Id < trimId)
                    .Select(i => GachaItem.CreateForMajor2Minor3OrHigher(archiveId, i)),
                UIGFVersion.Major2Minor2OrLower => uigf.List
                    .OrderByDescending(i => i.Id)
                    .Where(i => i.Id < trimId)
                    .Select(i => GachaItem.CreateForMajor2Minor2OrLower(archiveId, i, context.GetItemId(i))),
                _ => Enumerable.Empty<GachaItem>(),
            };

            await appDbContext.GachaItems.AddRangeAndSaveAsync(toAdd).ConfigureAwait(false);
            return archive;
        }
    }
}