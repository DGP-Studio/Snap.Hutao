// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameProcessStartHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        try
        {
            context.Process.Start();
            context.ServiceProvider.GetRequiredService<IMessenger>().Send<LaunchExecutionProcessStatusChangedMessage>();
            context.Logger.LogInformation("Process started");
        }
        catch (Win32Exception ex)
        {
            if (ex.HResult is HRESULT.E_FAIL)
            {
                return;
            }

            throw;
        }

        context.Progress.Report(new(SH.ServiceGameLaunchPhaseProcessStarted));
        await next().ConfigureAwait(false);
    }
}