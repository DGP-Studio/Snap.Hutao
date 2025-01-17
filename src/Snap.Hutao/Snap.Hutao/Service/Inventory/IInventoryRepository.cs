// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Inventory;

internal interface IInventoryRepository : IRepository<InventoryItem>
{
    void AddInventoryItemRangeByProjectId(List<InventoryItem> items);

    void RemoveAllInventoryItem();

    void RemoveInventoryItems(Guid projectId);

    void UpdateInventoryItem(InventoryItem item);

    ImmutableArray<InventoryItem> GetInventoryItemImmutableArrayByProjectId(Guid projectId);
}