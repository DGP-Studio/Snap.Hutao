// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;

namespace Snap.Hutao.Core.Threading;

internal sealed class DispatcherQueueSynchronizationContextSendSupport : SynchronizationContext
{
    private readonly DispatcherQueue dispatcherQueue;

    public DispatcherQueueSynchronizationContextSendSupport(DispatcherQueue dispatcherQueue)
    {
        this.dispatcherQueue = dispatcherQueue;
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        ArgumentNullException.ThrowIfNull(d);
        dispatcherQueue.TryEnqueue(() => d(state));
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        ArgumentNullException.ThrowIfNull(d);
        dispatcherQueue.Invoke(() => d(state));
    }

    public override SynchronizationContext CreateCopy()
    {
        return new DispatcherQueueSynchronizationContextSendSupport(dispatcherQueue);
    }
}