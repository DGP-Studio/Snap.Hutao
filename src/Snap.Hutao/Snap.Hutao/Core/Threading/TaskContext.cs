// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 任务上下文
/// </summary>
[Injection(InjectAs.Singleton, typeof(ITaskContext))]
internal sealed class TaskContext : ITaskContext
{
    private readonly DispatcherQueue dispatcherQueue;

    /// <summary>
    /// 构造一个新的任务上下文
    /// </summary>
    public TaskContext()
    {
        dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        DispatcherQueueSynchronizationContext context = new(dispatcherQueue);
        SynchronizationContext.SetSynchronizationContext(context);
    }

    /// <inheritdoc/>
    public ThreadPoolSwitchOperation SwitchToBackgroundAsync()
    {
        return new(dispatcherQueue);
    }

    /// <inheritdoc/>
    public DispatherQueueSwitchOperation SwitchToMainThreadAsync()
    {
        return new(dispatcherQueue);
    }

    /// <inheritdoc/>
    public void InvokeOnMainThread(Action action)
    {
        if (dispatcherQueue!.HasThreadAccess)
        {
            action();
        }
        else
        {
            dispatcherQueue.Invoke(action);
        }
    }
}