// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class ResumeLaunchExecutionEnsureGameRunningHandler : AbstractLaunchExecutionHandler
{
    public override ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        if (GameLifeCycle.IsGameRunning(out IProcess? gameProcess))
        {
            context.SetOption(LaunchExecutionOptionsKey.RunningProcess, gameProcess);
        }

        return ValueTask.CompletedTask;
    }
}