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
    void AddCultivateProject(CultivateProject project);

    void RemoveCultivateEntryById(Guid entryId);

    void RemoveCultivateItemRangeByEntryId(Guid entryId);

    void RemoveCultivateProjectById(Guid projectId);

    CultivateEntry? GetCultivateEntryByProjectIdAndItemId(Guid projectId, uint itemId);

    List<CultivateEntry> GetCultivateEntryListByProjectId(Guid projectId);

    List<CultivateItem> GetCultivateItemListByEntryId(Guid entryId);

    ObservableCollection<CultivateProject> GetCultivateProjectCollection();

    void AddCultivateEntry(CultivateEntry entry);

    void AddCultivateItemRange(IEnumerable<CultivateItem> toAdd);

    void UpdateCultivateItem(CultivateItem item);

    void RemoveLevelInformationByEntryId(Guid entryId);

    void AddLevelInformation(CultivateEntryLevelInformation levelInformation);

    List<CultivateEntry> GetCultivateEntryListIncludingLevelInformationByProjectId(Guid projectId);
}