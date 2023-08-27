// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using Snap.Hutao.Core.Threading.Abstraction;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 调度器队列切换操作
/// 等待此类型对象后上下文会被切换至主线程
/// </summary>
[HighQuality]
internal readonly struct DispatcherQueueSwitchOperation : IAwaitable<DispatcherQueueSwitchOperation>, ICriticalAwaiter
{
    private readonly DispatcherQueue dispatherQueue;

    /// <summary>
    /// 构造一个新的调度器队列等待器
    /// </summary>
    /// <param name="dispatherQueue">调度器队列</param>
    public DispatcherQueueSwitchOperation(DispatcherQueue dispatherQueue)
    {
        this.dispatherQueue = dispatherQueue;
    }

    /// <inheritdoc/>
    public bool IsCompleted
    {
        get => dispatherQueue.HasThreadAccess;
    }

    /// <inheritdoc/>
    public DispatcherQueueSwitchOperation GetAwaiter()
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
        dispatherQueue.TryEnqueue(new DispatcherQueueHandler(continuation));
    }

    /// <inheritdoc/>
    public void UnsafeOnCompleted(Action continuation)
    {
        using (ExecutionContext.SuppressFlow())
        {
            dispatherQueue.TryEnqueue(new DispatcherQueueHandler(continuation));
        }
    }
}