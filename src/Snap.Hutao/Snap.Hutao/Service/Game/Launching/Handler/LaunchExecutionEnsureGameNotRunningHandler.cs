// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.Launching.Context;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionEnsureGameNotRunningHandler : AbstractLaunchExecutionHandler
{
    public override async ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        if (await GameLifeCycle.IsGameRunningAsync(context.TaskContext).ConfigureAwait(false))
        {
            HutaoException.Throw(SH.ServiceGameLaunchExecutionGameIsRunning);
        }
    }
}