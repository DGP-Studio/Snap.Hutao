// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.DailyNote;

[Service(ServiceLifetime.Singleton, typeof(IDailyNoteRepository))]
internal sealed partial class DailyNoteRepository : IDailyNoteRepository
{
    [GeneratedConstructor]
    public partial DailyNoteRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public bool ContainsUid(string uid)
    {
        return this.Query(query => query.Any(n => n.Uid == uid));
    }

    public void AddDailyNoteEntry(DailyNoteEntry entry)
    {
        this.Add(entry);
    }

    public void DeleteDailyNoteEntryById(Guid entryId)
    {
        this.DeleteByInnerId(entryId);
    }

    public void UpdateDailyNoteEntry(DailyNoteEntry entry)
    {
        this.Update(entry);
    }

    public ImmutableArray<DailyNoteEntry> GetDailyNoteEntryImmutableArrayIncludingUser()
    {
        return this.ImmutableArray(query => query.Include(n => n.User));
    }
}