// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Service.Cultivation.Consumption;
using Snap.Hutao.Service.Inventory;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using ModelItem = Snap.Hutao.Model.Item;

namespace Snap.Hutao.Service.Cultivation;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(ICultivationService))]
internal sealed partial class CultivationService : ICultivationService
{
    private readonly ConcurrentDictionary<Guid, ObservableCollection<CultivateEntryView>> entryCollectionCache = [];
    private readonly AsyncLock entryCollectionLock = new();

    private readonly ICultivationRepository cultivationRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private AdvancedDbCollectionView<CultivateProject>? projects;

    public AdvancedDbCollectionView<CultivateProject> Projects
    {
        get => projects ??= new(cultivationRepository.GetCultivateProjectCollection(), serviceProvider);
    }

    public async ValueTask<ObservableCollection<CultivateEntryView>> GetCultivateEntryCollectionAsync(CultivateProject cultivateProject, ICultivationMetadataContext context)
    {
        using (await entryCollectionLock.LockAsync().ConfigureAwait(false))
        {
            if (entryCollectionCache.TryGetValue(cultivateProject.InnerId, out ObservableCollection<CultivateEntryView>? collection))
            {
                return collection;
            }

            await taskContext.SwitchToBackgroundAsync();
            List<CultivateEntry> entries = cultivationRepository.GetCultivateEntryListIncludingLevelInformationByProjectId(cultivateProject.InnerId);

            List<CultivateEntryView> resultEntries = new(entries.Count);
            foreach (CultivateEntry entry in entries)
            {
                List<CultivateItemView> entryItems = [];

                foreach (CultivateItem cultivateItem in cultivationRepository.GetCultivateItemListByEntryId(entry.InnerId))
                {
                    entryItems.Add(new(cultivateItem, context.GetMaterial(cultivateItem.ItemId)));
                }

                ModelItem item = entry.Type switch
                {
                    CultivateType.AvatarAndSkill => context.GetAvatar(entry.Id).ToItem<ModelItem>(),
                    CultivateType.Weapon => context.GetWeapon(entry.Id).ToItem<ModelItem>(),

                    // TODO: support furniture calc
                    _ => default!,
                };

                resultEntries.Add(new(entry, item, entryItems));
            }

            ObservableCollection<CultivateEntryView> result = resultEntries.SortByDescending(e => e.IsToday).ToObservableCollection();
            entryCollectionCache.TryAdd(cultivateProject.InnerId, result);
            return result;
        }
    }

    public async ValueTask<ObservableCollection<StatisticsCultivateItem>> GetStatisticsCultivateItemCollectionAsync(CultivateProject cultivateProject, ICultivationMetadataContext context, CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        List<StatisticsCultivateItem> resultItems = [];

        Guid projectId = cultivateProject.InnerId;

        foreach (CultivateEntry entry in cultivationRepository.GetCultivateEntryListByProjectId(projectId))
        {
            foreach (CultivateItem item in cultivationRepository.GetCultivateItemListByEntryId(entry.InnerId))
            {
                if (item.IsFinished)
                {
                    continue;
                }

                if (resultItems.SingleOrDefault(i => i.Inner.Id == item.ItemId) is { } existedItem)
                {
                    existedItem.Count += (int)item.Count;
                }
                else
                {
                    resultItems.Add(new(context.GetMaterial(item.ItemId), item));
                }
            }
        }

        foreach (InventoryItem inventoryItem in inventoryRepository.GetInventoryItemListByProjectId(projectId))
        {
            if (resultItems.SingleOrDefault(i => i.Inner.Id == inventoryItem.ItemId) is { } existedItem)
            {
                existedItem.TotalCount += (int)inventoryItem.Count;
            }
        }

        return resultItems.SortBy(item => item.Inner.Id, MaterialIdComparer.Shared).ToObservableCollection();
    }

