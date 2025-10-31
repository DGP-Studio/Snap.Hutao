// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Cultivation;

[Service(ServiceLifetime.Singleton, typeof(ICultivationRepository))]
internal sealed partial class CultivationRepository : ICultivationRepository
{
    [GeneratedConstructor]
    public partial CultivationRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public ImmutableArray<CultivateEntry> GetCultivateEntryImmutableArrayByProjectId(Guid projectId)
    {
        return this.ImmutableArray<CultivateEntry>(e => e.ProjectId == projectId);
    }

    public ImmutableArray<CultivateEntry> GetCultivateEntryImmutableArrayIncludingLevelInformationByProjectId(Guid projectId)
    {
        return this.ImmutableArray<CultivateEntry, CultivateEntry>(query => query.Where(e => e.ProjectId == projectId).Include(e => e.LevelInformation));
    }

    public ImmutableArray<CultivateItem> GetCultivateItemImmutableArrayByEntryId(Guid entryId)
    {
        return this.ImmutableArray<CultivateItem, CultivateItem>(query => query.Where(i => i.EntryId == entryId).OrderBy(i => i.ItemId));
    }

    public void RemoveCultivateEntryById(Guid entryId)
    {
        this.DeleteByInnerId<CultivateEntry>(entryId);
    }

    public void UpdateCultivateItem(CultivateItem item)
    {
        this.Update(item);
    }

    public ImmutableArray<CultivateEntry> GetCultivateEntryImmutableArrayByProjectIdAndItemId(Guid projectId, uint itemId)
    {
        return this.ImmutableArray<CultivateEntry>(e => e.ProjectId == projectId && e.Id == itemId);
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

    public Guid GetCultivateProjectIdByEntryId(Guid entryId)
    {
        return this.Single<CultivateEntry, Guid>(query => query.Where(entry => entry.InnerId == entryId).Select(entry => entry.InnerId));
    }
}