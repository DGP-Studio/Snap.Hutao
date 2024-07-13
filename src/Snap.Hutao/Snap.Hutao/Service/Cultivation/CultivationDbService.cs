// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Cultivation;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(ICultivationDbService))]
internal sealed partial class CultivationDbService : ICultivationDbService
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public List<CultivateEntry> GetCultivateEntryListByProjectId(Guid projectId)
    {
        return this.List<CultivateEntry>(e => e.ProjectId == projectId);
    }

    public List<CultivateEntry> GetCultivateEntryListIncludingLevelInformationByProjectId(Guid projectId)
    {
        return this.List<CultivateEntry, CultivateEntry>(query => query.Where(e => e.ProjectId == projectId).Include(e => e.LevelInformation));
    }

    public List<CultivateItem> GetCultivateItemListByEntryId(Guid entryId)
    {
        return this.List<CultivateItem, CultivateItem>(query => query.Where(i => i.EntryId == entryId).OrderBy(i => i.ItemId));
    }

    public void RemoveCultivateEntryById(Guid entryId)
    {
        this.DeleteByInnerId<CultivateEntry>(entryId);
    }

    public void UpdateCultivateItem(CultivateItem item)
    {
        this.Update(item);
    }

    public CultivateEntry? GetCultivateEntryByProjectIdAndItemId(Guid projectId, uint itemId)
    {
        return this.SingleOrDefault<CultivateEntry>(e => e.ProjectId == projectId && e.Id == itemId);
    }

    public void AddCultivateEntry(CultivateEntry entry)
    {
        this.Add(entry);
    }

    public void RemoveCultivateItemRangeByEntryId(Guid entryId)
    {
        this.Delete<CultivateItem>(i => i.EntryId == entryId);
    }

    public void AddCultivateItemRange(IEnumerable<CultivateItem> toAdd)
    {
        this.AddRange(toAdd);
    }

    public void AddCultivateProject(CultivateProject project)
    {
        this.Add(project);
    }

    public void RemoveCultivateProjectById(Guid projectId)
    {
        this.DeleteByInnerId<CultivateProject>(projectId);
    }

    public ObservableCollection<CultivateProject> GetCultivateProjectCollection()
    {
        return this.ObservableCollection<CultivateProject>();
    }

    public void RemoveLevelInformationByEntryId(Guid entryId)
    {
        this.Delete<CultivateEntryLevelInformation>(l => l.EntryId == entryId);
    }

    public void AddLevelInformation(CultivateEntryLevelInformation levelInformation)
    {
        this.Add(levelInformation);
    }

    public CultivateProject? GetCultivateProjectById(Guid projectId)
    {
        return this.SingleOrDefault<CultivateProject>(p => p.InnerId == projectId);
    }
}