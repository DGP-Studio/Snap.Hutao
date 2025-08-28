// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.ObjectPool;
using Microsoft.UI.Dispatching;
using System.Runtime.ExceptionServices;

namespace Snap.Hutao.Core.Threading;

internal static class DispatcherQueueExtension
{
    // TODO: DisposableObjectPool: Consider disposing the pool when application exits.
    // Once disposed, Invoke methods will not work anymore.
    private static readonly ObjectPool<ManualResetEventSlim> EventPool = new DefaultObjectPoolProvider().Create(new PooledManualResetEventSlimPolicy());

    public static void Invoke(this DispatcherQueue dispatcherQueue, Action action)
    {
        if (dispatcherQueue.HasThreadAccess)
        {
            action();
            return;
        }

        ExceptionDispatchInfo? exceptionDispatchInfo = null;
        ManualResetEventSlim blockEvent = EventPool.Get();

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
        EventPool.Return(blockEvent);

        exceptionDispatchInfo?.Throw();
    }

    public static void Invoke(this DispatcherQueue dispatcherQueue, DispatcherQueuePriority priority, Action action)
    {
        if (dispatcherQueue.HasThreadAccess)
        {
            action();
            return;
        }

        ExceptionDispatchInfo? exceptionDispatchInfo = null;
        ManualResetEventSlim blockEvent = EventPool.Get();
        dispatcherQueue.TryEnqueue(priority, () =>
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
        EventPool.Return(blockEvent);

        exceptionDispatchInfo?.Throw();
    }

    public static T Invoke<T>(this DispatcherQueue dispatcherQueue, Func<T> func)
    {
        if (dispatcherQueue.HasThreadAccess)
        {
            return func();
        }

        T result = default!;
        ExceptionDispatchInfo? exceptionDispatchInfo = null;
        ManualResetEventSlim blockEvent = EventPool.Get();
        dispatcherQueue.TryEnqueue(() =>
        {
            try
            {
                result = func();
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
        EventPool.Return(blockEvent);

        exceptionDispatchInfo?.Throw();
        return result;
    }

    public static T Invoke<T>(this DispatcherQueue dispatcherQueue, DispatcherQueuePriority priority, Func<T> func)
    {
        if (dispatcherQueue.HasThreadAccess)
        {
            return func();
        }

        T result = default!;
        ExceptionDispatchInfo? exceptionDispatchInfo = null;
        ManualResetEventSlim blockEvent = EventPool.Get();
        dispatcherQueue.TryEnqueue(priority, () =>
        {
            try
            {
                result = func();
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
        EventPool.Return(blockEvent);

        exceptionDispatchInfo?.Throw();
        return result;
    }

    private sealed class PooledManualResetEventSlimPolicy : PooledObjectPolicy<ManualResetEventSlim>
    {
        public override ManualResetEventSlim Create()
        {
            return new(false);
        }

        public override bool Return(ManualResetEventSlim @event)
        {
            try
            {
                @event.Reset();
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }
    }
}