// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Update;

internal interface IUpdateService
{
    string? UpdateInfo { get; }

    ValueTask<CheckUpdateResult> CheckUpdateAsync(CancellationToken token = default);

    ValueTask TriggerUpdateAsync(CheckUpdateResult result, CancellationToken token = default);
}