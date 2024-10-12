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
        if (context.Options.IsUseMiYouSheAccount)
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
        if (context.Scheme.GetSchemeType() is SchemeType.ChineseBilibili)
        {
            context.Logger.LogWarning("Bilibili server does not support auth ticket login");
            return true;
        }

        if (context.Scheme.IsOversea)
        {
            context.Logger.LogWarning("Oversea support broken for now, keep game account in game");
            return true;
        }

        if (context.UserAndUid is not { } userAndUid)
        {
            context.Logger.LogWarning("No user and uid selected, keep game account in game");
            return true;
        }

        if (userAndUid.IsOversea != context.Scheme.IsOversea)
        {
            context.Result.Kind = LaunchExecutionResultKind.GameAccountUserAndUidAndServerNotMatch;
            context.Result.ErrorMessage = "当前选中用户和游戏服务器不匹配，请重新选择";
            return false;
        }

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
            return true;
        }

        context.Result.Kind = LaunchExecutionResultKind.GameAccountCreateAuthTicketFailed;
        context.Result.ErrorMessage = "使用米游社账号登录失败，请重新登录米哈游账号";
        return false;
    }
}