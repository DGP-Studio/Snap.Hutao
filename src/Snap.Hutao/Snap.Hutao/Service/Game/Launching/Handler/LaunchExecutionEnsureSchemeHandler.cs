// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionEnsureSchemeHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (context.Scheme is null)
        {
            context.Result.Kind = LaunchExecutionResultKind.NoActiveScheme;
            context.Result.ErrorMessage = SH.ViewModelLaunchGameSchemeNotSelected;
            return;
        }

        context.Logger.LogInformation("Scheme [{Scheme}] is selected", context.Scheme.DisplayName);
        await next().ConfigureAwait(false);
    }
}
