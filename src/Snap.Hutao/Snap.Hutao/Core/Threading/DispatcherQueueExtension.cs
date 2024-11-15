// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using System.Runtime.ExceptionServices;

namespace Snap.Hutao.Core.Threading;

internal static class DispatcherQueueExtension
{
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
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                }
                finally
                {
                    blockEvent.Set();
                }
            });

            blockEvent.Wait();
            exceptionDispatchInfo?.Throw();
        }
    }

    public static T Invoke<T>(this DispatcherQueue dispatcherQueue, Func<T> action)
    {
        T result = default!;

        if (dispatcherQueue.HasThreadAccess)
        {
            return action();
        }

        ExceptionDispatchInfo? exceptionDispatchInfo = null;
        using (ManualResetEventSlim blockEvent = new(false))
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    result = action();
                }
                catch (Exception ex)
                {
                    exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                }
                finally
                {
                    blockEvent.Set();
                }
            });

            blockEvent.Wait();
            exceptionDispatchInfo?.Throw();
            return result;
        }
    }
}