// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Service.Game.Unlocker;

internal sealed class GameFpsUnlockerContext
{
    public string Description { get; set; } = default!;

    public bool IsUnlockerValid { get; set; } = true;

    public Process GameProcess { get; set; } = default!;

    public IProgress<GameFpsUnlockerContext> Progress { get; set; } = default!;

    public ILogger Logger { get; set; } = default!;

    public void Report()
    {
        Progress.Report(this);
    }
}