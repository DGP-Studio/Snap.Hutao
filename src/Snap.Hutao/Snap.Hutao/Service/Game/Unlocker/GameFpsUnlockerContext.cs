// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 解锁状态
/// </summary>
internal sealed class GameFpsUnlockerContext
{
    public string Description { get; set; } = default!;

    public FindModuleResult FindModuleResult { get; set; }

    public bool IsUnlockerValid { get; set; } = true;

    public nuint FpsAddress { get; set; }

    public UnlockOptions Options { get; set; }

    public Process GameProcess { get; set; } = default!;

    public IProgress<GameFpsUnlockerContext> Progress { get; set; } = default!;

    public void Report()
    {
        Progress.Report(this);
    }
}