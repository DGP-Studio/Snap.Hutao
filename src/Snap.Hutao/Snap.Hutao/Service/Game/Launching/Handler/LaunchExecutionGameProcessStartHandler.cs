// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameProcessStartHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        try
        {
            context.Process.Start();
            context.Logger.LogInformation("Process started");
        }
        catch (Win32Exception ex) when (ex.HResult == HRESULT.E_FAIL)
        {
            return;
        }

        context.Progress.Report(new(LaunchPhase.ProcessStarted, SH.ServiceGameLaunchPhaseProcessStarted));
        await next().ConfigureAwait(false);
    }
}