// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 线程帮助类
/// </summary>
internal static class ThreadHelper
{
    /// <summary>
    /// 异步切换到主线程
    /// </summary>
    /// <returns>等待体</returns>
    public static DispatherQueueSwitchOperation SwitchToMainThreadAsync()
    {
        return new(Program.DispatcherQueue!);
    }
}
