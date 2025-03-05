// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Launching.Handler;

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class ResumeLaunchExecutionInvoker : LaunchExecutionInvoker
{
    public ResumeLaunchExecutionInvoker()
    {
        Handlers.Enqueue(new ResumeLaunchExecutionEnsureGameRunningHandler());
        Handlers.Enqueue(new LaunchExecutionStatusProgressHandler());
        Handlers.Enqueue(new LaunchExecutionGameIslandHandler(resume: true));
        Handlers.Enqueue(new LaunchExecutionGameProcessExitHandler());
    }
}