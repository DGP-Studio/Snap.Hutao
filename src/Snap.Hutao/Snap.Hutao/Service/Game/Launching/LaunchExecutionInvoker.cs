// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.Launching.Handler;
using System.Collections.Concurrent;

namespace Snap.Hutao.Service.Game.Launching;

internal abstract class LaunchExecutionInvoker
{
    private static readonly ConcurrentDictionary<LaunchExecutionInvoker, Void> Invokers = [];

    private bool invoked;

    protected Queue<ILaunchExecutionDelegateHandler> Handlers { get; } = [];

    protected Queue<ILaunchExecutionDelegateHandler> ExecutionHandlers { get; } = [];

    public static bool IsAnyLaunchExecutionInvoking()
    {
        return !Invokers.IsEmpty;
    }

    public async ValueTask<LaunchExecutionResult> InvokeAsync(LaunchExecutionContext context)
    {
        HutaoException.ThrowIf(Interlocked.Exchange(ref invoked, true), "The invoker has been invoked");

        try
        {
            Invokers.TryAdd(this, default);
            context.ServiceProvider.GetRequiredService<IMessenger>().Send(new LaunchExecutionGameFileSystemExclusiveAccessChangedMessage(false));
            if (await RecursiveInvokeBeforeExecutionAsync(context, 0).ConfigureAwait(false))
            {
                await RecursiveInvokeExecutionAsync(context, 0).ConfigureAwait(false);
            }

            return context.Result;
        }
        finally
        {
            await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

            Invokers.TryRemove(this, out _);
            if (Invokers.IsEmpty)
            {
                unsafe
                {
                    SpinWaitPolyfill.SpinWhile(&LaunchExecutionEnsureGameNotRunningHandler.IsGameRunning);
                }

                context.ServiceProvider.GetRequiredService<IMessenger>().Send<LaunchExecutionProcessStatusChangedMessage>();
                context.ServiceProvider.GetRequiredService<IMessenger>().Send(new LaunchExecutionGameFileSystemExclusiveAccessChangedMessage(true));
            }
        }
    }

    private async ValueTask<bool> RecursiveInvokeBeforeExecutionAsync(LaunchExecutionContext context, int index)
    {
        bool result = true;
        if (Handlers.TryDequeue(out ILaunchExecutionDelegateHandler? handler))
        {
            ExecutionHandlers.Enqueue(handler);
            string typeName = TypeNameHelper.GetTypeDisplayName(handler, false);
            context.Logger.LogInformation("Handler {Index} [{Handler}] begin before-execution", index, typeName);
            result = await handler.BeforeExecutionAsync(context, () => RecursiveInvokeBeforeExecutionAsync(context, index + 1)).ConfigureAwait(false);
            context.Logger.LogInformation("Handler {Index} [{Handler}] end before-execution", index, typeName);
        }

        return result;
    }

    private async ValueTask RecursiveInvokeExecutionAsync(LaunchExecutionContext context, int index)
    {
        if (ExecutionHandlers.TryDequeue(out ILaunchExecutionDelegateHandler? handler))
        {
            string typeName = TypeNameHelper.GetTypeDisplayName(handler, false);
            context.Logger.LogInformation("Handler {Index} [{Handler}] begin execution", index, typeName);
            await handler.ExecutionAsync(context, () => RecursiveInvokeExecutionAsync(context, index + 1)).ConfigureAwait(false);
            context.Logger.LogInformation("Handler {Index} [{Handler}] end execution", index, typeName);
        }
    }
}