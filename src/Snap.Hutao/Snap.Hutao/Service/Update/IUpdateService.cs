// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Update;

namespace Snap.Hutao.Service.Abstraction;

internal interface IUpdateService
{
    ValueTask<bool> CheckForUpdateAndDownloadAsync(IProgress<UpdateStatus> progress, CancellationToken token = default);

    void LaunchInstaller();
}