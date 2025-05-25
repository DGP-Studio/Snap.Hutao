// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.UI.Xaml.View.Window;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionOverlayHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (HutaoRuntime.IsProcessElevated && context.Options.UsingOverlay)
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