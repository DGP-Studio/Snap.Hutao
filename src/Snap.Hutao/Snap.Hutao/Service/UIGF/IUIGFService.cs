// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

internal interface IUIGFService
{
    ValueTask ExportAsync(UIGFExportOptions exportOptions, CancellationToken token = default);

    ValueTask ImportAsync(UIGFImportOptions importOptions, CancellationToken token = default);
}