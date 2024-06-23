// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Diagnostics;

namespace Snap.Hutao.Service.Game.Unlocker;

internal sealed class GameFpsUnlockerContext
{
    public string Description { get; set; } = default!;

    public FindModuleResult FindModuleResult { get; set; }

    public bool IsUnlockerValid { get; set; } = true;

    public nuint FpsAddress { get; set; }

    public UnlockOptions Options { get; set; }

    public Process GameProcess { get; set; } = default!;

    public HANDLE AllAccess { get; set; }

    public IProgress<GameFpsUnlockerContext> Progress { get; set; } = default!;

    public ILogger Logger { get; set; } = default!;

    public void Report()
    {
        Progress.Report(this);
    }
}