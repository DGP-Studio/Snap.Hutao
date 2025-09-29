// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Service.Game.Launching.Context;
using Snap.Hutao.Service.Game.Launching.Handler;

namespace Snap.Hutao.Service.Game.Launching.Invoker;

internal sealed class ResumeLaunchExecutionInvoker : AbstractLaunchExecutionInvoker
{
    public ResumeLaunchExecutionInvoker()
    {
        Handlers =
        [
            new LaunchExecutionGameLifeCycleHandler(resume: true),
            new LaunchExecutionGameIslandHandler(resume: true),
            new LaunchExecutionOverlayHandler()
        ];
    }

    protected override IProcess? CreateProcess(BeforeLaunchExecutionContext context)
    {
        context.TryGetOption(LaunchExecutionOptionsKey.RunningProcess, out IProcess? process);
        return process;
    }
}