// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching;

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