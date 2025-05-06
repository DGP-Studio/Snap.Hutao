// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.ViewModel.Game;

internal static class LaunchGameLaunchExecution
{
    public static async ValueTask LaunchExecutionAsync(this IViewModelSupportLaunchExecution viewModel, UserAndUid? userAndUid)
    {
        // Force use root scope
        using (IServiceScope scope = Ioc.Default.CreateScope())
        {
            IInfoBarService infoBarService = scope.ServiceProvider.GetRequiredService<IInfoBarService>();
            DefaultLaunchExecutionInvoker invoker = new();
            try
            {
                using (LaunchExecutionContext context = new(scope.ServiceProvider, viewModel, userAndUid))
                {
                    LaunchExecutionResult result = await invoker.InvokeAsync(context).ConfigureAwait(false);

                    if (result.Kind is not LaunchExecutionResultKind.Ok)
                    {
                        infoBarService.Warning(result.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                infoBarService.Error(ex);
            }
        }
    }
}