// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.DailyNote;

internal interface IDailyNoteRepository : IRepository<DailyNoteEntry>
{
    void AddDailyNoteEntry(DailyNoteEntry entry);

    bool ContainsUid(string uid);

    void DeleteDailyNoteEntryById(Guid entryId);

    ImmutableArray<DailyNoteEntry> GetDailyNoteEntryImmutableArrayIncludingUser();

    void UpdateDailyNoteEntry(DailyNoteEntry entry);
}