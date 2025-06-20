// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;

namespace Snap.Hutao.Core.Threading;

#if DEBUG
internal static class TaskContextExtension
{
    public static Task<T> InvokeOnMainThreadAsync<T>(this ITaskContext taskContext, Func<T> func)
    {
        return taskContext.DispatcherQueue.EnqueueAsync(func);
    }

    [Obsolete("Do not pass a function that returns a Task<T> to InvokeOnMainThreadAsync method", true)]
    public static Task<Task<T>> InvokeOnMainThreadAsync<T>(this ITaskContext taskContext, Func<Task<T>> func)
    {
        return Task.FromException<Task<T>>(new NotSupportedException());
    }

    [Obsolete("Do not pass a function that returns a Task to InvokeOnMainThreadAsync method", true)]
    public static Task<Task> InvokeOnMainThreadAsync(this ITaskContext taskContext, Func<Task> func)
    {
        return Task.FromException<Task>(new NotSupportedException());
    }

    [Obsolete("Do not pass a function that returns a ValueTask<T> to InvokeOnMainThreadAsync method", true)]
    public static Task<ValueTask<T>> InvokeOnMainThreadAsync<T>(this ITaskContext taskContext, Func<ValueTask<T>> func)
    {
        return Task.FromException<ValueTask<T>>(new NotSupportedException());
    }

    [Obsolete("Do not pass a function that returns a ValueTask to InvokeOnMainThreadAsync method", true)]
    public static Task<ValueTask> InvokeOnMainThreadAsync(this ITaskContext taskContext, Func<ValueTask> func)
    {
        return Task.FromException<ValueTask>(new NotSupportedException());
    }
}
#endif