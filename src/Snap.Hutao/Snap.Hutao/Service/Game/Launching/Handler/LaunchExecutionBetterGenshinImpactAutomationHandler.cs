// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Windows.System;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionBetterGenshinImpactAutomationHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (context.Process.IsRunning() && context.Options.UsingBetterGenshinImpactAutomation.Value)
        {
            await LaunchBetterGenshinImpactAsync(context).ConfigureAwait(false);
        }

        await next().ConfigureAwait(false);
    }

    private static async ValueTask LaunchBetterGenshinImpactAsync(LaunchExecutionContext context)
    {
        Uri betterGenshinImpactUri = "bettergi://start".ToUri();
        if (await Launcher.QueryUriSupportAsync(betterGenshinImpactUri, LaunchQuerySupportType.Uri) is LaunchQuerySupportStatus.Available)
        {
            try
            {
                context.Logger.LogInformation("Waiting game window to be ready");
                SpinWait.SpinUntil(() => context.Process.MainWindowHandle.Value is not 0);
            }
            catch (InvalidOperationException)
            {
                context.Logger.LogInformation("Failed to get game window handle");
                return;
            }

            context.Logger.LogInformation("Launching BetterGI");
            await Launcher.LaunchUriAsync(betterGenshinImpactUri);
        }
    }
}