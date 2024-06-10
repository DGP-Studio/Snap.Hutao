// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.Inventory;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IInventoryDbService))]
internal sealed partial class InventoryDbService : IInventoryDbService
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public async ValueTask RemoveInventoryItemRangeByProjectIdAsync(Guid projectId, CancellationToken token = default)
    {
        await this.DeleteAsync(i => i.ProjectId == projectId, token).ConfigureAwait(false);
    }

    public async ValueTask AddInventoryItemRangeByProjectIdAsync(List<InventoryItem> items, CancellationToken token = default)
    {
        await this.AddRangeAsync(items, token).ConfigureAwait(false);
    }

    public void UpdateInventoryItem(InventoryItem item)
    {
        this.Update(item);
    }

    public async ValueTask UpdateInventoryItemAsync(InventoryItem item, CancellationToken token = default)
    {
        await this.UpdateAsync(item, token).ConfigureAwait(false);
    }

    public List<InventoryItem> GetInventoryItemListByProjectId(Guid projectId)
    {
        return this.List(i => i.ProjectId == projectId);
    }

    public ValueTask<List<InventoryItem>> GetInventoryItemListByProjectIdAsync(Guid projectId, CancellationToken token = default)
    {
        return this.ListAsync(i => i.ProjectId == projectId, token);
    }
}
