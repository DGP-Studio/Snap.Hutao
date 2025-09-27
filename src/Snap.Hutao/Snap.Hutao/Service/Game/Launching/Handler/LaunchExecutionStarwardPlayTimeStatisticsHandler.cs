// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Service.Game.Launching.Context;
using Windows.System;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionStarwardPlayTimeStatisticsHandler : AbstractLaunchExecutionHandler
{
    public override async ValueTask ExecuteAsync(LaunchExecutionContext context)
    {
        if (context.Process.IsRunning() && context.LaunchOptions.UsingStarwardPlayTimeStatistics.Value)
        {
            await LaunchStarwardForPlayTimeStatisticsAsync(context).ConfigureAwait(false);
        }
    }

    private static async ValueTask LaunchStarwardForPlayTimeStatisticsAsync(LaunchExecutionContext context)
    {
        string gameBiz = context.IsOversea ? "hk4e_global" : "hk4e_cn";
        Uri starwardPlayTimeUri = $"starward://playtime/{gameBiz}".ToUri();
        if (await Launcher.QueryUriSupportAsync(starwardPlayTimeUri, LaunchQuerySupportType.Uri) is LaunchQuerySupportStatus.Available)
        {
            await Launcher.LaunchUriAsync(starwardPlayTimeUri);
        }
    }
}