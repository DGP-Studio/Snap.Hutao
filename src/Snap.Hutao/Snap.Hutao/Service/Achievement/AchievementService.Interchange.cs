// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.InterChange.Achievement;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 数据交换部分
/// </summary>
internal sealed partial class AchievementService
{
    /// <inheritdoc/>
    public async Task<ImportResult> ImportFromUIAFAsync(AchievementArchive archive, List<UIAFItem> list, ImportStrategy strategy)
    {
        await taskContext.SwitchToBackgroundAsync();

        Guid archiveId = archive.InnerId;

        switch (strategy)
        {
            case ImportStrategy.AggressiveMerge:
                {
                    IOrderedEnumerable<UIAFItem> orederedUIAF = list.OrderBy(a => a.Id);
                    return achievementDbBulkOperation.Merge(archiveId, orederedUIAF, true);
                }

            case ImportStrategy.LazyMerge:
                {
                    IOrderedEnumerable<UIAFItem> orederedUIAF = list.OrderBy(a => a.Id);
                    return achievementDbBulkOperation.Merge(archiveId, orederedUIAF, false);
                }

            case ImportStrategy.Overwrite:
                {
                    IEnumerable<EntityAchievement> orederedUIAF = list
                        .Select(uiaf => EntityAchievement.Create(archiveId, uiaf))
                        .OrderBy(a => a.Id);
                    return achievementDbBulkOperation.Overwrite(archiveId, orederedUIAF);
                }

            default:
                throw Must.NeverHappen();
        }
    }

    /// <inheritdoc/>
    public async Task<UIAF> ExportToUIAFAsync(AchievementArchive archive)
    {
        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            List<UIAFItem> list = appDbContext.Achievements
                .Where(i => i.ArchiveId == archive.InnerId)
                .AsEnumerable()
                .Select(i => i.ToUIAFItem())
                .ToList();

            return new()
            {
                Info = UIAFInfo.Create(serviceProvider),
                List = list,
            };
        }
    }
}
