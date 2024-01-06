// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Game.Launching.Handler;

namespace Snap.Hutao.Service.Game.Launching;

[Injection(InjectAs.Transient)]
internal sealed class LaunchExecutionInvoker
{
    private readonly Queue<ILaunchExecutionDelegateHandler> handlers;

    public LaunchExecutionInvoker()
    {
        handlers = [];
        handlers.Enqueue(new LaunchExecutionEnsureGameNotRunningHandler());
        handlers.Enqueue(new LaunchExecutionEnsureSchemeHandler());
        handlers.Enqueue(new LaunchExecutionSetChannelOptionsHandler());
        handlers.Enqueue(new LaunchExecutionEnsureGameResourceHandler());
        handlers.Enqueue(new LaunchExecutionSetGameAccountHandler());
        handlers.Enqueue(new LaunchExecutionSetWindowsHDRHandler());
        handlers.Enqueue(new LaunchExecutionStatusProgressHandler());
        handlers.Enqueue(new LaunchExecutionGameProcessInitializationHandler());
        handlers.Enqueue(new LaunchExecutionSetDiscordActivityHandler());
        handlers.Enqueue(new LaunchExecutionGameProcessStartHandler());
        handlers.Enqueue(new LaunchExecutionStarwardPlayTimeStatisticsHandler());
        handlers.Enqueue(new LaunchExecutionUnlockFpsHandler());
        handlers.Enqueue(new LaunchExecutionGameProcessExitHandler());
    }

    public async ValueTask<LaunchExecutionResult> InvokeAsync(LaunchExecutionContext context)
    {
        await InvokeHandlerAsync(context).ConfigureAwait(false);
        return context.Result;
    }

    private async ValueTask<LaunchExecutionContext> InvokeHandlerAsync(LaunchExecutionContext context)
    {
        if (handlers.TryDequeue(out ILaunchExecutionDelegateHandler? handler))
        {
            string typeName = TypeNameHelper.GetTypeDisplayName(handler, false);
            context.Logger.LogInformation("Handler[{Handler}] begin execution", typeName);
            await handler.OnExecutionAsync(context, () => InvokeHandlerAsync(context)).ConfigureAwait(false);
            context.Logger.LogInformation("Handler[{Handler}] end execution", typeName);
        }

        return context;
    }
}