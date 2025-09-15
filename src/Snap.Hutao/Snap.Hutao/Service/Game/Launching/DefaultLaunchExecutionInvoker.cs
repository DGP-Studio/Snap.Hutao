// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Launching.Handler;

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class DefaultLaunchExecutionInvoker : AbstractLaunchExecutionInvoker
{
    public DefaultLaunchExecutionInvoker()
    {
        Handlers =
        [
            new LaunchExecutionEnsureGameNotRunningHandler(),
            new LaunchExecutionSetChannelOptionsHandler(),
            new LaunchExecutionEnsureGameResourceHandler(),
            new LaunchExecutionSetGameAccountHandler(),
            new LaunchExecutionSetWindowsHDRHandler(),
            new LaunchExecutionGameProcessStartHandler(),
            new LaunchExecutionGameIslandHandler(resume: false),
            new LaunchExecutionArbitraryLibraryHandler(),
            new LaunchExecutionOverlayHandler(),
            new LaunchExecutionStarwardPlayTimeStatisticsHandler(),
            new LaunchExecutionBetterGenshinImpactAutomationHandler()
        ];
    }
}