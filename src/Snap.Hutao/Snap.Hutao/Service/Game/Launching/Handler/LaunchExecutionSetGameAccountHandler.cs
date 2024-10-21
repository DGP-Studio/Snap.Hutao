// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Game.Account;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionSetGameAccountHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (context.Options.UsingHoyolabAccount)
        {
            if (!await HandleMiYouSheAccountAsync(context).ConfigureAwait(false))
            {
                return;
            }
        }
        else
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
        }

        await next().ConfigureAwait(false);
    }

    private static async ValueTask<bool> HandleMiYouSheAccountAsync(LaunchExecutionContext context)
    {
        if (context.TargetScheme.GetSchemeType() is SchemeType.ChineseBilibili)
        {
            context.Logger.LogWarning("Bilibili server does not support auth ticket login");

            // TODO: Consider return false here and notify user
            return true;
        }

        if (context.UserAndUid is not { } userAndUid)
        {
            context.Logger.LogWarning("No user and uid selected, keep game account in game");
            return true;
        }

        if (userAndUid.IsOversea ^ context.TargetScheme.IsOversea)
        {
            context.Result.Kind = LaunchExecutionResultKind.GameAccountUserAndUidAndServerNotMatch;
            context.Result.ErrorMessage = SH.ViewModelLaunchGameAccountAndServerNotMatch;
            return false;
        }

        using (IServiceScope scope = context.ServiceProvider.CreateScope())
        {
            IHoyoPlayPassportClient client = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<IHoyoPlayPassportClient>>()
                .CreateFor(userAndUid);
            Response<AuthTicketWrapper> resp = await client
                .CreateAuthTicketAsync(userAndUid.User, context.CancellationToken)
                .ConfigureAwait(false);

            if (ResponseValidator.TryValidate(resp, scope.ServiceProvider, out AuthTicketWrapper? wrapper))
            {
                context.AuthTicket = wrapper.Ticket;
                return true;
            }
        }

        context.Result.Kind = LaunchExecutionResultKind.GameAccountCreateAuthTicketFailed;
        context.Result.ErrorMessage = SH.ViewModelLaunchGameCreateAuthTicketFailed;
        return false;
    }
}