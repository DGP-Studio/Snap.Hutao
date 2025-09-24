// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Launching.Handler;

namespace Snap.Hutao.Service.Game.Launching.Invoker;

internal sealed class DefaultLaunchExecutionInvoker : AbstractLaunchExecutionInvoker
{
    public DefaultLaunchExecutionInvoker()
    {
        Handlers =
        [
            new LaunchExecutionGameLifeCycleHandler(resume: false),
            new LaunchExecutionChannelOptionsHandler(),
            new LaunchExecutionGameResourceHandler(convertOnly: false),
            new LaunchExecutionGameIdentityHandler(),
            new LaunchExecutionWindowsHDRHandler(),
            new LaunchExecutionGameProcessStartHandler(),
            new LaunchExecutionGameIslandHandler(resume: false),
            new LaunchExecutionArbitraryLibraryHandler(),
            new LaunchExecutionOverlayHandler(),
            new LaunchExecutionStarwardPlayTimeStatisticsHandler(),
            new LaunchExecutionBetterGenshinImpactAutomationHandler()
        ];
    }
}