// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Inventory;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IInventoryRepository))]
internal sealed partial class InventoryRepository : IInventoryRepository
{
    public partial IServiceProvider ServiceProvider { get; }

    public void RemoveInventoryItemRangeByProjectId(Guid projectId)
    {
        this.Delete(i => i.ProjectId == projectId);
    }

    public void RemoveAllInventoryItem()
    {
        this.Delete();
    }

    public void AddInventoryItemRangeByProjectId(List<InventoryItem> items)
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
}