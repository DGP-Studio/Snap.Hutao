// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.View.Window;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionOverlayHandlder : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        await context.TaskContext.SwitchToMainThreadAsync();
        LaunchExecutionOverlayWindow window = context.ServiceProvider.GetRequiredService<LaunchExecutionOverlayWindow>();

        await next().ConfigureAwait(false);

        await context.TaskContext.SwitchToMainThreadAsync();
        window.Close();
    }
}