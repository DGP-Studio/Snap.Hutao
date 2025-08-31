// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using WinRT;

namespace Snap.Hutao.Core.Threading;

[Service(ServiceLifetime.Singleton, typeof(ITaskContext))]
internal sealed class TaskContext : ITaskContext
{
    public TaskContext()
    {
        DispatcherQueue = DispatcherQueue.GetForCurrentThread();
        PrivateSynchronizationContext synchronizationContext = new(DispatcherQueue);
        SynchronizationContext.SetSynchronizationContext(synchronizationContext);
        XamlApplicationLifetime.DispatcherQueueInitialized = true;
    }

    public DispatcherQueue DispatcherQueue { get; }

    public static ITaskContext GetForDependencyObject(DependencyObject dependencyObject)
    {
        return GetForDispatcherQueue(dependencyObject.DispatcherQueue);
    }

    public static ITaskContext GetForDispatcherQueue(DispatcherQueue dispatcherQueue)
    {
        return new TaskContextWrapperForDispatcherQueue(dispatcherQueue);
    }

    public ThreadPoolSwitchOperation SwitchToBackgroundAsync()
    {
        return new(DispatcherQueue);
    }

    public DispatcherQueueSwitchOperation SwitchToMainThreadAsync()
    {
        return new(DispatcherQueue);
    }

    public void InvokeOnMainThread(Action action)
    {
        DispatcherQueue.Invoke(action);
    }

    public T InvokeOnMainThread<T>(Func<T> func)
    {
        return DispatcherQueue.Invoke(func);
    }

    public void BeginInvokeOnMainThread(Action action)
    {
        DispatcherQueue.TryEnqueue(() => action());
    }

    private sealed class PrivateSynchronizationContext : SynchronizationContext
    {
        private readonly DispatcherQueue dispatcherQueue;

        public PrivateSynchronizationContext(DispatcherQueue dispatcherQueue)
        {
            this.dispatcherQueue = dispatcherQueue;
        }

        public override void Post(SendOrPostCallback callback, object? state)
        {
            ArgumentNullException.ThrowIfNull(callback);
            dispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    callback(state);
                }
                catch (Exception ex)
                {
                    ExceptionHelpers.ReportUnhandledError(ex);
                }
            });
        }

        public override void Send(SendOrPostCallback callback, object? state)
        {
            ArgumentNullException.ThrowIfNull(callback);
            dispatcherQueue.Invoke(() =>
            {
                try
                {
                    callback(state);
                }
                catch (Exception ex)
                {
                    ExceptionHelpers.ReportUnhandledError(ex);
                }
            });
        }

        public override SynchronizationContext CreateCopy()
        {
            return new PrivateSynchronizationContext(dispatcherQueue);
        }
    }
}