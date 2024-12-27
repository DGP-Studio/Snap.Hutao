// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Cultivation;
using Snap.Hutao.ViewModel.Cultivation;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Inventory;

internal interface IInventoryService
{
    ImmutableArray<InventoryItemView> GetInventoryItemViews(ICultivationMetadataContext context, CultivateProject cultivateProject, ICommand saveCommand);

    void SaveInventoryItem(InventoryItemView item);

    ValueTask RefreshInventoryAsync(ICultivationMetadataContext context, CultivateProject project);
}