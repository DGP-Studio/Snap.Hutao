// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using System.Runtime.ExceptionServices;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 调度器队列拓展
/// </summary>
[HighQuality]
internal static class DispatcherQueueExtension
{
    /// <summary>
    /// 在调度器队列同步调用，直到执行结束，会持续阻塞当前线程
    /// </summary>
    /// <param name="dispatcherQueue">调度器队列</param>
    /// <param name="action">执行的回调</param>
    public static void Invoke(this DispatcherQueue dispatcherQueue, Action action)
    {
        if (dispatcherQueue.HasThreadAccess)
        {
            action();
            return;
        }

        ExceptionDispatchInfo? exceptionDispatchInfo = null;
        using (ManualResetEventSlim blockEvent = new(false))
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    ExceptionDispatchInfo.Capture(ex);
                }
                finally
                {
                    blockEvent.Set();
                }
            });

            blockEvent.Wait();
#pragma warning disable CA1508
            exceptionDispatchInfo?.Throw();
#pragma warning restore CA1508
        }
    }
}