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

    public ValueTask<List<CultivateEntry>> GetCultivateEntryListByProjectIdAsync(Guid projectId, CancellationToken token = default)
    {
        return this.ListAsync<CultivateEntry>(e => e.ProjectId == projectId, token);
    }

    public ValueTask<List<CultivateEntry>> GetCultivateEntryListIncludingLevelInformationByProjectIdAsync(Guid projectId, CancellationToken token = default)
    {
        return this.ListAsync<CultivateEntry, CultivateEntry>(query => query.Where(e => e.ProjectId == projectId).Include(e => e.LevelInformation), token);
    }

    public ValueTask<List<CultivateItem>> GetCultivateItemListByEntryIdAsync(Guid entryId, CancellationToken token = default)
    {
        return this.ListAsync<CultivateItem, CultivateItem>(query => query.Where(i => i.EntryId == entryId).OrderBy(i => i.ItemId), token);
    }

    public async ValueTask RemoveCultivateEntryByIdAsync(Guid entryId, CancellationToken token = default)
    {
        await this.DeleteByInnerIdAsync<CultivateEntry>(entryId, token).ConfigureAwait(false);
    }

    public void UpdateCultivateItem(CultivateItem item)
    {
        this.Update(item);
    }

    public async ValueTask UpdateCultivateItemAsync(CultivateItem item, CancellationToken token = default)
    {
        await this.UpdateAsync(item, token).ConfigureAwait(false);
    }

    public async ValueTask<CultivateEntry?> GetCultivateEntryByProjectIdAndItemIdAsync(Guid projectId, uint itemId, CancellationToken token = default)
    {
        return await this.SingleOrDefaultAsync<CultivateEntry>(e => e.ProjectId == projectId && e.Id == itemId, token).ConfigureAwait(false);
    }

    public async ValueTask AddCultivateEntryAsync(CultivateEntry entry, CancellationToken token = default)
    {
        await this.AddAsync(entry, token).ConfigureAwait(false);
    }

    public async ValueTask RemoveCultivateItemRangeByEntryIdAsync(Guid entryId, CancellationToken token = default)
    {
        await this.DeleteAsync<CultivateItem>(i => i.EntryId == entryId, token).ConfigureAwait(false);
    }

    public async ValueTask AddCultivateItemRangeAsync(IEnumerable<CultivateItem> toAdd, CancellationToken token = default)
    {
        await this.AddRangeAsync(toAdd, token).ConfigureAwait(false);
    }

    public async ValueTask AddCultivateProjectAsync(CultivateProject project, CancellationToken token = default)
    {
        await this.AddAsync(project, token).ConfigureAwait(false);
    }

    public async ValueTask RemoveCultivateProjectByIdAsync(Guid projectId, CancellationToken token = default)
    {
        await this.DeleteByInnerIdAsync<CultivateProject>(projectId, token).ConfigureAwait(false);
    }

    public ObservableCollection<CultivateProject> GetCultivateProjectCollection()
    {
        return this.ObservableCollection<CultivateProject>();
    }

    public async ValueTask RemoveLevelInformationByEntryIdAsync(Guid entryId, CancellationToken token = default)
    {
        await this.DeleteAsync<CultivateEntryLevelInformation>(l => l.EntryId == entryId, token).ConfigureAwait(false);
    }

    public async ValueTask AddLevelInformationAsync(CultivateEntryLevelInformation levelInformation, CancellationToken token = default)
    {
        await this.AddAsync(levelInformation, token).ConfigureAwait(false);
    }
}