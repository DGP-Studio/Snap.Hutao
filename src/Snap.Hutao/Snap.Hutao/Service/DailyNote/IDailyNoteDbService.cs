// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.DailyNote;

internal interface IDailyNoteDbService : IAppDbService<DailyNoteEntry>
{
    void AddDailyNoteEntry(DailyNoteEntry entry);

    bool ContainsUid(string uid);

    void DeleteDailyNoteEntryById(Guid entryId);

    List<DailyNoteEntry> GetDailyNoteEntryListIncludingUser();

    void UpdateDailyNoteEntry(DailyNoteEntry entry);
}