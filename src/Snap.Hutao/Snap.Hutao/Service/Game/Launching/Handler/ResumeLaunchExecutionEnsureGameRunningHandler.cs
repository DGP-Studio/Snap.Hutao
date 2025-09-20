// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Launching.Context;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class ResumeLaunchExecutionEnsureGameRunningHandler : AbstractLaunchExecutionHandler
{
    public override async ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        if (await GameLifeCycle.TryGetRunningGameProcessAsync(context.TaskContext).ConfigureAwait(false) is (true, { } gameProcess))
        {
            context.SetOption(LaunchExecutionOptionsKey.RunningProcess, gameProcess);
        }
    }
}