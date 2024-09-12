﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

internal sealed class UIGFExportOptions
{
    public required string FilePath { get; set; }

    public required List<uint> GachaArchiveUids { get; set; }
}