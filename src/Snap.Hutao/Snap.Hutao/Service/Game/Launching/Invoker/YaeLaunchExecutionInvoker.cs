// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.LifeCycle.InterProcess.Yae;
using Snap.Hutao.Service.Game.Launching.Context;
using Snap.Hutao.Service.Game.Launching.Handler;
using Snap.Hutao.Service.Yae.Achievement;

namespace Snap.Hutao.Service.Game.Launching.Invoker;

internal sealed class YaeLaunchExecutionInvoker : AbstractLaunchExecutionInvoker
{
    public YaeLaunchExecutionInvoker(TargetNativeConfiguration config, YaeDataArrayReceiver receiver)
    {
        Handlers =
        [
            new LaunchExecutionGameLifeCycleHandler(resume: false),
            new LaunchExecutionChannelOptionsHandler(),
            new LaunchExecutionGameResourceHandler(false),
            new LaunchExecutionGameIdentityHandler(),
            new LaunchExecutionGameProcessStartHandler(),
            new LaunchExecutionYaeNamedPipeHandler(config, receiver),
        ];
    }

    protected override IProcess? CreateProcess(BeforeLaunchExecutionContext beforeContext)
    {
        return GameProcessFactory.CreateForEmbeddedYae(beforeContext);
    }
}