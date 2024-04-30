// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Service.Inventory;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.ObjectModel;
using CalculateItem = Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate.Item;

namespace Snap.Hutao.Service.Cultivation;

/// <summary>
/// 养成计算服务
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(ICultivationService))]
internal sealed partial class CultivationService : ICultivationService
{
    private readonly ScopedDbCurrent<CultivateProject, Message.CultivateProjectChangedMessage> dbCurrent;
    private readonly ICultivationDbService cultivationDbService;
    private readonly IInventoryDbService inventoryDbService;
    private readonly ITaskContext taskContext;

    private ObservableCollection<CultivateProject>? projects;

    /// <inheritdoc/>
    public CultivateProject? Current
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    /// <inheritdoc/>
    public ObservableCollection<CultivateProject> ProjectCollection
    {
        get
        {
            if (projects is null)
            {
                projects = cultivationDbService.GetCultivateProjectCollection();
                Current ??= projects.SelectedOrDefault();
            }

            return projects;
        }
    }

    /// <inheritdoc/>
    public List<InventoryItemView> GetInventoryItemViews(CultivateProject cultivateProject, ICultivationMetadataContext context, ICommand saveCommand)
    {
        Guid projectId = cultivateProject.InnerId;
        List<InventoryItem> entities = cultivationDbService.GetInventoryItemListByProjectId(projectId);

        List<InventoryItemView> results = [];
        foreach (Material meta in context.EnumerateInventoryMaterial())
        {
            InventoryItem entity = entities.SingleOrDefault(e => e.ItemId == meta.Id) ?? InventoryItem.From(projectId, meta.Id);
            results.Add(new(entity, meta, saveCommand));
        }

        return results;
    }

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<CultivateEntryView>> GetCultivateEntriesAsync(CultivateProject cultivateProject, ICultivationMetadataContext context)
    {
        await taskContext.SwitchToBackgroundAsync();
        List<CultivateEntry> entries = await cultivationDbService
            .GetCultivateEntryListIncludingLevelInformationByProjectIdAsync(cultivateProject.InnerId)
            .ConfigureAwait(false);

        List<CultivateEntryView> resultEntries = new(entries.Count);
        foreach (CultivateEntry entry in entries)
        {
            List<CultivateItemView> entryItems = [];

            // Async operation here, thus we can't use the Span trick.
            foreach (CultivateItem cultivateItem in await cultivationDbService.GetCultivateItemListByEntryIdAsync(entry.InnerId).ConfigureAwait(false))
            {
                entryItems.Add(new(cultivateItem, context.GetMaterial(cultivateItem.ItemId)));
            }

            Item item = entry.Type switch
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

        foreach (CultivateEntry entry in await cultivationDbService.GetCultivateEntryListByProjectIdAsync(projectId, token).ConfigureAwait(false))
        {
            foreach (CultivateItem item in await cultivationDbService.GetCultivateItemListByEntryIdAsync(entry.InnerId, token).ConfigureAwait(false))
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

        foreach (InventoryItem inventoryItem in await cultivationDbService.GetInventoryItemListByProjectIdAsync(projectId, token).ConfigureAwait(false))
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
        await cultivationDbService.RemoveCultivateEntryByIdAsync(entryId).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void SaveInventoryItem(InventoryItemView item)
    {
        inventoryDbService.UpdateInventoryItem(item.Entity);
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

        await taskContext.SwitchToBackgroundAsync();

        if (Current is null)
        {
            _ = ProjectCollection;
        }

        if (Current is null)
        {
            return false;
        }

        CultivateEntry? entry = await cultivationDbService
            .GetCultivateEntryByProjectIdAndItemIdAsync(Current.InnerId, itemId)
            .ConfigureAwait(false);

        if (entry is null)
        {
            entry = CultivateEntry.From(Current.InnerId, type, itemId);
            await cultivationDbService.AddCultivateEntryAsync(entry).ConfigureAwait(false);
        }

        Guid entryId = entry.InnerId;

        await cultivationDbService.RemoveLevelInformationByEntryIdAsync(entryId).ConfigureAwait(false);
        CultivateEntryLevelInformation entryLevelInformation = CultivateEntryLevelInformation.From(entryId, type, levelInformation);
        await cultivationDbService.AddLevelInformationAsync(entryLevelInformation).ConfigureAwait(false);

        await cultivationDbService.RemoveCultivateItemRangeByEntryIdAsync(entryId).ConfigureAwait(false);
        IEnumerable<CultivateItem> toAdd = items.Select(item => CultivateItem.From(entryId, item));
        await cultivationDbService.AddCultivateItemRangeAsync(toAdd).ConfigureAwait(false);

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

        if (projects.Any(a => a.Name == project.Name))
        {
            return ProjectAddResultKind.AlreadyExists;
        }

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        projects.Add(project);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        await cultivationDbService.AddCultivateProjectAsync(project).ConfigureAwait(false);

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
        await cultivationDbService.RemoveCultivateProjectByIdAsync(project.InnerId).ConfigureAwait(false);
    }
}