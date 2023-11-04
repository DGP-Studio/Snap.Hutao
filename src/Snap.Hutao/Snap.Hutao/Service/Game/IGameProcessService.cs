// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game;

internal interface IGameProcessService
{
    bool IsGameRunning();

    ValueTask LaunchAsync(IProgress<LaunchStatus> progress);
}