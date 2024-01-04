// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Launching;

[Injection(InjectAs.Transient)]
internal sealed class LaunchExecutionInvoker
{
    private readonly Queue<ILaunchExecutionDelegateHandler> handlers;

    public LaunchExecutionInvoker()
    {
        handlers = [];
        handlers.Enqueue(new LaunchExecutionEnsureSchemeNotExistsHandler());
        handlers.Enqueue(new LaunchExecutionSetChannelOptionsHandler());
    }

    public async ValueTask<LaunchExecutionResult> LaunchAsync(LaunchExecutionContext context)
    {
        await ExecuteAsync(context).ConfigureAwait(false);
        return context.Result;
    }

    private async ValueTask<LaunchExecutionContext> ExecuteAsync(LaunchExecutionContext context)
    {
        if (handlers.TryDequeue(out ILaunchExecutionDelegateHandler? handler))
        {
            string handlerName = handler.GetType().Name;

            context.Logger.LogInformation("Handler[{Handler}] begin execute", handlerName);
            await handler.OnExecutionAsync(context, () => ExecuteAsync(context)).ConfigureAwait(false);
            context.Logger.LogInformation("Handler[{Handler}] end execute", handlerName);
        }

        return context;
    }
}