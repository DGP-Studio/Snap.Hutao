// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.Launching.Handler;

namespace Snap.Hutao.Service.Game.Launching;

internal abstract class LaunchExecutionInvoker
{
    private bool invoked;

    protected Queue<ILaunchExecutionDelegateHandler> Handlers { get; } = [];

    public async ValueTask<LaunchExecutionResult> InvokeAsync(LaunchExecutionContext context)
    {
        HutaoException.ThrowIf(Interlocked.Exchange(ref invoked, true), "The invoker has been invoked");

        try
        {
            context.ServiceProvider.GetRequiredService<IMessenger>().Send(new LaunchExecutionGameFileSystemExclusiveAccessChangedMessage(false));
            await RecursiveInvokeHandlerAsync(context, 0).ConfigureAwait(false);
            return context.Result;
        }
        finally
        {
            unsafe
            {
                SpinWaitPolyfill.SpinWhile(&LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning);
            }

            context.ServiceProvider.GetRequiredService<IMessenger>().Send<LaunchExecutionProcessStatusChangedMessage>();
            context.ServiceProvider.GetRequiredService<IMessenger>().Send(new LaunchExecutionGameFileSystemExclusiveAccessChangedMessage(true));
        }
    }

    private async ValueTask RecursiveInvokeHandlerAsync(LaunchExecutionContext context, int index)
    {
        if (Handlers.TryDequeue(out ILaunchExecutionDelegateHandler? handler))
        {
            string typeName = TypeNameHelper.GetTypeDisplayName(handler, false);
            context.Logger.LogInformation("Handler {Index} [{Handler}] begin execution", index, typeName);
            await handler.OnExecutionAsync(context, () => RecursiveInvokeHandlerAsync(context, index + 1)).ConfigureAwait(false);
            context.Logger.LogInformation("Handler {Index} [{Handler}] end execution", index, typeName);
        }
    }
}