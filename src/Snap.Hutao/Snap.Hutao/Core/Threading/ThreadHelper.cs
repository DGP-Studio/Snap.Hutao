// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 线程帮助类
/// </summary>
[Obsolete("Use TaskContext instead")]
internal static class ThreadHelper
{
    /// <summary>
    /// 主线程队列
    /// </summary>
    private static volatile ITaskContext taskContext;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Initialize(ITaskContext taskContext)
    {
        ThreadHelper.taskContext = taskContext;
    }

    /// <summary>
    /// 使用此静态方法以 异步切换到 后台线程
    /// </summary>
    /// <remarks>使用 <see cref="SwitchToMainThreadAsync"/> 异步切换到 主线程</remarks>
    /// <returns>等待体</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ThreadPoolSwitchOperation SwitchToBackgroundAsync()
    {
        return taskContext.SwitchToBackgroundAsync();
    }

    /// <summary>
    /// 使用此静态方法以 异步切换到 主线程
    /// </summary>
    /// <remarks>使用 <see cref="SwitchToBackgroundAsync"/> 异步切换到 后台线程</remarks>
    /// <returns>等待体</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DispatherQueueSwitchOperation SwitchToMainThreadAsync()
    {
        return taskContext.SwitchToMainThreadAsync();
    }

    /// <summary>
    /// 在主线程上同步等待执行操作
    /// </summary>
    /// <param name="action">操作</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InvokeOnMainThread(Action action)
    {
        taskContext.InvokeOnMainThread(action);
    }
}