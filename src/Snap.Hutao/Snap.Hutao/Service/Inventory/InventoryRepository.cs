// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Inventory;

[Service(ServiceLifetime.Singleton, typeof(IInventoryRepository))]
internal sealed partial class InventoryRepository : IInventoryRepository
{
    [GeneratedConstructor]
    public partial InventoryRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public void RemoveInventoryItemRangeByProjectId(Guid projectId)
    {
        this.Delete(i => i.ProjectId == projectId);
    }

    public void AddInventoryItemRangeByProjectId(IEnumerable<InventoryItem> items)
    {
        this.AddRange(items);
    }

    public void UpdateInventoryItem(InventoryItem item)
    {
        this.Update(item);
    }

    public ImmutableArray<InventoryItem> GetInventoryItemImmutableArrayByProjectId(Guid projectId)
    {
        return this.ImmutableArray(i => i.ProjectId == projectId);
    }

    public ImmutableDictionary<uint, InventoryItem> GetInventoryItemImmutableDictionaryByProjectId(Guid projectId)
    {
        return this.Query(query => query.Where(i => i.ProjectId == projectId).ToImmutableDictionary(i => i.ItemId));
    }
}