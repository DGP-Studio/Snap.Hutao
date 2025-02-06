// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Launching.Handler;

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class DefaultLaunchExecutionInvoker : LaunchExecutionInvoker
{
    public DefaultLaunchExecutionInvoker()
    {
        Handlers.Enqueue(new LaunchExecutionEnsureGameNotRunningHandler());
        Handlers.Enqueue(new LaunchExecutionEnsureSchemeHandler());
        Handlers.Enqueue(new LaunchExecutionSetChannelOptionsHandler());
        Handlers.Enqueue(new LaunchExecutionEnsureGameResourceHandler());
        Handlers.Enqueue(new LaunchExecutionSetGameAccountHandler());
        Handlers.Enqueue(new LaunchExecutionSetWindowsHDRHandler());
        Handlers.Enqueue(new LaunchExecutionStatusProgressHandler());
        Handlers.Enqueue(new LaunchExecutionGameProcessInitializationHandler());
        Handlers.Enqueue(new LaunchExecutionSetDiscordActivityHandler());
        Handlers.Enqueue(new LaunchExecutionGameProcessStartHandler());
        Handlers.Enqueue(new LaunchExecutionUnlockFpsHandler());
        Handlers.Enqueue(new LaunchExecutionStarwardPlayTimeStatisticsHandler());
        Handlers.Enqueue(new LaunchExecutionBetterGenshinImpactAutomationHandlder());
        Handlers.Enqueue(new LaunchExecutionGameProcessExitHandler());
    }
}