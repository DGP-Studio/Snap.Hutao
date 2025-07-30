// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Account;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionSetWindowsHDRHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (context.Options.IsWindowsHDREnabled)
        {
            context.Logger.LogInformation("Set Windows HDR");
            RegistryInterop.SetWindowsHDR(context.TargetScheme.IsOversea);
        }

        await next().ConfigureAwait(false);
    }
}