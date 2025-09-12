// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.ViewModel.Game;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Launching;

internal abstract class LaunchExecutionInvoker2
{
    protected ImmutableArray<ILaunchExecutionHandler> Handlers { get; init; }

    public async ValueTask InvokeAsync(LaunchExecutionInvocationContext context)
    {
        const string LockTrace = $"{nameof(LaunchExecutionInvoker2)}.{nameof(InvokeAsync)}";
        context.Options.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem);
        ArgumentNullException.ThrowIfNull(gameFileSystem);

        using (gameFileSystem)
        {
            BeforeLaunchExecutionContext beforeContext = new()
            {
                ViewModel = context.ViewModel,
            };

            foreach (ILaunchExecutionHandler handler in Handlers)
            {
                await handler.BeforeAsync(beforeContext).ConfigureAwait(false);
            }

            LaunchExecutionContext executionContext = new();
            foreach (ILaunchExecutionHandler handler in Handlers)
            {
                await handler.ExecuteAsync(executionContext).ConfigureAwait(false);
            }

            AfterLaunchExecutionContext afterContext = new();
            foreach (ILaunchExecutionHandler handler in Handlers)
            {
                await handler.AfterAsync(afterContext).ConfigureAwait(false);
            }
        }
    }
}

internal sealed class LaunchExecutionInvocationContext
{
    public required IViewModelSupportLaunchExecution2 ViewModel { get; init; }

    public required LaunchOptions Options { get; init; }
}