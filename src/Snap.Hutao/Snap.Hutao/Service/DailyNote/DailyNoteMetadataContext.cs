// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableArray;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.DailyNote;

internal class DailyNoteMetadataContext : IMetadataContext,
    IMetadataArrayChapterSource
{
    public ImmutableArray<Chapter> Chapters { get; set; } = default!;
}
