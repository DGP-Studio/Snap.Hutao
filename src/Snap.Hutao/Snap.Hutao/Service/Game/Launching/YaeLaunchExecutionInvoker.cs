// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.LifeCycle.InterProcess.Yae;
using Snap.Hutao.Service.Game.Launching.Handler;
using Snap.Hutao.Service.Yae.Achievement;

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class YaeLaunchExecutionInvoker : LaunchExecutionInvoker
{
    public YaeLaunchExecutionInvoker(NativeConfiguration config, YaeDataArrayReceiver receiver)
    {
        Handlers.Enqueue(new LaunchExecutionEnsureGameNotRunningHandler());
        Handlers.Enqueue(new LaunchExecutionEnsureSchemeHandler());
        Handlers.Enqueue(new LaunchExecutionSetChannelOptionsHandler());
        Handlers.Enqueue(new LaunchExecutionEnsureGameResourceHandler());
        Handlers.Enqueue(new LaunchExecutionSetGameAccountHandler());
        Handlers.Enqueue(new LaunchExecutionStatusProgressHandler());
        Handlers.Enqueue(new YaeLaunchExecutionGameProcessInitializationHandler());
        Handlers.Enqueue(new LaunchExecutionGameProcessStartHandler());
        Handlers.Enqueue(new YaeLaunchExecutionNamedPipeHandler(config, receiver));
        Handlers.Enqueue(new LaunchExecutionGameProcessExitHandler());
    }
}