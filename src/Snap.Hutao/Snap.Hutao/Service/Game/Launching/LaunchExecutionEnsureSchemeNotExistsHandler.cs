// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class LaunchExecutionEnsureSchemeNotExistsHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (context.Scheme is null)
        {
            context.Result.Kind = LaunchExecutionResultKind.NoActiveScheme;
            context.Result.ErrorMessage = SH.ViewModelLaunchGameSchemeNotSelected;
            return;
        }

        await next().ConfigureAwait(false);
    }
}
