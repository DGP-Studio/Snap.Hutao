// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using Snap.Hutao.Core.Threading.Abstraction;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 调度器队列切换操作
/// </summary>
public readonly struct DispatherQueueSwitchOperation : IAwaitable<DispatherQueueSwitchOperation>, IAwaiter
{
    private readonly DispatcherQueue dispatherQueue;

    /// <summary>
    /// 构造一个新的调度器队列等待器
    /// </summary>
    /// <param name="dispatherQueue">调度器队列</param>
    public DispatherQueueSwitchOperation(DispatcherQueue dispatherQueue)
    {
        this.dispatherQueue = dispatherQueue;
    }

    /// <inheritdoc/>
    public bool IsCompleted
    {
        get => dispatherQueue.HasThreadAccess;
    }

    /// <inheritdoc/>
    public void OnCompleted(Action continuation)
    {
        dispatherQueue.TryEnqueue(() => { continuation(); });
    }

    /// <inheritdoc/>
    public void GetResult()
    {
    }

    /// <inheritdoc/>
    public DispatherQueueSwitchOperation GetAwaiter()
    {
        return this;
    }
}