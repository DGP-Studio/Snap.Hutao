// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Service.Game.Account;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionSetGameAccountHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (context.Options.IsUseMiYouSheAccount)
        {
            if (context.UserAndUid is { } userAndUid)
            {
                Response<AuthTicketWrapper> resp;
                using (IServiceScope scope = context.ServiceProvider.CreateScope())
                {
                    IHoyoPlayPassportClient client = scope.ServiceProvider
                        .GetRequiredService<IOverseaSupportFactory<IHoyoPlayPassportClient>>()
                        .CreateFor(userAndUid);
                    resp = await client
                        .CreateAuthTicketAsync(userAndUid.User, context.CancellationToken)
                        .ConfigureAwait(false);
                }

                if (resp.IsOk())
                {
                    context.AuthTicket = resp.Data.Ticket;
                }
            }
            else
            {
                IInfoBarService infoBarService = context.ServiceProvider.GetRequiredService<IInfoBarService>();
                infoBarService.Warning("未选中米游社用户，将保留游戏内登录态");
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
}