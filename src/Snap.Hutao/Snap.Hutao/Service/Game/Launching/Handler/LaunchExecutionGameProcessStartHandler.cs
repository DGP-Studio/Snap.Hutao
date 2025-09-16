// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Launching.Context;
using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameProcessStartHandler : AbstractLaunchExecutionHandler
{
    public override ValueTask ExecuteAsync(LaunchExecutionContext context)
    {
        try
        {
            context.Process.Start();
            GameLifeCycle.IsGameRunningProperty.Value = true;
        }
        catch (Win32Exception ex)
        {
            if (ex.HResult is HRESULT.E_FAIL)
            {
                return ValueTask.CompletedTask;
            }

            throw;
        }

        context.Progress.Report(new(SH.ServiceGameLaunchPhaseProcessStarted));
        return ValueTask.CompletedTask;
    }
}