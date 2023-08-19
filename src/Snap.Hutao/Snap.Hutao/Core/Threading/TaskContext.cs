// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 任务上下文
/// </summary>
[Injection(InjectAs.Singleton, typeof(ITaskContext))]
internal sealed class TaskContext : ITaskContext
{
    private readonly DispatcherQueueSynchronizationContext dispatcherQueueSynchronizationContext;
    private readonly DispatcherQueue dispatcherQueue;

    /// <summary>
    /// 构造一个新的任务上下文
    /// </summary>
    public TaskContext()
    {
        dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        dispatcherQueueSynchronizationContext = new(dispatcherQueue);
        SynchronizationContext.SetSynchronizationContext(dispatcherQueueSynchronizationContext);
    }

    /// <inheritdoc/>
    public ThreadPoolSwitchOperation SwitchToBackgroundAsync()
    {
        return new(dispatcherQueue);
    }

    /// <inheritdoc/>
    public DispatcherQueueSwitchOperation SwitchToMainThreadAsync()
    {
        return new(dispatcherQueue);
    }

    /// <inheritdoc/>
    public void InvokeOnMainThread(Action action)
    {
        if (dispatcherQueue.HasThreadAccess)
        {
            action();
        }
        else
        {
            dispatcherQueue.Invoke(action);
        }
    }

    public IProgress<T> CreateProgressForMainThread<T>(Action<T> handler)
    {
        return new DispatcherQueueProgress<T>(handler, dispatcherQueueSynchronizationContext);
    }
}