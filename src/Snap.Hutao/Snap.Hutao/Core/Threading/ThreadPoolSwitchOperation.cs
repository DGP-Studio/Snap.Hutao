// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using Snap.Hutao.Core.Threading.Abstraction;

namespace Snap.Hutao.Core.Threading;

internal readonly struct ThreadPoolSwitchOperation : IAwaitable<ThreadPoolSwitchOperation>, ICriticalAwaiter
{
    private static readonly WaitCallback WaitCallbackRunAction = RunAction;
    private readonly DispatcherQueue dispatherQueue;

    public ThreadPoolSwitchOperation(DispatcherQueue dispatherQueue)
    {
        this.dispatherQueue = dispatherQueue;
    }

    public bool IsCompleted
    {
        // Only yields when we are on the DispatcherQueue thread.
        get => !dispatherQueue.HasThreadAccess;
    }

    public ThreadPoolSwitchOperation GetAwaiter()
    {
        return this;
    }

    public void GetResult()
    {
    }

    public void OnCompleted(Action continuation)
    {
        QueueContinuation(continuation, true);
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        QueueContinuation(continuation, false);
    }

    private static void QueueContinuation(Action continuation, bool flowContext)
    {
        if (flowContext)
        {
            ThreadPool.QueueUserWorkItem(WaitCallbackRunAction, continuation);
        }
        else
        {
            ThreadPool.UnsafeQueueUserWorkItem(WaitCallbackRunAction, continuation);
        }
    }

    [SuppressMessage("", "SH007")]
    private static void RunAction(object? state)
    {
        ((Action)state!)();
    }
}