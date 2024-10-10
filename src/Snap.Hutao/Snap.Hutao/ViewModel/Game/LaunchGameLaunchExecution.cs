// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.ViewModel.Game;

internal static class LaunchGameLaunchExecution
{
    public static async ValueTask LaunchExecutionAsync(this IViewModelSupportLaunchExecution launchExecution, LaunchScheme? targetScheme)
    {
        using (IServiceScope scope = Ioc.Default.CreateScope())
        {
            IInfoBarService infoBarService = scope.ServiceProvider.GetRequiredService<IInfoBarService>();
            ILogger<IViewModelSupportLaunchExecution> logger = scope.ServiceProvider.GetRequiredService<ILogger<IViewModelSupportLaunchExecution>>();

            try
            {
                LaunchExecutionContext context = new(scope.ServiceProvider, launchExecution, targetScheme, launchExecution.SelectedGameAccount, launchExecution.SelectedUserAndUid);
                LaunchExecutionResult result = await new LaunchExecutionInvoker().InvokeAsync(context).ConfigureAwait(false);

                if (result.Kind is not LaunchExecutionResultKind.Ok)
                {
                    infoBarService.Warning(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Launch failed");
                infoBarService.Error(ex);
            }
        }
    }
}