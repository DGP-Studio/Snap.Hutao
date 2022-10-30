// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 线程帮助类
/// </summary>
internal static class ThreadHelper
{
    /// <summary>
    /// 使用此静态方法以 异步切换到 主线程
    /// </summary>
    /// <remarks>使用 <see cref="Task.Yield"/> 异步切换到 后台线程</remarks>
    /// <returns>等待体</returns>
    public static DispatherQueueSwitchOperation SwitchToMainThreadAsync()
    {
        return new(Program.DispatcherQueue!);
    }
}
