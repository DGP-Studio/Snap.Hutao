// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;

namespace Snap.Hutao.Service.Game.Launching;

[Injection(InjectAs.Transient)]
internal sealed class LaunchExecutionInvoker
{
    private readonly Queue<ILaunchExecutionDelegateHandler> handlers;

    public LaunchExecutionInvoker()
    {
        handlers = [];
        handlers.Enqueue(new LaunchExecutionEnsureGameNotRunningHandler());
        handlers.Enqueue(new LaunchExecutionEnsureSchemeNotExistsHandler());
        handlers.Enqueue(new LaunchExecutionSetChannelOptionsHandler());
        handlers.Enqueue(new LaunchExecutionEnsureGameResourceHandler());
        handlers.Enqueue(new LaunchExecutionSetGameAccountHandler());
        handlers.Enqueue(new LaunchExecutionSetWindowsHDRHandler());
        handlers.Enqueue(new LaunchExecutionStatusProgressHandler());
        handlers.Enqueue(new LaunchExecutionGameProcessInitializationHandler());
        handlers.Enqueue(new LaunchExecutionSetDiscordActivityHandler());
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
            context.Logger.LogInformation("Handler[{Handler}] begin execution", TypeNameHelper.GetTypeDisplayName(handler));
            await handler.OnExecutionAsync(context, () => InvokeHandlerAsync(context)).ConfigureAwait(false);
        }

        return context;
    }
}