// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameProcessExitHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        try
        {
            if (!context.Process.HasExited)
            {
                context.Progress.Report(new(LaunchPhase.WaitingForExit, SH.ServiceGameLaunchPhaseWaitingProcessExit));
                await context.Process.WaitForExitAsync().ConfigureAwait(false);
            }

            context.Logger.LogInformation("Game process exited with code {ExitCode}", context.Process.ExitCode);
            context.Progress.Report(new(LaunchPhase.ProcessExited, SH.ServiceGameLaunchPhaseProcessExited));
            await next().ConfigureAwait(false);
        }
        finally
        {
            SpinWaitGameRunning();
            context.ServiceProvider.GetRequiredService<IMessenger>().Send<LaunchExecutionProcessStatusChangedMessage>();
        }
    }

    private static unsafe void SpinWaitGameRunning()
    {
        SpinWaitPolyfill.SpinWhile(&LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning);
    }
}