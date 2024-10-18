// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Inventory;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.ObjectModel;
using ModelItem = Snap.Hutao.Model.Item;

namespace Snap.Hutao.Service.Cultivation;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(ICultivationService))]
internal sealed partial class CultivationService : ICultivationService
{
    private readonly ICultivationRepository cultivationRepository;
    private readonly IInventoryRepository inventoryRepository;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private AdvancedDbCollectionView<CultivateProject>? projects;

    public AdvancedDbCollectionView<CultivateProject> Projects
    {
        get => projects ??= new(cultivationRepository.GetCultivateProjectCollection(), serviceProvider);
    }

    public async ValueTask<ObservableCollection<CultivateEntryView>> GetCultivateEntriesAsync(CultivateProject cultivateProject, ICultivationMetadataContext context)
    {
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
                CultivateType.AvatarAndSkill => context.GetAvatar(entry.Id).ToItem(),
                CultivateType.Weapon => context.GetWeapon(entry.Id).ToItem(),

                // TODO: support furniture calc
                _ => default!,
            };

            resultEntries.Add(new(entry, item, entryItems));
        }

        return resultEntries.SortByDescending(e => e.IsToday).ToObservableCollection();
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
                    existedItem.Count += item.Count;
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
                existedItem.TotalCount += inventoryItem.Count;
            }
        }

        return resultItems.SortBy(item => item.Inner.Id, MaterialIdComparer.Shared).ToObservableCollection();
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
        // Only return NoItem when not overwriting existing items.
        if (inputConsumption.Strategy is not ConsumptionSaveStrategyKind.OverwriteExisting && inputConsumption.Items.Count <= 0)
        {
            return ConsumptionSaveResultKind.NoItem;
        }

        if (Projects.CurrentItem is null)
        {
            await taskContext.SwitchToMainThreadAsync();
            Projects.MoveCurrentTo(Projects.SourceCollection.SelectedOrDefault());
            if (Projects.CurrentItem is null)
            {
                return ConsumptionSaveResultKind.NoProject;
            }
        }

        await taskContext.SwitchToBackgroundAsync();

        List<CultivateEntry> entries = [];

        if (inputConsumption.Strategy is ConsumptionSaveStrategyKind.PreserveExisting or ConsumptionSaveStrategyKind.OverwriteExisting)
        {
            entries = cultivationRepository.GetCultivateEntryListByProjectIdAndItemId(Projects.CurrentItem.InnerId, inputConsumption.ItemId);

            if (inputConsumption.Strategy is ConsumptionSaveStrategyKind.PreserveExisting && entries.Count > 0)
            {
                return ConsumptionSaveResultKind.Skipped;
            }
        }

        if (entries.Count > 0 && inputConsumption.Strategy is ConsumptionSaveStrategyKind.OverwriteExisting && inputConsumption.Items.Count <= 0)
        {
            foreach (CultivateEntry entry in entries)
            {
                cultivationRepository.RemoveCultivateEntryById(entry.InnerId);
            }

            return ConsumptionSaveResultKind.Removed;
        }

        // TODO: Not done yet
        //CultivateEntry? entry = default;
        if (entries.Count <= 0)
        {
            CultivateEntry entry = CultivateEntry.From(Projects.CurrentItem.InnerId, inputConsumption.Type, inputConsumption.ItemId);
            cultivationRepository.AddCultivateEntry(entry);
        }

        Guid entryId = entries.First().InnerId;

        cultivationRepository.RemoveLevelInformationByEntryId(entryId);
        CultivateEntryLevelInformation entryLevelInformation = CultivateEntryLevelInformation.From(entryId, inputConsumption.Type, inputConsumption.LevelInformation);
        cultivationRepository.AddLevelInformation(entryLevelInformation);

        cultivationRepository.RemoveCultivateItemRangeByEntryId(entryId);
        IEnumerable<CultivateItem> toAdd = inputConsumption.Items.Select(item => CultivateItem.From(entryId, item));
        cultivationRepository.AddCultivateItemRange(toAdd);

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

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        cultivationRepository.RemoveCultivateProjectById(project.InnerId);
    }
}