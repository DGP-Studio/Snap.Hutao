// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.Service.Inventory;
using System.Collections.Immutable;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact.Cultivation;

[Service(ServiceLifetime.Singleton, typeof(IAutomationCultivationService))]
internal sealed partial class AutomationCultivationService : IAutomationCultivationService
{
    private readonly ICultivationRepository cultivationRepository;
    private readonly IInventoryRepository inventoryRepository;

    [GeneratedConstructor]
    public partial AutomationCultivationService(IServiceProvider serviceProvider);

    public AutomationCultivationProject? GetCurrentProject()
    {
        CultivateProject? project = cultivationRepository.SingleOrDefault<CultivateProject>(p => p.IsSelected);

        if (project is null)
        {
            return default;
        }

        Guid projectId = project.InnerId;
        ImmutableArray<CultivateEntry> entries = cultivationRepository.ImmutableArray<CultivateEntry>(e => e.ProjectId == projectId);
        ImmutableArray<AutomationCultivationEntry>.Builder entriesBuilder = ImmutableArray.CreateBuilder<AutomationCultivationEntry>();
        foreach (ref readonly CultivateEntry entry in entries.AsSpan())
        {
            Guid entryId = entry.InnerId;
            ImmutableArray<CultivateItem> items = cultivationRepository.ImmutableArray<CultivateItem>(i => i.EntryId == entryId);
            ImmutableArray<AutomationCultivationItem>.Builder itemsBuilder = ImmutableArray.CreateBuilder<AutomationCultivationItem>();
            foreach (ref readonly CultivateItem item in items.AsSpan())
            {
                itemsBuilder.Add(new()
                {
                    ItemId = item.ItemId,
                    Count = item.Count,
                });
            }

            entriesBuilder.Add(new()
            {
                ItemId = entry.Id,
                Items = itemsBuilder.ToImmutable(),
            });
        }

        ImmutableArray<InventoryItem> inventoryItems = inventoryRepository.ImmutableArray(i => i.ProjectId == projectId);
        ImmutableArray<AutomationInventoryItem>.Builder inventoryItemsBuilder = ImmutableArray.CreateBuilder<AutomationInventoryItem>();
        foreach (ref readonly InventoryItem item in inventoryItems.AsSpan())
        {
            inventoryItemsBuilder.Add(new()
            {
                ItemId = item.ItemId,
                Count = item.Count,
            });
        }

        return new()
        {
            Entries = entriesBuilder.ToImmutable(),
            InventoryItems = inventoryItemsBuilder.ToImmutable(),
        };
    }
}