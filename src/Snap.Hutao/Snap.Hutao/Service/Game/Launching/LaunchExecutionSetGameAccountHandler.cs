// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Account;

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class LaunchExecutionSetGameAccountHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (context.Account is not null && !RegistryInterop.Set(context.Account))
        {
            context.Result.Kind = LaunchExecutionResultKind.GameAccountRegistryWriteResultNotMatch;
            context.Result.ErrorMessage = SH.ViewModelLaunchGameSwitchGameAccountFail;
            return;
        }

        await next().ConfigureAwait(false);
    }
}