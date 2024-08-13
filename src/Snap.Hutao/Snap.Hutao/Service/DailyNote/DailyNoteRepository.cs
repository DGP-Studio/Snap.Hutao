// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.DailyNote;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IDailyNoteRepository))]
internal sealed partial class DailyNoteRepository : IDailyNoteRepository
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

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

    public List<DailyNoteEntry> GetDailyNoteEntryListIncludingUser()
    {
        return this.List(query => query.Include(n => n.User));
    }
}