// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Threading.Abstraction;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 线程池切换操作
/// 等待此类型对象后上下文会被切换至线程池线程
/// </summary>
public readonly struct ThreadPoolSwitchOperation : IAwaitable<ThreadPoolSwitchOperation>, IAwaiter, ICriticalAwaiter
{
    private static readonly WaitCallback WaitCallbackRunAction = RunAction;

    /// <inheritdoc/>
    public bool IsCompleted { get => false; }

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
        QueueContinuation(continuation, flowContext: true);
    }

    /// <inheritdoc/>
    public void UnsafeOnCompleted(Action continuation)
    {
        QueueContinuation(continuation, flowContext: false);
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

    private static void RunAction(object? state)
    {
        ((Action)state!)();
    }
}