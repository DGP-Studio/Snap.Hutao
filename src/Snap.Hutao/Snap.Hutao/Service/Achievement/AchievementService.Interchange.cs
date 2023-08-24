// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.Achievement;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

/// <summary>
/// 数据交换部分
/// </summary>
internal sealed partial class AchievementService
{
    /// <inheritdoc/>
    public async ValueTask<ImportResult> ImportFromUIAFAsync(AchievementArchive archive, List<UIAFItem> list, ImportStrategy strategy)
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
                        .Select(uiaf => EntityAchievement.From(archiveId, uiaf))
                        .OrderBy(a => a.Id);
                    return achievementDbBulkOperation.Overwrite(archiveId, orederedUIAF);
                }

            default:
                throw Must.NeverHappen();
        }
    }

    /// <inheritdoc/>
    public async ValueTask<UIAF> ExportToUIAFAsync(AchievementArchive archive)
    {
        await taskContext.SwitchToBackgroundAsync();
        List<UIAFItem> list = achievementDbService
            .GetAchievementListByArchiveId(archive.InnerId)
            .SelectList(UIAFItem.From);

        return new()
        {
            Info = UIAFInfo.From(runtimeOptions),
            List = list,
        };
    }
}
