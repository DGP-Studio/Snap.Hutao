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
        IServiceProvider root = Ioc.Default;
        IInfoBarService infoBarService = root.GetRequiredService<IInfoBarService>();
        ILogger<IViewModelSupportLaunchExecution> logger = root.GetRequiredService<ILogger<IViewModelSupportLaunchExecution>>();

        // LaunchScheme? scheme = launchExecution.Shared.GetCurrentLaunchSchemeFromConfigFile();
        try
        {
            // Root service provider is required.
            LaunchExecutionContext context = new(root, launchExecution, targetScheme, launchExecution.SelectedGameAccount);
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