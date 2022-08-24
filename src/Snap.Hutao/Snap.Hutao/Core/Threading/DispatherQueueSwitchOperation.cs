// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using Snap.Hutao.Core.Threading.Abstraction;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 调度器队列切换操作
/// </summary>
public struct DispatherQueueSwitchOperation : IAwaitable<DispatherQueueSwitchOperation>, IAwaiter
{
    private readonly DispatcherQueue dispatherQueue;

    /// <summary>
    /// 构造一个新的同步上下文等待器
    /// </summary>
    /// <param name="dispatherQueue">同步上下文</param>
    public DispatherQueueSwitchOperation(DispatcherQueue dispatherQueue)
    {
        this.dispatherQueue = dispatherQueue;
    }

    /// <summary>
    /// 是否完成
    /// </summary>
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

    /// <summary>
    /// 获取等待器
    /// </summary>
    /// <returns>等待器</returns>
    public DispatherQueueSwitchOperation GetAwaiter()
    {
        return this;
    }
}