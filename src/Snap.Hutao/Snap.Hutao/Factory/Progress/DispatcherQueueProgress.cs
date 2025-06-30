// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;

namespace Snap.Hutao.Factory.Progress;

internal class DispatcherQueueProgress<T> : IProgress<T>
{
    private readonly DispatcherQueue dispatcherQueue;
    private readonly Action<T> handler;

    public DispatcherQueueProgress(Action<T> handler, DispatcherQueue dispatcherQueue)
    {
        this.dispatcherQueue = dispatcherQueue;
        this.handler = handler;
    }

    public void Report(T value)
    {
        // Avoid capture <this>
        Action<T> handler = this.handler;

        if (dispatcherQueue.HasThreadAccess)
        {
            handler(value);
        }
        else
        {
            // We should always wait for the report to finish
            // If we use TryEnqueue, DispatcherQueue can queue the item far from now
            dispatcherQueue.Invoke(DispatcherQueuePriority.High, () => handler(value));
        }
    }
}

[SuppressMessage("", "SA1402")]
internal class DispatcherQueueProgress<T, TState> : IProgress<T>
{
    private readonly DispatcherQueue dispatcherQueue;
    private readonly Action<T, TState> handler;
    private readonly TState state;

    public DispatcherQueueProgress(Action<T, TState> handler, TState state, DispatcherQueue dispatcherQueue)
    {
        this.dispatcherQueue = dispatcherQueue;
        this.handler = handler;
        this.state = state;
    }

    public void Report(T value)
    {
        // Avoid capture <this>
        Action<T, TState> handler = this.handler;
        TState state = this.state;

        if (dispatcherQueue.HasThreadAccess)
        {
            handler(value, state);
        }
        else
        {
            // We should always wait for the report to finish
            // If we use TryEnqueue, DispatcherQueue can queue the item far from now
            dispatcherQueue.Invoke(DispatcherQueuePriority.High, () => handler(value, state));
        }
    }
}