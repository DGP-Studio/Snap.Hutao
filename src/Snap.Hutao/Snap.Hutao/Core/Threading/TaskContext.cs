// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.Core.Threading;

[Injection(InjectAs.Singleton, typeof(ITaskContext))]
internal sealed class TaskContext : ITaskContext, ITaskContextUnsafe
{
    public TaskContext()
    {
        DispatcherQueue = DispatcherQueue.GetForCurrentThread();
        DispatcherQueueSynchronizationContext synchronizationContext = new(DispatcherQueue);
        SynchronizationContext.SetSynchronizationContext(synchronizationContext);
    }

    public DispatcherQueue DispatcherQueue { get; }

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
        return new(DispatcherQueue);
    }

    public DispatcherQueueSwitchOperation SwitchToMainThreadAsync()
    {
        return new(DispatcherQueue);
    }

    public void InvokeOnMainThread(Action action)
    {
        DispatcherQueue.Invoke(action);
    }

    public T InvokeOnMainThread<T>(Func<T> action)
    {
        return DispatcherQueue.Invoke(action);
    }

    public void BeginInvokeOnMainThread(Action action)
    {
        DispatcherQueue.TryEnqueue(() => action());
    }
}