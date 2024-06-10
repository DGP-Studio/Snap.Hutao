// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Service.Inventory;

internal interface IInventoryDbService
{
    ValueTask AddInventoryItemRangeByProjectId(List<InventoryItem> items);

    ValueTask RemoveInventoryItemRangeByProjectId(Guid projectId);

    void UpdateInventoryItem(InventoryItem item);

    ValueTask UpdateInventoryItemAsync(InventoryItem item);
}