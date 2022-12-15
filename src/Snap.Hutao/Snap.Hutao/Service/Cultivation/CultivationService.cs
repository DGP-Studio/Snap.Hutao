// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Binding.Cultivation;
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
        List<Model.Metadata.Material> metadata,
        Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> idAvatarMap,
        Dictionary<WeaponId, Model.Metadata.Weapon.Weapon> idWeaponMap)
    {
        // TODO: cache the collection
        await ThreadHelper.SwitchToBackgroundAsync();
        Guid projectId = cultivateProject.InnerId;
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            List<BindingCultivateEntry> bindingEntries = new();
            foreach (Model.Entity.CultivateEntry? entry in await appDbContext.CultivateEntries.ToListAsync().ConfigureAwait(false))
            {
                Guid entryId = entry.InnerId;

                List<BindingCultivateItem> items = new();
                foreach (Model.Entity.CultivateItem? item in await appDbContext.CultivateItems.Where(i => i.EntryId == entryId).OrderBy(i => i.ItemId).ToListAsync().ConfigureAwait(false))
                {
                    items.Add(new(metadata.Single(m => m.Id == item.ItemId), item));
                }

                Model.Binding.Gacha.Abstraction.ItemBase itemBase = entry.Type switch
                {
                    CultivateType.AvatarAndSkill => idAvatarMap[entry.Id].ToItemBase(),
                    CultivateType.Weapon => idWeaponMap[entry.Id].ToItemBase(),
                    _ => null!, // TODO: support furniture calc
                };

                bindingEntries.Add(new()
                {
                    Id = entry.Id,
                    EntryId = entryId,
                    Name = itemBase.Name,
                    Icon = itemBase.Icon,
                    Badge = itemBase.Badge,
                    Quality = itemBase.Quality,
                    Items = items,
                });
            }

            return new(bindingEntries);
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
    public async Task<bool> SaveConsumptionAsync(CultivateType type, int itemId, List<Web.Hoyolab.Takumi.Event.Calculate.Item> items)
    {
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Current ??= appDbContext.CultivateProjects.AsNoTracking().SingleOrDefault(proj => proj.IsSelected);
            if (Current == null)
            {
                return false;
            }

            Guid projectId = Current!.InnerId;
            Model.Entity.CultivateEntry? entry = await appDbContext.CultivateEntries
                .SingleOrDefaultAsync(e => e.ProjectId == projectId && e.Id == itemId)
                .ConfigureAwait(false);

            if (entry == null)
            {
                entry = Model.Entity.CultivateEntry.Create(projectId, type, itemId);
                await appDbContext.CultivateEntries.AddAndSaveAsync(entry).ConfigureAwait(false);
            }

            Guid entryId = entry.InnerId;
            await appDbContext.CultivateItems.Where(i => i.EntryId == entryId).ExecuteDeleteAsync().ConfigureAwait(false);
            IEnumerable<Model.Entity.CultivateItem> toAdd = items.Select(i => Model.Entity.CultivateItem.Create(entryId, i.Id, i.Num));
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