// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.UI.Xaml.View.Window;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionOverlayHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (HutaoRuntime.IsProcessElevated && context.Options.UsingOverlay.Value)
        {
            await context.TaskContext.SwitchToMainThreadAsync();
            LaunchExecutionOverlayWindow window = context.ServiceProvider.GetRequiredService<LaunchExecutionOverlayWindow>();

            await next().ConfigureAwait(false);

            await context.TaskContext.SwitchToMainThreadAsync();
            window.PreventClose = false;
            window.Close();
        }
        else
        {
            await next().ConfigureAwait(false);
        }
    }
}