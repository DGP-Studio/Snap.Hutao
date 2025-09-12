// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionEnsureGameNotRunningHandler : AbstractLaunchExecutionHandler
{
    public override ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        if (GameLifeCycle.IsGameRunning())
        {
            return ValueTask.FromException(HutaoException.OperationCanceled(SH.ServiceGameLaunchExecutionGameIsRunning));
        }

        return ValueTask.CompletedTask;
    }
}