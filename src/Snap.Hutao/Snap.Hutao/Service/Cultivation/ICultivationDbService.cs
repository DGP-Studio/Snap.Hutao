// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Cultivation;

internal interface ICultivationDbService
{
    ValueTask AddCultivateProjectAsync(CultivateProject project);

    ValueTask RemoveCultivateEntryByIdAsync(Guid entryId);

    ValueTask RemoveCultivateItemRangeByEntryIdAsync(Guid entryId);

    ValueTask RemoveCultivateProjectByIdAsync(Guid projectId);

    ValueTask<CultivateEntry?> GetCultivateEntryByProjectIdAndItemIdAsync(Guid projectId, uint itemId);

    ValueTask<List<CultivateEntry>> GetCultivateEntryListByProjectIdAsync(Guid projectId);

    ValueTask<List<CultivateItem>> GetCultivateItemListByEntryIdAsync(Guid entryId);

    ObservableCollection<CultivateProject> GetCultivateProjectCollection();

    List<InventoryItem> GetInventoryItemListByProjectId(Guid projectId);

    ValueTask<List<InventoryItem>> GetInventoryItemListByProjectIdAsync(Guid projectId);

    ValueTask AddCultivateEntryAsync(CultivateEntry entry);

    ValueTask AddCultivateItemRangeAsync(IEnumerable<CultivateItem> toAdd);

    void UpdateCultivateItem(CultivateItem item);

    ValueTask UpdateCultivateItemAsync(CultivateItem item);
}