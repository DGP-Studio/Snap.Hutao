// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 任务上下文
/// </summary>
internal interface ITaskContext
{
    SynchronizationContext GetSynchronizationContext();

    /// <summary>
    /// 在主线程上同步等待执行操作
    /// </summary>
    /// <param name="action">操作</param>
    void InvokeOnMainThread(Action action);

    /// <summary>
    /// 异步切换到 后台线程
    /// </summary>
    /// <remarks>使用 <see cref="SwitchToMainThreadAsync"/> 异步切换到 主线程</remarks>
    /// <returns>等待体</returns>
    ThreadPoolSwitchOperation SwitchToBackgroundAsync();

    /// <summary>
    /// 异步切换到 主线程
    /// </summary>
    /// <remarks>使用 <see cref="SwitchToBackgroundAsync"/> 异步切换到 后台线程</remarks>
    /// <returns>等待体</returns>
    DispatcherQueueSwitchOperation SwitchToMainThreadAsync();
}