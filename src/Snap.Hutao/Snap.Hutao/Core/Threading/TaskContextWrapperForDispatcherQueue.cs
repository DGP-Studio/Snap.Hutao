// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;

namespace Snap.Hutao.Core.Threading;

internal sealed class TaskContextWrapperForDispatcherQueue : ITaskContext
{
    private readonly DispatcherQueue dispatcherQueue;

    public TaskContextWrapperForDispatcherQueue(DispatcherQueue dispatcherQueue)
    {
        this.dispatcherQueue = dispatcherQueue;
    }

    public void BeginInvokeOnMainThread(Action action)
    {
        dispatcherQueue.TryEnqueue(() => action());
    }

    public void InvokeOnMainThread(Action action)
    {
        dispatcherQueue.Invoke(action);
    }

    public T InvokeOnMainThread<T>(Func<T> action)
    {
        return dispatcherQueue.Invoke(action);
    }

    public ThreadPoolSwitchOperation SwitchToBackgroundAsync()
    {
        return new(dispatcherQueue);
    }

    public DispatcherQueueSwitchOperation SwitchToMainThreadAsync()
    {
        return new(dispatcherQueue);
    }
}