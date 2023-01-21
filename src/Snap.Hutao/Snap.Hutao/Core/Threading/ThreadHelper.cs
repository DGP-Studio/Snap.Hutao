// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 线程帮助类
/// </summary>
internal static class ThreadHelper
{
    /// <summary>
    /// 主线程队列
    /// </summary>
    private static volatile DispatcherQueue? dispatcherQueue;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Initialize()
    {
        dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        DispatcherQueueSynchronizationContext context = new(dispatcherQueue);
        SynchronizationContext.SetSynchronizationContext(context);
    }

    /// <summary>
    /// 使用此静态方法以 异步切换到 后台线程
    /// </summary>
    /// <remarks>使用 <see cref="SwitchToMainThreadAsync"/> 异步切换到 主线程</remarks>
    /// <returns>等待体</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThreadPoolSwitchOperation SwitchToBackgroundAsync()
    {
        return default;
    }

    /// <summary>
    /// 使用此静态方法以 异步切换到 主线程
    /// </summary>
    /// <remarks>使用 <see cref="SwitchToBackgroundAsync"/> 异步切换到 后台线程</remarks>
    /// <returns>等待体</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DispatherQueueSwitchOperation SwitchToMainThreadAsync()
    {
        return new(dispatcherQueue!);
    }

    /// <summary>
    /// 在主线程上同步等待执行操作
    /// </summary>
    /// <param name="action">操作</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InvokeOnMainThread(Action action)
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