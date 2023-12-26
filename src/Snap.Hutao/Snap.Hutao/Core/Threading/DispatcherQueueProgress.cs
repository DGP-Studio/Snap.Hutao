// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;

namespace Snap.Hutao.Core.Threading;

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
        Action<T> handler = this.handler;
        dispatcherQueue.TryEnqueue(() => handler(value));
    }
}