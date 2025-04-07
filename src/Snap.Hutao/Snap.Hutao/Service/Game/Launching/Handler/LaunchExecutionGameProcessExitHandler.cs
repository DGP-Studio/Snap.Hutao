// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameProcessExitHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (context.Process.IsRunning())
        {
            context.Progress.Report(new(LaunchPhase.WaitingForExit, SH.ServiceGameLaunchPhaseWaitingProcessExit));
            try
            {
                await context.Process.WaitForExitAsync().ConfigureAwait(false);
            }
            catch (Win32Exception)
            {
                // Access denied, we are in non-elevated process
                // Just leave and let invoker spin wait
                return;
            }
        }

        // Accessing Process there is unsafe
        context.Progress.Report(new(LaunchPhase.ProcessExited, SH.ServiceGameLaunchPhaseProcessExited));
        await next().ConfigureAwait(false);
    }
}