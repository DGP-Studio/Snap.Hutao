// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;

namespace Snap.Hutao.Core.Threading;

internal sealed class TaskContextWrapperForDispatcherQueue : ITaskContext
{
    public TaskContextWrapperForDispatcherQueue(DispatcherQueue dispatcherQueue)
    {
        DispatcherQueue = dispatcherQueue;
    }

    public DispatcherQueue DispatcherQueue { get; }

    public void BeginInvokeOnMainThread(Action action)
    {
        DispatcherQueue.TryEnqueue(() => action());
    }

    public void InvokeOnMainThread(Action action)
    {
        DispatcherQueue.Invoke(action);
    }

    public T InvokeOnMainThread<T>(Func<T> func)
    {
        return DispatcherQueue.Invoke(func);
    }

    public ThreadPoolSwitchOperation SwitchToBackgroundAsync()
    {
        return new(DispatcherQueue);
    }

    public DispatcherQueueSwitchOperation SwitchToMainThreadAsync()
    {
        return new(DispatcherQueue);
    }
}