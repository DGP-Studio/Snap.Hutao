// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.DailyNote;

internal interface IDailyNoteDbService : IAppDbService<DailyNoteEntry>
{
    ValueTask AddDailyNoteEntryAsync(DailyNoteEntry entry, CancellationToken token = default);

    bool ContainsUid(string uid);

    ValueTask<bool> ContainsUidAsync(string uid, CancellationToken token = default);

    ValueTask DeleteDailyNoteEntryByIdAsync(Guid entryId, CancellationToken token = default);

    List<DailyNoteEntry> GetDailyNoteEntryListIncludingUser();

    ValueTask<List<DailyNoteEntry>> GetDailyNoteEntryListIncludingUserAsync(CancellationToken token = default);

    ValueTask UpdateDailyNoteEntryAsync(DailyNoteEntry entry, CancellationToken token = default);
}