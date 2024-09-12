// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Update;

internal interface IUpdateService
{
    ValueTask<CheckUpdateResult> CheckUpdateAsync(IProgress<UpdateStatus> progress, CancellationToken token = default);

    ValueTask<bool> DownloadUpdateAsync(HutaoSelectedMirrorInformation mirrorInformation, IProgress<UpdateStatus> progress, CancellationToken token = default);

    LaunchUpdaterResult LaunchUpdater();
}