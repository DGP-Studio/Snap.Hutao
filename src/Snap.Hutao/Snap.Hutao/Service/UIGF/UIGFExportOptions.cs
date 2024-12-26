// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Service.UIGF;

internal sealed class UIGFExportOptions
{
    public required string FilePath { get; set; }

    public required ImmutableArray<uint> GachaArchiveUids { get; set; }
}