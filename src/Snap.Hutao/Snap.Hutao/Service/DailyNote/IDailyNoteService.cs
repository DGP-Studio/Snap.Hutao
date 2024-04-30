// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.User;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.DailyNote;

[HighQuality]
internal interface IDailyNoteService
{
    ValueTask AddDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<ObservableCollection<DailyNoteEntry>> GetDailyNoteEntryCollectionAsync(bool forceRefresh = false, CancellationToken token = default);

    ValueTask RefreshDailyNotesAsync(CancellationToken token = default);

    ValueTask RemoveDailyNoteAsync(DailyNoteEntry entry, CancellationToken token = default);

    ValueTask UpdateDailyNoteAsync(DailyNoteEntry entry, CancellationToken token = default);
}