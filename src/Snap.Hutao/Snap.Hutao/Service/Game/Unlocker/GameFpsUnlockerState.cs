// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 解锁状态
/// </summary>
internal sealed class GameFpsUnlockerState
{
    public string Description { get; set; } = default!;

    public FindModuleResult FindModuleResult { get; set; }

    public bool IsUnlockerValid { get; set; } = true;

    public nuint FpsAddress { get; set; }

    public UnlockTimingOptions TimingOptions { get; set; }

    public Process GameProcess { get; set; } = default!;

    public IProgress<GameFpsUnlockerState> Progress { get; set; } = default!;

    public void Report()
    {
        Progress.Report(this);
    }
}