// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.Progress;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionStatusProgressHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        IProgressFactory progressFactory = context.ServiceProvider.GetRequiredService<IProgressFactory>();
        LaunchStatusOptions statusOptions = context.ServiceProvider.GetRequiredService<LaunchStatusOptions>();
        context.Progress = progressFactory.CreateForMainThread<LaunchStatus>(status => statusOptions.LaunchStatus = status);

        await next().ConfigureAwait(false);

        // Clear status
        context.Progress.Report(default!);
    }
}