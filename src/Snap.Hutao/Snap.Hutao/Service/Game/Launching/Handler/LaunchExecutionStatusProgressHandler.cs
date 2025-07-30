// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.Progress;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionStatusProgressHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        IProgressFactory progressFactory = context.ServiceProvider.GetRequiredService<IProgressFactory>();
        LaunchStatusOptions statusOptions = context.ServiceProvider.GetRequiredService<LaunchStatusOptions>();
        context.Progress = progressFactory.CreateForMainThread<LaunchStatus?, LaunchStatusOptions>(static (status, statusOptions) => statusOptions.LaunchStatus = status, statusOptions);

        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        await next().ConfigureAwait(false);

        // Clear status
        context.Progress.Report(default!);
    }
}