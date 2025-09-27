// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game;

internal sealed class LaunchStatus
{
    public LaunchStatus(string description)
    {
        Description = description;
    }

    public string Description { get; }
}