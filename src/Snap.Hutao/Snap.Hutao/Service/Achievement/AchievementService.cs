// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.ViewModel.Achievement;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using EntityAchievement = Snap.Hutao.Model.Entity.Achievement;

namespace Snap.Hutao.Service.Achievement;

[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IAchievementService))]
internal sealed partial class AchievementService : IAchievementService
{
    private readonly AsyncLock archivesLock = new();
    private readonly ConcurrentDictionary<Guid, Lock> viewCollectionLocks = [];
    private readonly ConcurrentDictionary<Guid, IAdvancedCollectionView<AchievementView>> viewCollectionCache = [];

    private readonly AchievementRepositoryOperation achievementDbBulkOperation;
    private readonly IAchievementRepository achievementRepository;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private IAdvancedDbCollectionView<AchievementArchive>? archives;

    public async ValueTask<IAdvancedDbCollectionView<AchievementArchive>> GetArchiveCollectionAsync(CancellationToken token = default)
    {
        using (await archivesLock.LockAsync().ConfigureAwait(false))
        {
            return archives ??= achievementRepository.GetAchievementArchiveCollection().AsAdvancedDbCollectionView(serviceProvider);
        }
    }

    public IAdvancedCollectionView<AchievementView> GetAchievementViewCollection(AchievementArchive archive, AchievementServiceMetadataContext context)
    {
        Guid innerId = archive.InnerId;
        lock (viewCollectionLocks.GetOrAdd(innerId, _ => new()))
        {
            if (viewCollectionCache.TryGetValue(innerId, out IAdvancedCollectionView<AchievementView>? viewCollection))
            {
                return viewCollection;
            }

            FrozenDictionary<AchievementId, EntityAchievement> entities = achievementRepository.GetAchievementMapByArchiveId(archive.InnerId);
            ImmutableArray<AchievementView>.Builder results = ImmutableArray.CreateBuilder<AchievementView>(context.Achievements.Length);
            foreach (ref readonly Model.Metadata.Achievement.Achievement meta in context.Achievements.AsSpan())
            {
                results.Add(AchievementView.Create(entities.GetValueOrDefault(meta.Id) ?? EntityAchievement.From(archive.InnerId, meta.Id), meta));
            }

            IAdvancedCollectionView<AchievementView> collection = results.ToImmutable().AsAdvancedCollectionView();
            viewCollectionCache.TryAdd(innerId, collection);
            return collection;
        }
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

        if (archives is null)
        {
            return ArchiveAddResultKind.ArchivesNotInitialized;
        }

        if (archives.Source.Any(a => a.Name == newArchive.Name))
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
        archives.MoveCurrentToFirst();

        // Invalidate cache
        viewCollectionCache.TryRemove(archive.InnerId, out _);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        achievementRepository.RemoveAchievementArchive(archive);
    }

    public async ValueTask<ImportResult> ImportFromUIAFAsync(AchievementArchive archive, ImmutableArray<UIAFItem> array, ImportStrategyKind strategy)
    {
        await taskContext.SwitchToBackgroundAsync();

        Guid archiveId = archive.InnerId;

        // Invalidate cache
        viewCollectionCache.TryRemove(archiveId, out _);

        switch (strategy)
        {
            case ImportStrategyKind.AggressiveMerge:
                {
                    IOrderedEnumerable<UIAFItem> orderedUIAF = array.OrderBy(a => a.Id);
                    return achievementDbBulkOperation.Merge(archiveId, orderedUIAF, true);
                }

            case ImportStrategyKind.LazyMerge:
                {
                    IOrderedEnumerable<UIAFItem> orderedUIAF = array.OrderBy(a => a.Id);
                    return achievementDbBulkOperation.Merge(archiveId, orderedUIAF, false);
                }

            case ImportStrategyKind.Overwrite:
                {
                    IEnumerable<EntityAchievement> orderedUIAF = array
                        .Select(uiaf => EntityAchievement.From(archiveId, uiaf))
                        .OrderBy(a => a.Id);
                    return achievementDbBulkOperation.Overwrite(archiveId, orderedUIAF);
                }

            default:
                throw HutaoException.NotSupported();
        }
    }

    public async ValueTask<UIAF> ExportToUIAFAsync(AchievementArchive archive)
    {
        await taskContext.SwitchToBackgroundAsync();
        ImmutableArray<EntityAchievement> entities = achievementRepository.GetAchievementImmutableArrayByArchiveId(archive.InnerId);

        return new()
        {
            Info = UIAFInfo.CreateForExport(),
            List = entities.SelectAsArray(UIAFItem.From),
        };
    }
}