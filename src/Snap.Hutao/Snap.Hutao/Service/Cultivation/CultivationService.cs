// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Cultivation.Consumption;
using Snap.Hutao.Service.Inventory;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ModelItem = Snap.Hutao.Model.Item;

namespace Snap.Hutao.Service.Cultivation;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(ICultivationService))]
internal sealed partial class CultivationService : ICultivationService
{
    private readonly ConcurrentDictionary<Guid, ObservableCollection<CultivateEntryView>> entryCollectionCache = [];
    private readonly AsyncLock entryCollectionLock = new();
    private readonly AsyncLock projectsLock = new();

    private readonly ICultivationResinStatisticsService cultivationResinStatisticsService;
    private readonly ICultivationRepository cultivationRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private AdvancedDbCollectionView<CultivateProject>? projects;

    public async ValueTask<IAdvancedDbCollectionView<CultivateProject>> GetProjectCollectionAsync()
    {
        using (await projectsLock.LockAsync().ConfigureAwait(false))
        {
            return projects ??= new(cultivationRepository.GetCultivateProjectCollection(), serviceProvider);
        }
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
            return GetCultivateEntryCollectionCore(cultivateProject, context);
        }

        ObservableCollection<CultivateEntryView> GetCultivateEntryCollectionCore(CultivateProject cultivateProject, ICultivationMetadataContext context)
        {
            ImmutableArray<CultivateEntry> entries = cultivationRepository.GetCultivateEntryImmutableArrayIncludingLevelInformationByProjectId(cultivateProject.InnerId);

            List<CultivateEntryView> resultEntries = new(entries.Length);
            foreach (ref readonly CultivateEntry entry in entries.AsSpan())
            {
                ImmutableArray<CultivateItem> items = cultivationRepository.GetCultivateItemImmutableArrayByEntryId(entry.InnerId);
                ImmutableArray<CultivateItemView>.Builder entryItems = ImmutableArray.CreateBuilder<CultivateItemView>(items.Length);

                foreach (ref readonly CultivateItem cultivateItem in items.AsSpan())
                {
                    entryItems.Add(CultivateItemView.Create(cultivateItem, context.GetMaterial(cultivateItem.ItemId)));
                }

                ModelItem item = entry.Type switch
                {
                    CultivateType.AvatarAndSkill => context.GetAvatarItem(entry.Id),
                    CultivateType.Weapon => context.GetWeaponItem(entry.Id),

                    // TODO: support furniture calc
                    _ => default!,
                };

                resultEntries.Add(CultivateEntryView.Create(entry, item, entryItems.ToImmutable()));
            }

            ObservableCollection<CultivateEntryView> result = resultEntries.SortByDescending(e => e.IsToday).ToObservableCollection();
            entryCollectionCache.TryAdd(cultivateProject.InnerId, result);
            return result;
        }
    }

    public async ValueTask<ObservableCollection<StatisticsCultivateItem>> GetStatisticsCultivateItemCollectionAsync(CultivateProject cultivateProject, ICultivationMetadataContext context, CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        return GetStatisticsCultivateItemCollectionCore(cultivateProject, context);

        ObservableCollection<StatisticsCultivateItem> GetStatisticsCultivateItemCollectionCore(CultivateProject cultivateProject, ICultivationMetadataContext context)
        {
            Dictionary<uint, StatisticsCultivateItem> resultItems = [];
            Guid projectId = cultivateProject.InnerId;

            foreach (ref readonly CultivateEntry entry in cultivationRepository.GetCultivateEntryImmutableArrayByProjectId(projectId).AsSpan())
            {
                foreach (ref readonly CultivateItem item in cultivationRepository.GetCultivateItemImmutableArrayByEntryId(entry.InnerId).AsSpan())
                {
                    if (item.IsFinished)
                    {
                        continue;
                    }

                    ref StatisticsCultivateItem? existedItem = ref CollectionsMarshal.GetValueRefOrAddDefault(resultItems, item.ItemId, out _);
                    if (existedItem is not null)
                    {
                        existedItem.Count += item.Count;
                    }
                    else
                    {
                        existedItem = StatisticsCultivateItem.Create(context.GetMaterial(item.ItemId), item);
                    }
                }
            }

            foreach (ref readonly InventoryItem inventoryItem in inventoryRepository.GetInventoryItemImmutableArrayByProjectId(projectId).AsSpan())
            {
                ref StatisticsCultivateItem existedItem = ref CollectionsMarshal.GetValueRefOrNullRef(resultItems, inventoryItem.ItemId);
                if (!Unsafe.IsNullRef(in existedItem))
                {
                    existedItem.Current += inventoryItem.Count;
                }
            }

            return resultItems.Select(static kvp => kvp.Value).OrderBy(item => item.Inner.Id, MaterialIdComparer.Shared).ToObservableCollection();
        }
    }

    public ValueTask<ResinStatistics> GetResinStatisticsAsync(ObservableCollection<StatisticsCultivateItem> statisticsCultivateItems, CancellationToken token)
    {
        return cultivationResinStatisticsService.GetResinStatisticsAsync(statisticsCultivateItems, token);
    }

    public async ValueTask RemoveCultivateEntryAsync(Guid entryId)
    {
        await taskContext.SwitchToBackgroundAsync();
        cultivationRepository.RemoveCultivateEntryById(entryId);
    }

    public void SaveCultivateItem(CultivateItemView item)
    {
        cultivationRepository.UpdateCultivateItem(item.Entity);
    }

    public async ValueTask<ConsumptionSaveResultKind> SaveConsumptionAsync(InputConsumption inputConsumption)
    {
        // No selected project
        IAdvancedDbCollectionView<CultivateProject> projects = await GetProjectCollectionAsync().ConfigureAwait(false);
        if (!await EnsureCurrentProjectAsync(projects).ConfigureAwait(false))
        {
            return ConsumptionSaveResultKind.NoProject;
        }

        ArgumentNullException.ThrowIfNull(projects.CurrentItem);

        await taskContext.SwitchToBackgroundAsync();

        // PreserveExisting or CreateNewEntry, but no item
        if (inputConsumption is { Strategy: not ConsumptionSaveStrategyKind.OverwriteExisting, Items: [] })
        {
            return ConsumptionSaveResultKind.NoItem;
        }

        // PreserveExisting or OverwriteExisting
        if (inputConsumption.Strategy is not ConsumptionSaveStrategyKind.CreateNewEntry)
        {
            // Check for existing entries
            ImmutableArray<CultivateEntry> entries = cultivationRepository.GetCultivateEntryImmutableArrayByProjectIdAndItemId(projects.CurrentItem.InnerId, inputConsumption.ItemId);

            if (entries.Length > 0)
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
            else
            {
                if (inputConsumption.Items is [])
                {
                    return ConsumptionSaveResultKind.NoItem;
                }
            }
        }

        {
            CultivateEntry entry = CultivateEntry.From(projects.CurrentItem.InnerId, inputConsumption.Type, inputConsumption.ItemId);
            cultivationRepository.AddCultivateEntry(entry);

            CultivateEntryLevelInformation entryLevelInformation = CultivateEntryLevelInformation.From(entry.InnerId, inputConsumption.Type, inputConsumption.LevelInformation);
            cultivationRepository.AddLevelInformation(entryLevelInformation);

            IEnumerable<CultivateItem> toAdd = inputConsumption.Items.Select(item => CultivateItem.From(entry.InnerId, item));
            cultivationRepository.AddCultivateItemRange(toAdd);

            // The consumption save operation is always performed outside cultivation page
            // and without touching the cache. So we have to invalidate the cache manually.
            entryCollectionCache.TryRemove(projects.CurrentItem.InnerId, out _);
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

        // Keep this on main thread.
        await taskContext.SwitchToMainThreadAsync();
        projects.Remove(project);

        await taskContext.SwitchToBackgroundAsync();
        entryCollectionCache.TryRemove(project.InnerId, out _);
        cultivationRepository.RemoveCultivateProjectById(project.InnerId);
    }

    public async ValueTask<bool> EnsureCurrentProjectAsync(IAdvancedDbCollectionView<CultivateProject> projects)
    {
        if (projects.CurrentItem is null)
        {
            try
            {
                await taskContext.SwitchToMainThreadAsync();
                projects.MoveCurrentTo(projects.SourceCollection.SelectedOrDefault());
            }
            catch (InvalidOperationException)
            {
                // Sequence contains more than one matching element
            }

            if (projects.CurrentItem is null)
            {
                return false;
            }
        }

        return true;
    }
}