    public async ValueTask<ResinStatistics> GetResinStatisticsAsync(ObservableCollection<StatisticsCultivateItem> statisticsCultivateItems, CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        ResinStatistics statistics = new();

        IEnumerable<IGrouping<uint, StatisticsCultivateItem>> groupedItems = statisticsCultivateItems
            .Where(i => i.Inner.IsResinItem())
            .GroupBy(i => i.Inner.Rank);

        foreach ((uint rank, IEnumerable<StatisticsCultivateItem> items) in groupedItems)
        {
            // 摩拉
            if (rank is 10U)
            {
                StatisticsCultivateItem item = items.Single();
                if (item.IsFinished)
                {
                    continue;
                }

                double times = item.Count - item.TotalCount;
                statistics.BlossomOfWealth.RawItemCount += times;
                continue;
            }

            if (rank is 100U)
            {
                StatisticsCultivateItem item = items.Single();
                if (item.IsFinished)
                {
                    continue;
                }

                double times = (item.Count - item.TotalCount) * 20000D;
                statistics.BlossomOfRevelation.RawItemCount += times;
                continue;
            }

            if (rank is 11101U)
            {
                foreach (StatisticsCultivateItem item in items)
                {
                    if (item.IsFinished)
                    {
                        continue;
                    }

                    double times = item.Count - item.TotalCount;
                    switch (item.Inner.RankLevel)
                    {
                        case QualityType.QUALITY_PURPLE:
                            statistics.NormalBoss.RawItemCount += times;
                            continue;
                        case QualityType.QUALITY_ORANGE:
                            statistics.WeeklyBoss.RawItemCount += times;
                            continue;
                        default:
                            throw HutaoException.NotSupported();
                    }
                }

                continue;
            }

            // 天赋书，武器突破材料。一次循环最多为4个
            double greenItems = 0D;
            double blueItems = 0D;
            double purpleItems = 0D;
            double orangeItems = 0D;

            ResinStatisticsItem targetStatisticsItem = ((rank / 1000) % 10) switch
            {
                3 => statistics.TalentBooks,
                5 => statistics.WeaponAscension,
                _ => throw HutaoException.NotSupported(),
            };

            foreach (StatisticsCultivateItem item in items)
            {
                switch (item.Inner.RankLevel)
                {
                    case QualityType.QUALITY_GREEN:
                        greenItems += item.Count - item.TotalCount;
                        continue;
                    case QualityType.QUALITY_BLUE:
                        blueItems += item.Count - item.TotalCount;
                        continue;
                    case QualityType.QUALITY_PURPLE:
                        purpleItems += item.Count - item.TotalCount;
                        continue;
                    case QualityType.QUALITY_ORANGE:
                        orangeItems += item.Count - item.TotalCount;
                        continue;
                    default:
                        throw HutaoException.NotSupported();
                }
            }

            blueItems *= 3D;
            purpleItems *= 9D;
            orangeItems *= 27D;

            // TODO: Refactor this
            if (orangeItems > 0)
            {
                if (purpleItems > 0)
                {
                    if (blueItems > 0)
                    {
                        if (greenItems > 0)
                        {
                            targetStatisticsItem.RawItemCount += orangeItems + purpleItems + blueItems + greenItems;
                        }
                        else
                        {
                            double need = orangeItems + purpleItems + blueItems;
                            double green = -greenItems > need ? need : -greenItems;
                            targetStatisticsItem.RawItemCount += orangeItems + purpleItems + blueItems - green;
                        }
                    }
                    else
                    {
                        double orangeAndPurpleNeed = orangeItems + purpleItems;
                        if (greenItems > 0)
                        {
                            double blue = -blueItems > orangeAndPurpleNeed ? orangeAndPurpleNeed : -blueItems;
                            targetStatisticsItem.RawItemCount += orangeAndPurpleNeed + greenItems - blue;
                        }
                        else
                        {
                            double blueAndGreen = -(blueItems + greenItems) > orangeAndPurpleNeed ? orangeAndPurpleNeed : -(blueItems + greenItems);
                            targetStatisticsItem.RawItemCount += orangeItems + purpleItems - blueAndGreen;
                        }
                    }
                }
                else
                {
                    if (blueItems > 0)
                    {
                        double purple = -purpleItems > orangeItems ? orangeItems : -purpleItems;
                        if (greenItems > 0)
                        {
                            targetStatisticsItem.RawItemCount += orangeItems - purple + blueItems + greenItems;
                        }
                        else
                        {
                            double green = -greenItems > orangeItems - purple + blueItems ? orangeItems - purple + blueItems : -greenItems;
                            targetStatisticsItem.RawItemCount += orangeItems - purple + blueItems - green;
                        }
                    }
                    else
                    {
                        if (greenItems > 0)
                        {
                            double purpleAndBlue = -(purpleItems + blueItems) > orangeItems ? orangeItems : -(purpleItems + blueItems);
                            targetStatisticsItem.RawItemCount += orangeItems - purpleAndBlue + greenItems;
                        }
                        else
                        {
                            double purpleAndBlueAndGreen = -(purpleItems + blueItems + greenItems) > orangeItems ? orangeItems : -(purpleItems + blueItems + greenItems);
                            targetStatisticsItem.RawItemCount += orangeItems - purpleAndBlueAndGreen;
                        }
                    }
                }
            }
            else
            {
                if (purpleItems > 0)
                {
                    if (blueItems > 0)
                    {
                        if (greenItems > 0)
                        {
                            targetStatisticsItem.RawItemCount += purpleItems + blueItems + greenItems;
                        }
                        else
                        {
                            double green = -greenItems > purpleItems + blueItems ? purpleItems + blueItems : -greenItems;
                            targetStatisticsItem.RawItemCount += purpleItems + blueItems - green;
                        }
                    }
                    else
                    {
                        if (greenItems > 0)
                        {
                            double blue = -blueItems > purpleItems ? purpleItems : -blueItems;
                            targetStatisticsItem.RawItemCount += purpleItems - blue + greenItems;
                        }
                        else
                        {
                            double blueAndGreen = -(blueItems + greenItems) > purpleItems ? purpleItems : -(blueItems + greenItems);
                            targetStatisticsItem.RawItemCount += purpleItems - blueAndGreen;
                        }
                    }
                }
                else
                {
                    if (blueItems > 0)
                    {
                        if (greenItems > 0)
                        {
                            targetStatisticsItem.RawItemCount += blueItems + greenItems;
                        }
                        else
                        {
                            double green = -greenItems > blueItems ? blueItems : -greenItems;
                            targetStatisticsItem.RawItemCount += blueItems - green;
                        }
                    }
                    else
                    {
                        if (greenItems > 0)
                        {
                            targetStatisticsItem.RawItemCount += greenItems;
                        }
                    }
                }
            }
        }

        return statistics;
    }

