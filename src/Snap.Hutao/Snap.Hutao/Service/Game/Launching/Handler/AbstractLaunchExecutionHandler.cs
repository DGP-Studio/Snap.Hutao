// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Launching.Context;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal abstract class AbstractLaunchExecutionHandler : ILaunchExecutionHandler
{
    public virtual ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask ExecuteAsync(LaunchExecutionContext context)
    {
        return ValueTask.CompletedTask;
    }

    public virtual ValueTask AfterAsync(AfterLaunchExecutionContext context)
    {
        return ValueTask.CompletedTask;
    }
}