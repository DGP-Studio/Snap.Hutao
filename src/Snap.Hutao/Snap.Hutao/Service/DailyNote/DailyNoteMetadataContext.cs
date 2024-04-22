// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;

namespace Snap.Hutao.Service.DailyNote;

internal class DailyNoteMetadataContext : IDailyNoteMetadataContext
{
    public List<Chapter> Chapters { get; set; } = default!;
}
