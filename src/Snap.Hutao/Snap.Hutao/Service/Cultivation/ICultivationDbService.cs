// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Cultivation;

internal interface ICultivationDbService
{
    ValueTask AddCultivateProjectAsync(CultivateProject project);

    ValueTask DeleteCultivateEntryByIdAsync(Guid entryId);

    ValueTask DeleteCultivateItemRangeByEntryIdAsync(Guid entryId);

    ValueTask DeleteCultivateProjectByIdAsync(Guid projectId);

    ValueTask<CultivateEntry?> GetCultivateEntryByProjectIdAndItemIdAsync(Guid projectId, uint itemId);

    ValueTask<List<CultivateEntry>> GetCultivateEntryListByProjectIdAsync(Guid projectId);

    ValueTask<List<CultivateItem>> GetCultivateItemListByEntryIdAsync(Guid entryId);

    ObservableCollection<CultivateProject> GetCultivateProjectCollection();

    List<InventoryItem> GetInventoryItemListByProjectId(Guid projectId);

    ValueTask<List<InventoryItem>> GetInventoryItemListByProjectIdAsync(Guid projectId);

    ValueTask InsertCultivateEntryAsync(CultivateEntry entry);

    ValueTask InsertCultivateItemRangeAsync(IEnumerable<CultivateItem> toAdd);

    void UpdateCultivateItem(CultivateItem item);

    void UpdateInventoryItem(InventoryItem item);
}