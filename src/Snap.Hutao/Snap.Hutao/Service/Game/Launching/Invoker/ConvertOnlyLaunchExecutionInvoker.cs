// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Service.Game.Launching.Context;
using Snap.Hutao.Service.Game.Launching.Handler;

namespace Snap.Hutao.Service.Game.Launching.Invoker;

internal sealed class ConvertOnlyLaunchExecutionInvoker : AbstractLaunchExecutionInvoker
{
    public ConvertOnlyLaunchExecutionInvoker()
    {
        Handlers =
        [
            new LaunchExecutionGameLifeCycleHandler(resume: false),
            new LaunchExecutionChannelOptionsHandler(),
            new LaunchExecutionGameResourceHandler(convertOnly: true),
        ];
    }

    protected override IProcess? CreateProcess(BeforeLaunchExecutionContext beforeContext)
    {
        // Since this invoker is only for conversion, we do not actually need the process.
        return default;
    }
}