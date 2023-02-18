// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Binding;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Metadata;
using System.Collections.ObjectModel;
using BindingCultivateEntry = Snap.Hutao.Model.Binding.Cultivation.CultivateEntryView;
using BindingCultivateItem = Snap.Hutao.Model.Binding.Cultivation.CultivateItem;
using BindingInventoryItem = Snap.Hutao.Model.Binding.Inventory.InventoryItem;
using BindingStatisticsItem = Snap.Hutao.Model.Binding.Cultivation.StatisticsCultivateItem;

namespace Snap.Hutao.Service.Cultivation;

/// <summary>
/// 养成计算服务
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton, typeof(ICultivationService))]
internal sealed class CultivationService : ICultivationService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ScopedDbCurrent<CultivateProject, Message.CultivateProjectChangedMessage> dbCurrent;

    private ObservableCollection<CultivateProject>? projects;

    /// <summary>
    /// 构造一个新的养成计算服务
    /// </summary>
    /// <param name="scopeFactory">范围工厂</param>
    /// <param name="messenger">消息器</param>
    public CultivationService(IServiceScopeFactory scopeFactory, IMessenger messenger)
    {
        this.scopeFactory = scopeFactory;
        dbCurrent = new(scopeFactory, provider => provider.GetRequiredService<AppDbContext>().CultivateProjects, messenger);
    }

    /// <inheritdoc/>
    public CultivateProject? Current
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    /// <inheritdoc/>
    public ObservableCollection<CultivateProject> GetProjectCollection()
    {
        if (projects == null)
        {
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                projects = appDbContext.CultivateProjects.AsNoTracking().ToObservableCollection();
            }

            try
            {
                Current ??= projects.SingleOrDefault(proj => proj.IsSelected);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ThrowHelper.UserdataCorrupted(SH.ServiceCultivationProjectCurrentUserdataCourrpted, ex);
            }
            catch (InvalidOperationException ex)
            {
                ThrowHelper.UserdataCorrupted(SH.ServiceCultivationProjectCurrentUserdataCourrpted2, ex);
            }
        }

        return projects;
    }

    /// <inheritdoc/>
    public async Task<ProjectAddResult> TryAddProjectAsync(CultivateProject project)
    {
        if (string.IsNullOrWhiteSpace(project.Name))
        {
            return ProjectAddResult.InvalidName;
        }

        if (projects!.SingleOrDefault(a => a.Name == project.Name) != null)
        {
            return ProjectAddResult.AlreadyExists;
        }
        else
        {
            // Sync cache
            await ThreadHelper.SwitchToMainThreadAsync();
            projects!.Add(project);

            // Sync database
            await ThreadHelper.SwitchToBackgroundAsync();
            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await appDbContext.CultivateProjects.AddAndSaveAsync(project).ConfigureAwait(false);
            }

            return ProjectAddResult.Added;
        }
    }

    /// <inheritdoc/>
    public async Task RemoveProjectAsync(CultivateProject project)
    {
        // Sync cache
        // Keep this on main thread.
        await ThreadHelper.SwitchToMainThreadAsync();
        projects!.Remove(project);

        // Sync database
        await ThreadHelper.SwitchToBackgroundAsync();
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.CultivateProjects
                .ExecuteDeleteWhereAsync(p => p.InnerId == project.InnerId)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public List<BindingInventoryItem> GetInventoryItems(CultivateProject cultivateProject, List<Model.Metadata.Material> metadata, ICommand saveCommand)
    {
        Guid projectId = cultivateProject.InnerId;
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            List<InventoryItem> entities = appDbContext.InventoryItems
                .Where(a => a.ProjectId == projectId)
                .ToList();

            List<BindingInventoryItem> results = new();
            foreach (Model.Metadata.Material meta in metadata.Where(m => m.IsInventoryItem()).OrderBy(m => m.Id))
            {
                InventoryItem entity = entities.SingleOrDefault(e => e.ItemId == meta.Id) ?? InventoryItem.Create(projectId, meta.Id);
                results.Add(new(entity, meta, saveCommand));
            }

            return results;
        }
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<BindingCultivateEntry>> GetCultivateEntriesAsync(CultivateProject cultivateProject)
    {
        await ThreadHelper.SwitchToBackgroundAsync();
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            IMetadataService metadataService = scope.ServiceProvider.GetRequiredService<IMetadataService>();

            List<Model.Metadata.Material> materials = await metadataService.GetMaterialsAsync().ConfigureAwait(false);
            Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            Dictionary<WeaponId, Model.Metadata.Weapon.Weapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync().ConfigureAwait(false);

            List<BindingCultivateEntry> results = new();
            List<CultivateEntry> entries = await appDbContext.CultivateEntries
                .Where(e => e.ProjectId == cultivateProject.InnerId)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (CultivateEntry entry in entries)
            {
                Guid entryId = entry.InnerId;

                List<BindingCultivateItem> resultItems = new();

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
    public async Task<ObservableCollection<BindingStatisticsItem>> GetStatisticsCultivateItemCollectionAsync(CultivateProject cultivateProject, CancellationToken token)
    {
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            List<BindingStatisticsItem> resultItems = new();
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            List<Model.Metadata.Material> materials = await scope.ServiceProvider
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

                    if (resultItems.SingleOrDefault(i => i.Inner.Id == item.ItemId) is BindingStatisticsItem existedItem)
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
                if (resultItems.SingleOrDefault(i => i.Inner.Id == inventoryItem.ItemId) is BindingStatisticsItem existedItem)
                {
                    existedItem.TotalCount += inventoryItem.Count;
                }
            }

            token.ThrowIfCancellationRequested();

            await ThreadHelper.SwitchToMainThreadAsync();
            return resultItems.OrderByDescending(i => i.Count).ToObservableCollection();
        }
    }

    /// <inheritdoc/>
    public async Task RemoveCultivateEntryAsync(Guid entryId)
    {
        await ThreadHelper.SwitchToBackgroundAsync();
        IEnumerable<CultivateItem> removed;
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            removed = await GetEntryItemsAsync(appDbContext, entryId).ConfigureAwait(false);
            await appDbContext.CultivateEntries.Where(i => i.InnerId == entryId).ExecuteDeleteAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public void SaveInventoryItem(BindingInventoryItem item)
    {
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<AppDbContext>().InventoryItems.UpdateAndSave(item.Entity);
        }
    }

    /// <inheritdoc/>
    public void SaveCultivateItem(CultivateItem item)
    {
        using (IServiceScope scope = scopeFactory.CreateScope())
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

        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                Current ??= appDbContext.CultivateProjects.AsNoTracking().SingleOrDefault(proj => proj.IsSelected);
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

    private static Task<List<InventoryItem>> GetProjectInventoryAsync(AppDbContext appDbContext, Guid projectId)
    {
        return appDbContext.InventoryItems
            .AsNoTracking()
            .Where(e => e.ProjectId == projectId)
            .ToListAsync();
    }

    private static Task<List<CultivateEntry>> GetProjectEntriesAsync(AppDbContext appDbContext, Guid projectId)
    {
        return appDbContext.CultivateEntries
            .AsNoTracking()
            .Where(e => e.ProjectId == projectId)
            .ToListAsync();
    }

    private static Task<List<CultivateItem>> GetEntryItemsAsync(AppDbContext appDbContext, Guid entryId)
    {
        return appDbContext.CultivateItems
            .AsNoTracking()
            .Where(i => i.EntryId == entryId)
            .OrderBy(i => i.ItemId)
            .ToListAsync();
    }
}