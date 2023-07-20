// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Service.Cultivation;

internal interface ICultivationDbService
{
    ValueTask DeleteCultivateEntryByIdAsync(Guid entryId);

    ValueTask<List<CultivateEntry>> GetCultivateEntryListByProjectIdAsync(Guid projectId);

    ValueTask<List<CultivateItem>> GetCultivateItemListByEntryIdAsync(Guid entryId);

    List<InventoryItem> GetInventoryItemListByProjectId(Guid projectId);

    ValueTask<List<InventoryItem>> GetInventoryItemListByProjectIdAsync(Guid projectId);

    void UpdateCultivateItem(CultivateItem item);

    void UpdateInventoryItem(InventoryItem item);
}