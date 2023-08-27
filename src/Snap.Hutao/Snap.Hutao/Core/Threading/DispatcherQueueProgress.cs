// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

internal class DispatcherQueueProgress<T> : IProgress<T>
{
    private readonly SynchronizationContext synchronizationContext;
    private readonly Action<T>? handler;
    private readonly SendOrPostCallback invokeHandlers;

    public DispatcherQueueProgress(Action<T> handler, SynchronizationContext synchronizationContext)
    {
        this.synchronizationContext = synchronizationContext;
        invokeHandlers = new SendOrPostCallback(InvokeHandlers);

        ArgumentNullException.ThrowIfNull(handler);

        this.handler = handler;
    }

    public event EventHandler<T>? ProgressChanged;

    public void Report(T value)
    {
        Action<T>? handler = this.handler;
        EventHandler<T>? changedEvent = ProgressChanged;
        if (handler is not null || changedEvent is not null)
        {
            synchronizationContext.Post(invokeHandlers, value);
        }
    }

    [SuppressMessage("", "SH007")]
    private void InvokeHandlers(object? state)
    {
        T value = (T)state!;

        Action<T>? handler = this.handler;
        EventHandler<T>? changedEvent = ProgressChanged;

        handler?.Invoke(value);
        changedEvent?.Invoke(this, value);
    }
}