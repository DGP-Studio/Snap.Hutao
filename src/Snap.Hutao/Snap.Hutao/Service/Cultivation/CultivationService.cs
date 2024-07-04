// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Inventory;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.ObjectModel;
using CalculateItem = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Item;
using ModelItem = Snap.Hutao.Model.Item;

namespace Snap.Hutao.Service.Cultivation;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(ICultivationService))]
internal sealed partial class CultivationService : ICultivationService
{
    private readonly ICultivationDbService cultivationDbService;
    private readonly IInventoryDbService inventoryDbService;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    private AdvancedDbCollectionView<CultivateProject>? projects;

    /// <inheritdoc/>
    public AdvancedDbCollectionView<CultivateProject> Projects
    {
        get => projects ??= new(cultivationDbService.GetCultivateProjectCollection(), serviceProvider);
    }

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<CultivateEntryView>> GetCultivateEntriesAsync(CultivateProject cultivateProject, ICultivationMetadataContext context)
    {
        await taskContext.SwitchToBackgroundAsync();
        List<CultivateEntry> entries = cultivationDbService.GetCultivateEntryListIncludingLevelInformationByProjectId(cultivateProject.InnerId);

        List<CultivateEntryView> resultEntries = new(entries.Count);
        foreach (CultivateEntry entry in entries)
        {
            List<CultivateItemView> entryItems = [];

            foreach (CultivateItem cultivateItem in cultivationDbService.GetCultivateItemListByEntryId(entry.InnerId))
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

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<StatisticsCultivateItem>> GetStatisticsCultivateItemCollectionAsync(
        CultivateProject cultivateProject, ICultivationMetadataContext context, CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        List<StatisticsCultivateItem> resultItems = [];

        Guid projectId = cultivateProject.InnerId;

        foreach (CultivateEntry entry in cultivationDbService.GetCultivateEntryListByProjectId(projectId))
        {
            foreach (CultivateItem item in cultivationDbService.GetCultivateItemListByEntryId(entry.InnerId))
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

        foreach (InventoryItem inventoryItem in inventoryDbService.GetInventoryItemListByProjectId(projectId))
        {
            if (resultItems.SingleOrDefault(i => i.Inner.Id == inventoryItem.ItemId) is { } existedItem)
            {
                existedItem.TotalCount += inventoryItem.Count;
            }
        }

        return resultItems.SortBy(item => item.Inner.Id, MaterialIdComparer.Shared).ToObservableCollection();
    }

    /// <inheritdoc/>
    public async ValueTask RemoveCultivateEntryAsync(Guid entryId)
    {
        await taskContext.SwitchToBackgroundAsync();
        cultivationDbService.RemoveCultivateEntryById(entryId);
    }

    /// <inheritdoc/>
    public void SaveCultivateItem(CultivateItemView item)
    {
        cultivationDbService.UpdateCultivateItem(item.Entity);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> SaveConsumptionAsync(CultivateType type, uint itemId, List<CalculateItem> items, LevelInformation levelInformation)
    {
        if (items.Count == 0)
        {
            return true;
        }

        await taskContext.SwitchToMainThreadAsync();
        if (Projects.CurrentItem is null)
        {
            Projects.MoveCurrentTo(Projects.SourceCollection.SelectedOrDefault());
            if (Projects.CurrentItem is null)
            {
                return false;
            }
        }

        await taskContext.SwitchToBackgroundAsync();
        CultivateEntry? entry = type is CultivateType.AvatarAndSkill
            ? cultivationDbService.GetCultivateEntryByProjectIdAndItemId(Projects.CurrentItem.InnerId, itemId)
            : default;

        if (entry is null)
        {
            entry = CultivateEntry.From(Projects.CurrentItem.InnerId, type, itemId);
            cultivationDbService.AddCultivateEntry(entry);
        }

        Guid entryId = entry.InnerId;

        cultivationDbService.RemoveLevelInformationByEntryId(entryId);
        CultivateEntryLevelInformation entryLevelInformation = CultivateEntryLevelInformation.From(entryId, type, levelInformation);
        cultivationDbService.AddLevelInformation(entryLevelInformation);

        cultivationDbService.RemoveCultivateItemRangeByEntryId(entryId);
        IEnumerable<CultivateItem> toAdd = items.Select(item => CultivateItem.From(entryId, item));
        cultivationDbService.AddCultivateItemRange(toAdd);

        return true;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async ValueTask RemoveProjectAsync(CultivateProject project)
    {
        ArgumentNullException.ThrowIfNull(projects);

        // Sync cache
        // Keep this on main thread.
        await taskContext.SwitchToMainThreadAsync();
        projects.Remove(project);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        cultivationDbService.RemoveCultivateProjectById(project.InnerId);
    }
}