    public async ValueTask RemoveCultivateEntryAsync(Guid entryId)
    {
        // Invalidate cache
        entryCollectionCache.TryRemove(cultivationRepository.GetCultivateProjectIdByEntryId(entryId), out _);

        await taskContext.SwitchToBackgroundAsync();
        cultivationRepository.RemoveCultivateEntryById(entryId);
    }

    public void SaveCultivateItem(CultivateItemView item)
    {
        // Invalidate cache
        entryCollectionCache.TryRemove(cultivationRepository.GetCultivateProjectIdByEntryId(item.Entity.EntryId), out _);

        cultivationRepository.UpdateCultivateItem(item.Entity);
    }

    public async ValueTask<ConsumptionSaveResultKind> SaveConsumptionAsync(InputConsumption inputConsumption)
    {
        if (inputConsumption is { Strategy: not ConsumptionSaveStrategyKind.OverwriteExisting, Items: [] })
        {
            return ConsumptionSaveResultKind.NoItem;
        }

        if (!await EnsureCurrentProjectAsync().ConfigureAwait(false))
        {
            return ConsumptionSaveResultKind.NoProject;
        }

        ArgumentNullException.ThrowIfNull(Projects.CurrentItem);

        await taskContext.SwitchToBackgroundAsync();

        if (inputConsumption.Strategy is not ConsumptionSaveStrategyKind.CreateNewEntry)
        {
            // Check for existing entries
            List<CultivateEntry> entries = cultivationRepository.GetCultivateEntryListByProjectIdAndItemId(Projects.CurrentItem.InnerId, inputConsumption.ItemId);

            if (entries is [_, ..])
            {
                if (inputConsumption.Strategy is ConsumptionSaveStrategyKind.PreserveExisting)
                {
                    return ConsumptionSaveResultKind.Skipped;
                }

                if (inputConsumption.Strategy is ConsumptionSaveStrategyKind.OverwriteExisting)
                {
                    foreach (CultivateEntry entry in entries)
                    {
                        cultivationRepository.RemoveLevelInformationByEntryId(entry.InnerId);
                        cultivationRepository.RemoveCultivateItemRangeByEntryId(entry.InnerId);
                        cultivationRepository.RemoveCultivateEntryById(entry.InnerId);
                    }

                    if (inputConsumption.Items is [])
                    {
                        return ConsumptionSaveResultKind.Removed;
                    }
                }
            }
        }

        {
            CultivateEntry entry = CultivateEntry.From(Projects.CurrentItem.InnerId, inputConsumption.Type, inputConsumption.ItemId);
            cultivationRepository.AddCultivateEntry(entry);

            CultivateEntryLevelInformation entryLevelInformation = CultivateEntryLevelInformation.From(entry.InnerId, inputConsumption.Type, inputConsumption.LevelInformation);
            cultivationRepository.AddLevelInformation(entryLevelInformation);

            IEnumerable<CultivateItem> toAdd = inputConsumption.Items.Select(item => CultivateItem.From(entry.InnerId, item));
            cultivationRepository.AddCultivateItemRange(toAdd);

            // Invalidate cache
            entryCollectionCache.TryRemove(Projects.CurrentItem.InnerId, out _);
        }

        return ConsumptionSaveResultKind.Added;
    }

    public async ValueTask<ProjectAddResultKind> TryAddProjectAsync(CultivateProject project)
    {
        if (string.IsNullOrWhiteSpace(project.Name))
        {
            return ProjectAddResultKind.InvalidName;
        }

        ArgumentNullException.ThrowIfNull(projects);

        if (projects.SourceCollection.Any(a => a.Name == project.Name))
        {
            return ProjectAddResultKind.AlreadyExists;
        }

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        projects.Add(project);
        projects.MoveCurrentTo(project);

        return ProjectAddResultKind.Added;
    }

    public async ValueTask RemoveProjectAsync(CultivateProject project)
    {
        ArgumentNullException.ThrowIfNull(projects);

        // Sync cache
        // Keep this on main thread.
        await taskContext.SwitchToMainThreadAsync();
        projects.Remove(project);

        // Invalidate cache
        entryCollectionCache.TryRemove(project.InnerId, out _);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        cultivationRepository.RemoveCultivateProjectById(project.InnerId);
    }

    public async ValueTask<bool> EnsureCurrentProjectAsync()
    {
        if (Projects.CurrentItem is null)
        {
            await taskContext.SwitchToMainThreadAsync();
            try
            {
                Projects.MoveCurrentTo(Projects.SourceCollection.SelectedOrDefault());
            }
            catch (InvalidOperationException)
            {
                // Sequence contains more than one matching element
            }

            if (Projects.CurrentItem is null)
            {
                return false;
            }
        }

        return true;
    }
}