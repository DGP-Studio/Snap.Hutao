// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameProcessExitHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (context.Process.IsRunning())
        {
            context.Progress.Report(new(SH.ServiceGameLaunchPhaseWaitingProcessExit));
            try
            {
                await context.TaskContext.SwitchToBackgroundAsync();
                context.Process.WaitForExit();
            }
            catch (Exception ex)
            {
                // Access denied, we are in non-elevated process
                // Just leave and let invoker spin wait
                SentrySdk.CaptureException(ex);
                return;
            }
        }

        // Accessing Process there is unsafe
        context.Progress.Report(new(SH.ServiceGameLaunchPhaseProcessExited));
        await next().ConfigureAwait(false);
    }
}