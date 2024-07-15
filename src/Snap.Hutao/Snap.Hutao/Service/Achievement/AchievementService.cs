// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Achievement;
using System.Collections.ObjectModel;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IAchievementService))]
internal sealed partial class AchievementService : IAchievementService
{
    private readonly AchievementDbBulkOperation achievementDbBulkOperation;
    private readonly IAchievementDbService achievementDbService;
    private readonly IServiceProvider serviceProvider;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;

    private AdvancedDbCollectionView<AchievementArchive>? archives;

    public async ValueTask<IAdvancedDbCollectionView<AchievementArchive>> GetArchivesAsync(CancellationToken token = default)
    {
        if (archives is null)
        {
            await taskContext.SwitchToBackgroundAsync();
            archives = new(achievementDbService.GetAchievementArchiveCollection(), serviceProvider);
        }

        return archives;
    }

    public List<AchievementView> GetAchievementViewList(AchievementArchive archive, AchievementServiceMetadataContext context)
    {
        Dictionary<AchievementId, EntityAchievement> entities = achievementDbService.GetAchievementMapByArchiveId(archive.InnerId);

        return context.Achievements.SelectList(meta =>
        {
            EntityAchievement entity = entities.GetValueOrDefault(meta.Id) ?? EntityAchievement.From(archive.InnerId, meta.Id);
            return new AchievementView(entity, meta);
        });
    }

    public void SaveAchievement(AchievementView achievement)
    {
        achievementDbService.OverwriteAchievement(achievement.Entity);
    }

    public async ValueTask<ArchiveAddResultKind> AddArchiveAsync(AchievementArchive newArchive)
    {
        if (string.IsNullOrWhiteSpace(newArchive.Name))
        {
            return ArchiveAddResultKind.InvalidName;
        }

        ArgumentNullException.ThrowIfNull(archives);

        if (archives.SourceCollection.Any(a => a.Name == newArchive.Name))
        {
            return ArchiveAddResultKind.AlreadyExists;
        }

        await taskContext.SwitchToMainThreadAsync();
        archives.Add(newArchive);
        archives.MoveCurrentTo(newArchive);

        return ArchiveAddResultKind.Added;
    }

    public async ValueTask RemoveArchiveAsync(AchievementArchive archive)
    {
        ArgumentNullException.ThrowIfNull(archives);

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        archives.Remove(archive);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        achievementDbService.RemoveAchievementArchive(archive);
    }

    public async ValueTask<ImportResult> ImportFromUIAFAsync(AchievementArchive archive, List<UIAFItem> list, ImportStrategyKind strategy)
    {
        await taskContext.SwitchToBackgroundAsync();

        Guid archiveId = archive.InnerId;

        switch (strategy)
        {
            case ImportStrategyKind.AggressiveMerge:
                {
                    IOrderedEnumerable<UIAFItem> orederedUIAF = list.OrderBy(a => a.Id);
                    return achievementDbBulkOperation.Merge(archiveId, orederedUIAF, true);
                }

            case ImportStrategyKind.LazyMerge:
                {
                    IOrderedEnumerable<UIAFItem> orederedUIAF = list.OrderBy(a => a.Id);
                    return achievementDbBulkOperation.Merge(archiveId, orederedUIAF, false);
                }

            case ImportStrategyKind.Overwrite:
                {
                    IEnumerable<EntityAchievement> orederedUIAF = list
                        .SelectList(uiaf => EntityAchievement.From(archiveId, uiaf))
                        .SortBy(a => a.Id);
                    return achievementDbBulkOperation.Overwrite(archiveId, orederedUIAF);
                }

            default:
                throw HutaoException.NotSupported();
        }
    }

    public async ValueTask<UIAF> ExportToUIAFAsync(AchievementArchive archive)
    {
        await taskContext.SwitchToBackgroundAsync();
        List<EntityAchievement> entities = achievementDbService.GetAchievementListByArchiveId(archive.InnerId);
        List<UIAFItem> list = entities.SelectList(UIAFItem.From);

        return new()
        {
            Info = UIAFInfo.From(runtimeOptions),
            List = list,
        };
    }
}