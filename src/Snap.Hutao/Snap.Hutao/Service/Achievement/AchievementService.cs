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
    private readonly ScopedDbCurrent<AchievementArchive, Message.AchievementArchiveChangedMessage> dbCurrent;
    private readonly AchievementDbBulkOperation achievementDbBulkOperation;
    private readonly IAchievementDbService achievementDbService;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;

    private ObservableCollection<AchievementArchive>? archiveCollection;

    public AchievementArchive? CurrentArchive
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    public ObservableCollection<AchievementArchive> ArchiveCollection
    {
        get
        {
            if (archiveCollection is null)
            {
                archiveCollection = achievementDbService.GetAchievementArchiveCollection();
                CurrentArchive = archiveCollection.SelectedOrDefault();
            }

            return archiveCollection;
        }
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

        ArgumentNullException.ThrowIfNull(archiveCollection);

        if (archiveCollection.Any(a => a.Name == newArchive.Name))
        {
            return ArchiveAddResultKind.AlreadyExists;
        }

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        archiveCollection.Add(newArchive);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        CurrentArchive = newArchive;

        return ArchiveAddResultKind.Added;
    }

    public async ValueTask RemoveArchiveAsync(AchievementArchive archive)
    {
        ArgumentNullException.ThrowIfNull(archiveCollection);

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        archiveCollection.Remove(archive);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        await achievementDbService.RemoveAchievementArchiveAsync(archive).ConfigureAwait(false);
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
        List<EntityAchievement> entities = await achievementDbService
            .GetAchievementListByArchiveIdAsync(archive.InnerId)
            .ConfigureAwait(false);
        List<UIAFItem> list = entities.SelectList(UIAFItem.From);

        return new()
        {
            Info = UIAFInfo.From(runtimeOptions),
            List = list,
        };
    }
}