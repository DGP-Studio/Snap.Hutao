// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Unlocker;

namespace Snap.Hutao.Service.Game;

internal sealed class LaunchStatus
{
    public LaunchStatus(LaunchPhase phase, string description)
    {
        Phase = phase;
        Description = description;
    }

    public LaunchPhase Phase { get; set; }

    public string Description { get; set; }

    public static LaunchStatus FromUnlockerContext(GameFpsUnlockerContext unlockerState)
    {
        if (unlockerState.FindModuleResult == FindModuleResult.Ok)
        {
            return new(LaunchPhase.UnlockFpsSucceed, unlockerState.Description ?? SH.ServiceGameLaunchPhaseUnlockFpsSucceed);
        }
        else
        {
            return new(LaunchPhase.UnlockFpsFailed, unlockerState.Description ?? SH.ServiceGameLaunchPhaseUnlockFpsFailed);
        }
    }
}