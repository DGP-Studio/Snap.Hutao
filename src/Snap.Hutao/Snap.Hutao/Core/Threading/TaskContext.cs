// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.Core.Threading;

[Injection(InjectAs.Singleton, typeof(ITaskContext))]
internal sealed class TaskContext : ITaskContext, ITaskContextUnsafe
{
    private readonly DispatcherQueue dispatcherQueue;

    public TaskContext()
    {
        dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        DispatcherQueueSynchronizationContext synchronizationContext = new(dispatcherQueue);
        SynchronizationContext.SetSynchronizationContext(synchronizationContext);
    }

    public DispatcherQueue DispatcherQueue { get => dispatcherQueue; }

    public static ITaskContext GetForDependencyObject(DependencyObject dependencyObject)
    {
        return GetForDispatcherQueue(dependencyObject.DispatcherQueue);
    }

    public static ITaskContext GetForDispatcherQueue(DispatcherQueue dispatcherQueue)
    {
        return new TaskContextWrapperForDispatcherQueue(dispatcherQueue);
    }

    public ThreadPoolSwitchOperation SwitchToBackgroundAsync()
    {
        return new(dispatcherQueue);
    }

    public DispatcherQueueSwitchOperation SwitchToMainThreadAsync()
    {
        return new(dispatcherQueue);
    }

    public void InvokeOnMainThread(Action action)
    {
        dispatcherQueue.Invoke(action);
    }

    public T InvokeOnMainThread<T>(Func<T> action)
    {
        return dispatcherQueue.Invoke(action);
    }

    public void BeginInvokeOnMainThread(Action action)
    {
        dispatcherQueue.TryEnqueue(() => action());
    }
}