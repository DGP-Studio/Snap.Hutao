// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game;

internal enum LaunchPhase
{
    ProcessInitializing,
    ProcessStarted,
    UnlockingFps,
    UnlockFpsSucceed,
    UnlockFpsFailed,
    WaitingForExit,
    ProcessExited,
}