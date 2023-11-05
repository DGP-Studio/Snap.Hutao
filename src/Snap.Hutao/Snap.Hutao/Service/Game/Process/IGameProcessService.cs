// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Process;

internal interface IGameProcessService
{
    bool IsGameRunning();

    ValueTask LaunchAsync(IProgress<LaunchStatus> progress);
}