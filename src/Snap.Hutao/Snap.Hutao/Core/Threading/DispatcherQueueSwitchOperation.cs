// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using Snap.Hutao.Core.Threading.Abstraction;

namespace Snap.Hutao.Core.Threading;

[HighQuality]
internal readonly struct DispatcherQueueSwitchOperation : IAwaitable<DispatcherQueueSwitchOperation>, ICriticalAwaiter
{
    private readonly DispatcherQueue dispatherQueue;

    public DispatcherQueueSwitchOperation(DispatcherQueue dispatherQueue)
    {
        this.dispatherQueue = dispatherQueue;
    }

    public bool IsCompleted
    {
        // Only yields when we are not on the DispatcherQueue thread.
        get => dispatherQueue.HasThreadAccess;
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
        dispatherQueue.TryEnqueue(new DispatcherQueueHandler(continuation));
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        using (ExecutionContext.SuppressFlow())
        {
            dispatherQueue.TryEnqueue(new DispatcherQueueHandler(continuation));
        }
    }
}