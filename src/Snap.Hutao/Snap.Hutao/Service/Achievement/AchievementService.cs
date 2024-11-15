// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.Achievement;
using System.Collections.Immutable;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IAchievementService))]
internal sealed partial class AchievementService : IAchievementService
{
    private readonly AchievementRepositoryOperation achievementDbBulkOperation;
    private readonly IAchievementRepository achievementRepository;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private AdvancedDbCollectionView<AchievementArchive>? archives;

    public async ValueTask<IAdvancedDbCollectionView<AchievementArchive>> GetArchivesAsync(CancellationToken token = default)
    {
        if (archives is null)
        {
            await taskContext.SwitchToBackgroundAsync();
            archives = new(achievementRepository.GetAchievementArchiveCollection(), serviceProvider);
        }

        return archives;
    }

    public List<AchievementView> GetAchievementViewList(AchievementArchive archive, AchievementServiceMetadataContext context)
    {
        Dictionary<AchievementId, EntityAchievement> entities = achievementRepository.GetAchievementMapByArchiveId(archive.InnerId);
        List<AchievementView> results = [];
        foreach (ref readonly Model.Metadata.Achievement.Achievement meta in context.Achievements.AsSpan())
        {
            EntityAchievement entity = entities.GetValueOrDefault(meta.Id) ?? EntityAchievement.From(archive.InnerId, meta.Id);
            results.Add(new AchievementView(entity, meta));
        }

        return results;
    }

    public void SaveAchievement(AchievementView achievement)
    {
        achievementRepository.OverwriteAchievement(achievement.Entity);
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
        achievementRepository.RemoveAchievementArchive(archive);
    }

    public async ValueTask<ImportResult> ImportFromUIAFAsync(AchievementArchive archive, ImmutableArray<UIAFItem> array, ImportStrategyKind strategy)
    {
        await taskContext.SwitchToBackgroundAsync();

        Guid archiveId = archive.InnerId;

        switch (strategy)
        {
            case ImportStrategyKind.AggressiveMerge:
                {
                    IOrderedEnumerable<UIAFItem> orederedUIAF = array.OrderBy(a => a.Id);
                    return achievementDbBulkOperation.Merge(archiveId, orederedUIAF, true);
                }

            case ImportStrategyKind.LazyMerge:
                {
                    IOrderedEnumerable<UIAFItem> orederedUIAF = array.OrderBy(a => a.Id);
                    return achievementDbBulkOperation.Merge(archiveId, orederedUIAF, false);
                }

            case ImportStrategyKind.Overwrite:
                {
                    IEnumerable<EntityAchievement> orederedUIAF = array
                        .Select(uiaf => EntityAchievement.From(archiveId, uiaf))
                        .OrderBy(a => a.Id);
                    return achievementDbBulkOperation.Overwrite(archiveId, orederedUIAF);
                }

            default:
                throw HutaoException.NotSupported();
        }
    }

    public async ValueTask<UIAF> ExportToUIAFAsync(AchievementArchive archive)
    {
        await taskContext.SwitchToBackgroundAsync();
        List<EntityAchievement> entities = achievementRepository.GetAchievementListByArchiveId(archive.InnerId);

        return new()
        {
            Info = UIAFInfo.Create(),
            List = entities.Select(UIAFItem.From).ToImmutableArray(),
        };
    }
}