// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using Snap.Hutao.Core.Threading.Abstraction;

namespace Snap.Hutao.Core.Threading;

internal readonly struct DispatcherQueueSwitchOperation : IAwaitable<DispatcherQueueSwitchOperation>, ICriticalAwaiter
{
    private readonly DispatcherQueue dispatcherQueue;

    public DispatcherQueueSwitchOperation(DispatcherQueue dispatcherQueue)
    {
        this.dispatcherQueue = dispatcherQueue;
    }

    public bool IsCompleted
    {
        // Only yields when we are not on the DispatcherQueue thread.
        get => dispatcherQueue.HasThreadAccess;
    }

    public DispatcherQueueSwitchOperation GetAwaiter()
    {
        return this;
    }

    public void GetResult()
    {
    }

    public void OnCompleted(Action continuation)
    {
        dispatcherQueue.TryEnqueue(new(continuation));
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        using (ExecutionContext.SuppressFlow())
        {
            dispatcherQueue.TryEnqueue(new(continuation));
        }
    }
}