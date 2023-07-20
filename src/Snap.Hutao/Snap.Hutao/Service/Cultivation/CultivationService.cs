// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.ObjectModel;

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
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;

    /// <inheritdoc/>
    public List<InventoryItemView> GetInventoryItemViews(CultivateProject cultivateProject, List<Material> metadata, ICommand saveCommand)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Guid projectId = cultivateProject.InnerId;
            List<InventoryItem> entities = cultivationDbService.GetInventoryItemListByProjectId(projectId);

            List<InventoryItemView> results = new();
            foreach (Material meta in metadata.Where(m => m.IsInventoryItem()).OrderBy(m => m.Id.Value))
            {
                InventoryItem entity = entities.SingleOrDefault(e => e.ItemId == meta.Id) ?? InventoryItem.From(projectId, meta.Id);
                results.Add(new(entity, meta, saveCommand));
            }

            return results;
        }
    }

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<CultivateEntryView>> GetCultivateEntriesAsync(
        CultivateProject cultivateProject,
        List<Material> materials,
        Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> idAvatarMap,
        Dictionary<WeaponId, Model.Metadata.Weapon.Weapon> idWeaponMap)
    {
        await taskContext.SwitchToBackgroundAsync();
        List<CultivateEntry> entries = await cultivationDbService
            .GetCultivateEntryListByProjectIdAsync(cultivateProject.InnerId)
            .ConfigureAwait(false);

        List<CultivateEntryView> resultEntries = new(entries.Count);
        foreach (CultivateEntry entry in entries)
        {
            List<CultivateItemView> entryItems = new();
            foreach (CultivateItem item in await cultivationDbService.GetCultivateItemListByEntryIdAsync(entry.InnerId).ConfigureAwait(false))
            {
                entryItems.Add(new(item, materials.Single(m => m.Id == item.ItemId)));
            }

            Item itemBase = entry.Type switch
            {
                CultivateType.AvatarAndSkill => idAvatarMap[entry.Id].ToItem(),
                CultivateType.Weapon => idWeaponMap[entry.Id].ToItem(),

                // TODO: support furniture calc
                _ => default!,
            };

            resultEntries.Add(new(entry, itemBase, entryItems));
        }

        return resultEntries
            .OrderByDescending(e => e.IsToday)
            .ToObservableCollection();
    }

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<StatisticsCultivateItem>> GetStatisticsCultivateItemCollectionAsync(
        CultivateProject cultivateProject,
        List<Material> materials,
        CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        List<StatisticsCultivateItem> resultItems = new();

        Guid projectId = cultivateProject.InnerId;

        token.ThrowIfCancellationRequested();

        foreach (CultivateEntry entry in await cultivationDbService.GetCultivateEntryListByProjectIdAsync(projectId).ConfigureAwait(false))
        {
            foreach (CultivateItem item in await cultivationDbService.GetCultivateItemListByEntryIdAsync(entry.InnerId).ConfigureAwait(false))
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
                    resultItems.Add(new(materials.Single(m => m.Id == item.ItemId), item));
                }
            }
        }

        token.ThrowIfCancellationRequested();

        foreach (InventoryItem inventoryItem in await cultivationDbService.GetInventoryItemListByProjectIdAsync(projectId).ConfigureAwait(false))
        {
            if (resultItems.SingleOrDefault(i => i.Inner.Id == inventoryItem.ItemId) is { } existedItem)
            {
                existedItem.TotalCount += inventoryItem.Count;
            }
        }

        token.ThrowIfCancellationRequested();

        return resultItems
            .OrderByDescending(i => i.TotalCount)
            .ThenByDescending(i => i.Count)
            .ToObservableCollection();
    }

    /// <inheritdoc/>
    public async ValueTask RemoveCultivateEntryAsync(Guid entryId)
    {
        await taskContext.SwitchToBackgroundAsync();
        await cultivationDbService.DeleteCultivateEntryByIdAsync(entryId).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void SaveInventoryItem(InventoryItemView item)
    {
        cultivationDbService.UpdateInventoryItem(item.Entity);
    }

    /// <inheritdoc/>
    public void SaveCultivateItem(CultivateItemView item)
    {
        cultivationDbService.UpdateCultivateItem(item.Entity);
    }

    /// <inheritdoc/>
    public async Task<bool> SaveConsumptionAsync(CultivateType type, uint itemId, List<Web.Hoyolab.Takumi.Event.Calculate.Item> items)
    {
        if (items.Count == 0)
        {
            return true;
        }

        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                Current ??= appDbContext.CultivateProjects.SelectedOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                ThrowHelper.UserdataCorrupted(SH.ServiceCultivationProjectCurrentUserdataCourrpted2, ex);
            }

            if (Current == null)
            {
                return false;
            }

            Guid projectId = Current!.InnerId;
            CultivateEntry? entry = await appDbContext.CultivateEntries
                .SingleOrDefaultAsync(e => e.ProjectId == projectId && e.Id == itemId)
                .ConfigureAwait(false);

            if (entry == null)
            {
                entry = CultivateEntry.From(projectId, type, itemId);
                await appDbContext.CultivateEntries.AddAndSaveAsync(entry).ConfigureAwait(false);
            }

            Guid entryId = entry.InnerId;
            await appDbContext.CultivateItems.ExecuteDeleteWhereAsync(i => i.EntryId == entryId).ConfigureAwait(false);

            IEnumerable<CultivateItem> toAdd = items.Select(item => CultivateItem.From(entryId, item));
            await appDbContext.CultivateItems.AddRangeAndSaveAsync(toAdd).ConfigureAwait(false);
        }

        return true;
    }
}