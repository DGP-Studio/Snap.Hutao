// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameProcessStartHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        try
        {
            context.Process.Start();
            context.ServiceProvider.GetRequiredService<IMessenger>().Send<LaunchExecutionProcessStatusChangedMessage>();
            context.Logger.LogInformation("Process started");
        }
        catch (Win32Exception ex)
        {
            // E_FAIL
            if (ex.HResult is unchecked((int)0x80004005))
            {
                return;
            }

            throw;
        }

        context.Progress.Report(new(LaunchPhase.ProcessStarted, SH.ServiceGameLaunchPhaseProcessStarted));
        await next().ConfigureAwait(false);
    }
}