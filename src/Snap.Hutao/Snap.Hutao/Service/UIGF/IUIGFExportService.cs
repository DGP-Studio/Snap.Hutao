// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

internal interface IUIGFExportService
{
    ValueTask ExportAsync(UIGFExportOptions exportOptions, CancellationToken token);
}