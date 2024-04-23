// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;

namespace Snap.Hutao.Service.DailyNote;

internal class DailyNoteMetadataContext : IMetadataContext,
    IMetadataListChapterSource
{
    public List<Chapter> Chapters { get; set; } = default!;
}
