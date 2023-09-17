// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
}
