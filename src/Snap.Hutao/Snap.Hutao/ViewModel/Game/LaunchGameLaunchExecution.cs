// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Game.Launching.Context;
using Snap.Hutao.Service.Game.Launching.Invoker;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.ViewModel.Game;

internal static class LaunchGameLaunchExecution
{
    public static async ValueTask LaunchExecutionAsync(this IViewModelSupportLaunchExecution2 viewModel, UserAndUid? userAndUid)
    {
        // The game process can exist longer than the view model
        // Force use root scope
        using (IServiceScope scope = Ioc.Default.CreateScope())
        {
            DefaultLaunchExecutionInvoker invoker = new();
            try
            {
                LaunchExecutionInvocationContext context = new()
                {
                    ViewModel = viewModel,
                    ServiceProvider = scope.ServiceProvider,
                    LaunchOptions = scope.ServiceProvider.GetRequiredService<LaunchOptions>(),
                    Identity = GameIdentity.Create(userAndUid, viewModel.GameAccount),
                };

                await invoker.InvokeAsync(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                scope.ServiceProvider.GetRequiredService<IMessenger>().Send(InfoBarMessage.Error(ex));
            }
        }
    }
}