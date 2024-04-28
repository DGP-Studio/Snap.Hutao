// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.DailyNote;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IDailyNoteDbService))]
internal sealed partial class DailyNoteDbService : IDailyNoteDbService
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public bool ContainsUid(string uid)
    {
        return this.Query(query => query.Any(n => n.Uid == uid));
    }

    public ValueTask<bool> ContainsUidAsync(string uid, CancellationToken token = default)
    {
        return this.QueryAsync(query => query.AnyAsync(n => n.Uid == uid));
    }

    public async ValueTask AddDailyNoteEntryAsync(DailyNoteEntry entry, CancellationToken token = default)
    {
        await this.AddAsync(entry, token).ConfigureAwait(false);
    }

    public async ValueTask DeleteDailyNoteEntryByIdAsync(Guid entryId, CancellationToken token = default)
    {
        await this.DeleteByInnerIdAsync(entryId, token).ConfigureAwait(false);
    }

    public async ValueTask UpdateDailyNoteEntryAsync(DailyNoteEntry entry, CancellationToken token = default)
    {
        await this.UpdateAsync(entry, token).ConfigureAwait(false);
    }

    public List<DailyNoteEntry> GetDailyNoteEntryListIncludingUser()
    {
        return this.List(query => query.Include(n => n.User));
    }

    public ValueTask<List<DailyNoteEntry>> GetDailyNoteEntryListIncludingUserAsync(CancellationToken token = default)
    {
        return this.ListAsync(query => query.Include(n => n.User), token);
    }
}