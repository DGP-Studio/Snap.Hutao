// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.UIGF;

internal interface IUIGFImportService
{
    ValueTask<bool> ImportAsync(UIGFImportOptions importOptions, CancellationToken token);
}