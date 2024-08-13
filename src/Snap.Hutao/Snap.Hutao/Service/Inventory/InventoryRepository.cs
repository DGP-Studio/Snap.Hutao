// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.Inventory;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IInventoryRepository))]
internal sealed partial class InventoryRepository : IInventoryRepository
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public void RemoveInventoryItemRangeByProjectId(Guid projectId)
    {
        this.Delete(i => i.ProjectId == projectId);
    }

    public void AddInventoryItemRangeByProjectId(List<InventoryItem> items)
    {
        this.AddRange(items);
    }

    public void UpdateInventoryItem(InventoryItem item)
    {
        this.Update(item);
    }

    public List<InventoryItem> GetInventoryItemListByProjectId(Guid projectId)
    {
        return this.List(i => i.ProjectId == projectId);
    }
}