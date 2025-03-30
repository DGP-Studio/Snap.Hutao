// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Update;

internal interface IUpdateService
{
    ValueTask<CheckUpdateResult> CheckUpdateAsync(CancellationToken token = default);

    ValueTask<bool> LaunchUpdaterAsync();
}