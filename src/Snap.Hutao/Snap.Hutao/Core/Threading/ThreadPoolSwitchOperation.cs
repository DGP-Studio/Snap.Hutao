// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using Snap.Hutao.Core.Threading.Abstraction;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 线程池切换操作
/// 等待此类型对象后上下文会被切换至线程池线程
/// </summary>
internal readonly struct ThreadPoolSwitchOperation : IAwaitable<ThreadPoolSwitchOperation>, ICriticalAwaiter
{
    private static readonly WaitCallback WaitCallbackRunAction = RunAction;
    private readonly DispatcherQueue dispatherQueue;

    /// <summary>
    /// 构造一个新的线程池切换操作
    /// </summary>
    /// <param name="dispatherQueue">主线程队列</param>
    public ThreadPoolSwitchOperation(DispatcherQueue dispatherQueue)
    {
        this.dispatherQueue = dispatherQueue;
    }

    /// <inheritdoc/>
    public bool IsCompleted
    {
        // 如果已经处于后台就不再切换新的线程
        get => !dispatherQueue.HasThreadAccess;
    }

    /// <inheritdoc/>
    public ThreadPoolSwitchOperation GetAwaiter()
    {
        return this;
    }

    /// <inheritdoc/>
    public void GetResult()
    {
    }

    /// <inheritdoc/>
    public void OnCompleted(Action continuation)
    {
        QueueContinuation(continuation, true);
    }

    /// <inheritdoc/>
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