// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Account;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionSetGameAccountHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (context.Account is not null)
        {
            context.Logger.LogInformation("Set game account to [{Account}]", context.Account.Name);

            if (!RegistryInterop.Set(context.Account))
            {
                context.Result.Kind = LaunchExecutionResultKind.GameAccountRegistryWriteResultNotMatch;
                context.Result.ErrorMessage = SH.ViewModelLaunchGameSwitchGameAccountFail;
                return;
            }
        }

        await next().ConfigureAwait(false);
    }
}