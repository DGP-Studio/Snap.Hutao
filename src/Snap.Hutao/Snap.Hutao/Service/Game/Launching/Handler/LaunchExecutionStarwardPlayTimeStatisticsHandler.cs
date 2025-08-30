// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Windows.System;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionStarwardPlayTimeStatisticsHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (context.Process.IsRunning() && context.Options.UsingStarwardPlayTimeStatistics.Value)
        {
            context.Logger.LogInformation("Using Starward to count game time");
            await LaunchStarwardForPlayTimeStatisticsAsync(context).ConfigureAwait(false);
        }

        await next().ConfigureAwait(false);
    }

    private static async ValueTask LaunchStarwardForPlayTimeStatisticsAsync(LaunchExecutionContext context)
    {
        string gameBiz = context.TargetScheme.IsOversea ? "hk4e_global" : "hk4e_cn";
        Uri starwardPlayTimeUri = $"starward://playtime/{gameBiz}".ToUri();
        if (await Launcher.QueryUriSupportAsync(starwardPlayTimeUri, LaunchQuerySupportType.Uri) is LaunchQuerySupportStatus.Available)
        {
            context.Logger.LogInformation("Launching Starward");
            await Launcher.LaunchUriAsync(starwardPlayTimeUri);
        }
    }
}