// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.Inventory;

internal interface IInventoryDbService : IAppDbService<InventoryItem>
{
    ValueTask AddInventoryItemRangeByProjectIdAsync(List<InventoryItem> items, CancellationToken token = default);

    ValueTask RemoveInventoryItemRangeByProjectIdAsync(Guid projectId, CancellationToken token = default);

    void UpdateInventoryItem(InventoryItem item);

    ValueTask UpdateInventoryItemAsync(InventoryItem item, CancellationToken token = default);

    List<InventoryItem> GetInventoryItemListByProjectId(Guid projectId);

    ValueTask<List<InventoryItem>> GetInventoryItemListByProjectIdAsync(Guid projectId, CancellationToken token = default);
}