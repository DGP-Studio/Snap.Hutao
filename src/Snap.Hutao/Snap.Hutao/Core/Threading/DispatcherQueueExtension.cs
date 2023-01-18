// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 调度器队列拓展
/// </summary>
public static class DispatcherQueueExtension
{
    /// <summary>
    /// 在调度器队列同步调用，直到执行结束，会持续阻塞当前线程
    /// </summary>
    /// <param name="dispatcherQueue">调度器队列</param>
    /// <param name="action">执行的回调</param>
    public static void Invoke(this DispatcherQueue dispatcherQueue, Action action)
    {
        ManualResetEventSlim blockEvent = new();
        dispatcherQueue.TryEnqueue(() =>
        {
            action();
            blockEvent.Set();
        });

        blockEvent.Wait();
    }
}