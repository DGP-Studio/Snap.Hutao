// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class ResumeLaunchExecutionEnsureGameRunningHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (!LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning(out IProcess? gameProcess))
        {
            return;
        }

        context.Process = gameProcess;
        await next().ConfigureAwait(false);
    }
}