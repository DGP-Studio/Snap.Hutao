// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.Inventory;

internal interface IInventoryDbService : IAppDbService<InventoryItem>
{
    void AddInventoryItemRangeByProjectId(List<InventoryItem> items);

    void RemoveInventoryItemRangeByProjectId(Guid projectId);

    void UpdateInventoryItem(InventoryItem item);

    List<InventoryItem> GetInventoryItemListByProjectId(Guid projectId);
}