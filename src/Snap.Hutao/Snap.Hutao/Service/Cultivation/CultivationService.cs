// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Primitive;
using System.Collections.ObjectModel;
using BindingCultivateEntry = Snap.Hutao.Model.Binding.Cultivation.CultivateEntry;
using BindingCultivateItem = Snap.Hutao.Model.Binding.Cultivation.CultivateItem;
using BindingInventoryItem = Snap.Hutao.Model.Binding.Inventory.InventoryItem;

namespace Snap.Hutao.Service.Cultivation;

/// <summary>
/// 养成计算服务
/// </summary>
[Injection(InjectAs.Singleton, typeof(ICultivationService))]
internal class CultivationService : ICultivationService
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
                projects = new(appDbContext.CultivateProjects.AsNoTracking().ToList());
            }

            Current ??= projects.SingleOrDefault(proj => proj.IsSelected);
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
                await scope.ServiceProvider.GetRequiredService<AppDbContext>().CultivateProjects.AddAndSaveAsync(project).ConfigureAwait(false);
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
            await scope.ServiceProvider.GetRequiredService<AppDbContext>().CultivateProjects.RemoveAndSaveAsync(project).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public List<BindingInventoryItem> GetInventoryItems(CultivateProject cultivateProject, List<Model.Metadata.Material> metadata)
    {
        Guid projectId = cultivateProject.InnerId;
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            List<InventoryItem> entities = appDbContext.InventoryItems
                .Where(a => a.ProjectId == projectId)
                .ToList();

            List<BindingInventoryItem> results = new();
            foreach (Model.Metadata.Material meta in metadata.Where(IsInventoryItem).OrderBy(m => m.Id))
            {
                InventoryItem entity = entities.SingleOrDefault(e => e.ItemId == meta.Id) ?? InventoryItem.Create(projectId, meta.Id);
                results.Add(new(meta, entity));
            }

            return results;
        }
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<BindingCultivateEntry>> GetCultivateEntriesAsync(
        CultivateProject cultivateProject,
        List<Model.Metadata.Material> materials,
        Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> idAvatarMap,
        Dictionary<WeaponId, Model.Metadata.Weapon.Weapon> idWeaponMap)
    {
        await ThreadHelper.SwitchToBackgroundAsync();
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Guid projectId = cultivateProject.InnerId;

            List<BindingCultivateEntry> results = new();
            List<CultivateEntry> entries = await appDbContext.CultivateEntries
                .Where(e => e.ProjectId == projectId)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (CultivateEntry entry in entries)
            {
                Guid entryId = entry.InnerId;

                List<BindingCultivateItem> resultItems = new();
                List<CultivateItem> items = await appDbContext.CultivateItems
                    .Where(i => i.EntryId == entryId)
                    .OrderBy(i => i.ItemId)
                    .ToListAsync()
                    .ConfigureAwait(false);

                foreach (CultivateItem item in items)
                {
                    resultItems.Add(new(materials.Single(m => m.Id == item.ItemId), item));
                }

                Model.Binding.Gacha.Abstraction.ItemBase itemBase = entry.Type switch
                {
                    Model.Binding.Cultivation.CultivateType.AvatarAndSkill => idAvatarMap[entry.Id].ToItemBase(),
                    Model.Binding.Cultivation.CultivateType.Weapon => idWeaponMap[entry.Id].ToItemBase(),
                    _ => null!, // TODO: support furniture calc
                };

                results.Add(new(entry, itemBase, resultItems));
            }

            return new(results.OrderByDescending(e => e.Items.Any(i => i.IsToday)));
        }
    }

    /// <inheritdoc/>
    public async Task<List<Model.Binding.Cultivation.StatisticsCultivateItem>> GetStatisticsCultivateItemsAsync(CultivateProject cultivateProject, List<Model.Metadata.Material> materials)
    {
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Guid projectId = cultivateProject.InnerId;

            List<Model.Binding.Cultivation.StatisticsCultivateItem> resultItems = new();

            List<CultivateEntry> entries = await appDbContext.CultivateEntries
                .AsNoTracking()
                .Where(e => e.ProjectId == projectId)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (CultivateEntry entry in entries)
            {
                Guid entryId = entry.InnerId;

                List<CultivateItem> items = await appDbContext.CultivateItems
                    .AsNoTracking()
                    .Where(i => i.EntryId == entryId)
                    .OrderBy(i => i.ItemId)
                    .ToListAsync()
                    .ConfigureAwait(false);

                foreach (CultivateItem item in items)
                {
                    if (item.IsFinished)
                    {
                        continue;
                    }

                    if (resultItems.SingleOrDefault(i => i.Inner.Id == item.ItemId) is Model.Binding.Cultivation.StatisticsCultivateItem inPlaceItem)
                    {
                        inPlaceItem.Count += item.Count;
                    }
                    else
                    {
                        resultItems.Add(new(materials.Single(m => m.Id == item.ItemId), item));
                    }
                }
            }

            List<InventoryItem> inventoryItems = await appDbContext.InventoryItems
                .AsNoTracking()
                .Where(e => e.ProjectId == projectId)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (InventoryItem inventoryItem in inventoryItems)
            {
                if (resultItems.SingleOrDefault(i => i.Inner.Id == inventoryItem.ItemId) is Model.Binding.Cultivation.StatisticsCultivateItem inPlaceItem)
                {
                    inPlaceItem.TotalCount += inventoryItem.Count;
                }
            }

            return resultItems.OrderByDescending(i => i.Count).ToList();
        }
    }

    /// <inheritdoc/>
    public async Task RemoveCultivateEntryAsync(Guid entryId)
    {
        await ThreadHelper.SwitchToBackgroundAsync();
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
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
    public async Task<bool> SaveConsumptionAsync(Model.Binding.Cultivation.CultivateType type, int itemId, List<Web.Hoyolab.Takumi.Event.Calculate.Item> items)
    {
        if (items.Count == 0)
        {
            return true;
        }

        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Current ??= appDbContext.CultivateProjects.AsNoTracking().SingleOrDefault(proj => proj.IsSelected);
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
            await appDbContext.CultivateItems.Where(i => i.EntryId == entryId).ExecuteDeleteAsync().ConfigureAwait(false);

            IEnumerable<CultivateItem> toAdd = items.Select(i => CultivateItem.Create(entryId, i.Id, i.Num));
            await appDbContext.CultivateItems.AddRangeAndSaveAsync(toAdd).ConfigureAwait(false);
        }

        return true;
    }

    private bool IsInventoryItem(Model.Metadata.Material material)
    {
        // 原质
        if (material.Id == 112001)
        {
            return false;
        }

        // 摩拉
        if (material.Id == 202)
        {
            return true;
        }

        if (material.TypeDescription.EndsWith("区域特产"))
        {
            return true;
        }

        return material.TypeDescription switch
        {
            "角色经验素材" => true,
            "角色培养素材" => true,
            "天赋培养素材" => true,
            "武器强化素材" => true,
            _ => false,
        };
    }
}
