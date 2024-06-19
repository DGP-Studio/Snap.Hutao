// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Cultivation;

internal interface ICultivationDbService : IAppDbService<CultivateEntryLevelInformation>,
    IAppDbService<CultivateProject>,
    IAppDbService<CultivateEntry>,
    IAppDbService<CultivateItem>
{
    ValueTask AddCultivateProjectAsync(CultivateProject project, CancellationToken token = default);

    ValueTask RemoveCultivateEntryByIdAsync(Guid entryId, CancellationToken token = default);

    ValueTask RemoveCultivateItemRangeByEntryIdAsync(Guid entryId, CancellationToken token = default);

    ValueTask RemoveCultivateProjectByIdAsync(Guid projectId, CancellationToken token = default);

    ValueTask<CultivateEntry?> GetCultivateEntryByProjectIdAndItemIdAsync(Guid projectId, uint itemId, CancellationToken token = default);

    ValueTask<List<CultivateEntry>> GetCultivateEntryListByProjectIdAsync(Guid projectId, CancellationToken token = default);

    ValueTask<List<CultivateItem>> GetCultivateItemListByEntryIdAsync(Guid entryId, CancellationToken token = default);

    ObservableCollection<CultivateProject> GetCultivateProjectCollection();

    ValueTask AddCultivateEntryAsync(CultivateEntry entry, CancellationToken token = default);

    ValueTask AddCultivateItemRangeAsync(IEnumerable<CultivateItem> toAdd, CancellationToken token = default);

    void UpdateCultivateItem(CultivateItem item);

    ValueTask UpdateCultivateItemAsync(CultivateItem item, CancellationToken token = default);

    ValueTask RemoveLevelInformationByEntryIdAsync(Guid entryId, CancellationToken token = default);

    ValueTask AddLevelInformationAsync(CultivateEntryLevelInformation levelInformation, CancellationToken token = default);

    ValueTask<List<CultivateEntry>> GetCultivateEntryListIncludingLevelInformationByProjectIdAsync(Guid projectId, CancellationToken token = default);
}