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
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Cultivation;

/// <summary>
/// 养成计算服务
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton, typeof(ICultivationService))]
internal sealed partial class CultivationService : ICultivationService
{
    private readonly ITaskContext taskContext;
    private readonly IServiceProvider serviceProvider;
    private readonly ScopedDbCurrent<CultivateProject, Message.CultivateProjectChangedMessage> dbCurrent;

    private ObservableCollection<CultivateProject>? projects;

    /// <summary>
    /// 构造一个新的养成计算服务
    /// </summary>
    /// <param name="serviceProvider">范围工厂</param>
    /// <param name="messenger">消息器</param>
    public CultivationService(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        dbCurrent = new(serviceProvider);

        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public List<InventoryItemView> GetInventoryItems(CultivateProject cultivateProject, List<Material> metadata, ICommand saveCommand)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Guid projectId = cultivateProject.InnerId;
            List<InventoryItem> entities = appDbContext.InventoryItems
                .Where(a => a.ProjectId == projectId)
                .ToList();

            List<InventoryItemView> results = new();
            foreach (Material meta in metadata.Where(m => m.IsInventoryItem()).OrderBy(m => m.Id.Value))
            {
                InventoryItem entity = entities.SingleOrDefault(e => e.ItemId == meta.Id) ?? InventoryItem.Create(projectId, meta.Id);
                results.Add(new(entity, meta, saveCommand));
            }

            return results;
        }
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<CultivateEntryView>> GetCultivateEntriesAsync(CultivateProject cultivateProject)
    {
        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            IMetadataService metadataService = scope.ServiceProvider.GetRequiredService<IMetadataService>();

            List<Material> materials = await metadataService.GetMaterialsAsync().ConfigureAwait(false);
            Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            Dictionary<WeaponId, Model.Metadata.Weapon.Weapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync().ConfigureAwait(false);

            List<CultivateEntry> entries = await appDbContext.CultivateEntries
                .Where(e => e.ProjectId == cultivateProject.InnerId)
                .ToListAsync()
                .ConfigureAwait(false);

            List<CultivateEntryView> results = new(entries.Count);
            foreach (CultivateEntry entry in entries)
            {
                Guid entryId = entry.InnerId;

                List<CultivateItemView> resultItems = new();

                foreach (CultivateItem item in await GetEntryItemsAsync(appDbContext, entryId).ConfigureAwait(false))
                {
                    resultItems.Add(new(item, materials.Single(m => m.Id == item.ItemId)));
                }

                Item itemBase = entry.Type switch
                {
                    CultivateType.AvatarAndSkill => idAvatarMap[entry.Id].ToItemBase(),
                    CultivateType.Weapon => idWeaponMap[entry.Id].ToItemBase(),
                    _ => null!, // TODO: support furniture calc
                };

                results.Add(new(entry, itemBase, resultItems));
            }

            return results
                .OrderByDescending(e => e.IsToday)
                .ToObservableCollection();
        }
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<StatisticsCultivateItem>> GetStatisticsCultivateItemCollectionAsync(CultivateProject cultivateProject, CancellationToken token)
    {
        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            List<StatisticsCultivateItem> resultItems = new();
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            List<Material> materials = await scope.ServiceProvider
                .GetRequiredService<IMetadataService>()
                .GetMaterialsAsync(default)
                .ConfigureAwait(false);

            Guid projectId = cultivateProject.InnerId;

            token.ThrowIfCancellationRequested();

            foreach (CultivateEntry entry in await GetProjectEntriesAsync(appDbContext, projectId).ConfigureAwait(false))
            {
                foreach (CultivateItem item in await GetEntryItemsAsync(appDbContext, entry.InnerId).ConfigureAwait(false))
                {
                    if (item.IsFinished)
                    {
                        continue;
                    }

                    if (resultItems.SingleOrDefault(i => i.Inner.Id == item.ItemId) is StatisticsCultivateItem existedItem)
                    {
                        existedItem.Count += item.Count;
                    }
                    else
                    {
                        resultItems.Add(new(materials!.Single(m => m.Id == item.ItemId), item));
                    }
                }
            }

            token.ThrowIfCancellationRequested();

            foreach (InventoryItem inventoryItem in await GetProjectInventoryAsync(appDbContext, projectId).ConfigureAwait(false))
            {
                if (resultItems.SingleOrDefault(i => i.Inner.Id == inventoryItem.ItemId) is StatisticsCultivateItem existedItem)
                {
                    existedItem.TotalCount += inventoryItem.Count;
                }
            }

            token.ThrowIfCancellationRequested();

            await taskContext.SwitchToMainThreadAsync();
            return resultItems.OrderByDescending(i => i.TotalCount).ToObservableCollection();
        }
    }

    /// <inheritdoc/>
    public async Task RemoveCultivateEntryAsync(Guid entryId)
    {
        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.CultivateEntries
                .ExecuteDeleteWhereAsync(i => i.InnerId == entryId)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public void SaveInventoryItem(InventoryItemView item)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<AppDbContext>().InventoryItems.UpdateAndSave(item.Entity);
        }
    }

    /// <inheritdoc/>
    public void SaveCultivateItem(CultivateItem item)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<AppDbContext>().CultivateItems.UpdateAndSave(item);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SaveConsumptionAsync(CultivateType type, int itemId, List<Web.Hoyolab.Takumi.Event.Calculate.Item> items)
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
                entry = CultivateEntry.Create(projectId, type, itemId);
                await appDbContext.CultivateEntries.AddAndSaveAsync(entry).ConfigureAwait(false);
            }

            Guid entryId = entry.InnerId;
            await appDbContext.CultivateItems.ExecuteDeleteWhereAsync(i => i.EntryId == entryId).ConfigureAwait(false);

            IEnumerable<CultivateItem> toAdd = items.Select(i => CultivateItem.Create(entryId, i.Id, i.Num));
            await appDbContext.CultivateItems.AddRangeAndSaveAsync(toAdd).ConfigureAwait(false);
        }

        return true;
    }

    [SuppressMessage("", "SH002")]
    private static Task<List<InventoryItem>> GetProjectInventoryAsync(AppDbContext appDbContext, Guid projectId)
    {
        return appDbContext.InventoryItems
            .Where(e => e.ProjectId == projectId)
            .ToListAsync();
    }

    [SuppressMessage("", "SH002")]
    private static Task<List<CultivateEntry>> GetProjectEntriesAsync(AppDbContext appDbContext, Guid projectId)
    {
        return appDbContext.CultivateEntries
            .Where(e => e.ProjectId == projectId)
            .ToListAsync();
    }

    [SuppressMessage("", "SH002")]
    private static Task<List<CultivateItem>> GetEntryItemsAsync(AppDbContext appDbContext, Guid entryId)
    {
        return appDbContext.CultivateItems
            .Where(i => i.EntryId == entryId)
            .OrderBy(i => i.ItemId)
            .ToListAsync();
    }
}