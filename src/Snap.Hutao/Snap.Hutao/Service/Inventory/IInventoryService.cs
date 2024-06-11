// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.ViewModel.Cultivation;

namespace Snap.Hutao.Service.Inventory;

internal interface IInventoryService
{
    List<InventoryItemView> GetInventoryItemViews(CultivateProject cultivateProject, ICultivationMetadataContext context, ICommand saveCommand);

    void SaveInventoryItem(InventoryItemView item);

    ValueTask RefreshInventoryAsync(CultivateProject project);
}