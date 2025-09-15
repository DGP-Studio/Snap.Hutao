// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Game.Account;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.Passport;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionSetGameAccountHandler : AbstractLaunchExecutionHandler
{
    public override async ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        if (context.LaunchOptions.UsingHoyolabAccount.Value)
        {
            await HandleHoyolabAccountAsync(context).ConfigureAwait(false);
        }
        else if (context.Identity.GameAccount is { } account && !RegistryInterop.Set(account))
        {
            HutaoException.Throw(SH.ViewModelLaunchGameSwitchGameAccountFail);
        }
    }

    private static async ValueTask HandleHoyolabAccountAsync(BeforeLaunchExecutionContext context)
    {
        if (context.TargetScheme.GetSchemeType() is SchemeType.ChineseBilibili)
        {
            return;
        }

        if (context.Identity.UserAndUid is not { } userAndUid)
        {
            return;
        }

        if (userAndUid.IsOversea ^ context.TargetScheme.IsOversea)
        {
            HutaoException.NotSupported(SH.ViewModelLaunchGameAccountAndServerNotMatch);
        }

        using (IServiceScope scope = context.ServiceProvider.CreateScope())
        {
            IHoyoPlayPassportClient client = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<IHoyoPlayPassportClient>>()
                .CreateFor(userAndUid);
            Response<AuthTicketWrapper> resp = await client
                .CreateAuthTicketAsync(userAndUid.User)
                .ConfigureAwait(false);

            if (ResponseValidator.TryValidate(resp, scope.ServiceProvider, out AuthTicketWrapper? wrapper))
            {
                context.SetOption(LaunchExecutionOptionsKey.LoginAuthTicket, wrapper.Ticket);
                return;
            }
        }

        HutaoException.NotSupported(SH.ViewModelLaunchGameCreateAuthTicketFailed);
    }
}