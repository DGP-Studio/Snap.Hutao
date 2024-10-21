// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Account;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionSetWindowsHDRHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (context.Options.IsWindowsHDREnabled)
        {
            context.Logger.LogInformation("Set Windows HDR");
            RegistryInterop.SetWindowsHDR(context.TargetScheme.IsOversea);
        }

        await next().ConfigureAwait(false);
    }
}