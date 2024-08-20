// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 任务扩展
/// </summary>
[HighQuality]
internal static class TaskExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask AsValueTask(this Task task)
    {
        return new(task);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<T> AsValueTask<T>(this Task<T> task)
    {
        return new(task);
    }

#if NET9_0_OR_GREATER
    [Obsolete("SafeForget without logger is not recommended.")]
#endif
    public static async void SafeForget(this Task task)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
#if DEBUG
        catch (Exception ex)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(ExceptionFormat.Format(ex));
                System.Diagnostics.Debugger.Break();
            }
        }
#else
        catch
        {
        }
#endif
    }

    public static async void SafeForget(this Task task, ILogger logger)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception e)
        {
            logger?.LogError(e, "SafeForget:\r\n{Exception}", ExceptionFormat.Format(e.GetBaseException()));
        }
    }

    public static async void SafeForget(this Task task, ILogger logger, Action<Exception> onException)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception e)
        {
            logger?.LogError(e, "SafeForget:\r\n{Exception}", ExceptionFormat.Format(e.GetBaseException()));
            onException?.Invoke(e);
        }
    }

    public static async void SafeForget(this Task task, ILogger logger, Action onCanceled, Action<Exception>? onException = null)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            onCanceled?.Invoke();
        }
        catch (Exception e)
        {
            logger?.LogError(e, "SafeForget:\r\n{Exception}", ExceptionFormat.Format(e.GetBaseException()));
            onException?.Invoke(e);
        }
    }

#if NET9_0_OR_GREATER
    [Obsolete("SafeForget without logger is not recommended.")]
#endif
    public static async void SafeForget(this ValueTask task)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
#if DEBUG
        catch (Exception ex)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine(ExceptionFormat.Format(ex));
                System.Diagnostics.Debugger.Break();
            }
        }
#else
        catch
        {
        }
#endif
    }

    public static async void SafeForget(this ValueTask task, ILogger logger)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception e)
        {
            logger?.LogError(e, "SafeForget:\r\n{Exception}", ExceptionFormat.Format(e.GetBaseException()));
        }
    }

    public static async void SafeForget(this ValueTask task, ILogger logger, Action<Exception> onException)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception e)
        {
            logger?.LogError(e, "SafeForget:\r\n{Exception}", ExceptionFormat.Format(e.GetBaseException()));
            onException?.Invoke(e);
        }
    }

    public static async void SafeForget(this ValueTask task, ILogger logger, Action onCanceled, Action<Exception>? onException = null)
    {
        try
        {
            await task.ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            onCanceled?.Invoke();
        }
        catch (Exception e)
        {
            logger?.LogError(e, "SafeForget:\r\n{Exception}", ExceptionFormat.Format(e.GetBaseException()));
            onException?.Invoke(e);
        }
    }

    [SuppressMessage("", "SH003")]
    [SuppressMessage("", "SH007")]
    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
        TaskCompletionSource tcs = new();
        using (cancellationToken.Register(s => ((TaskCompletionSource)s!).TrySetResult(), tcs))
        {
            if (task != await Task.WhenAny(task, tcs.Task).ConfigureAwait(true))
            {
                throw new OperationCanceledException(cancellationToken);
            }
        }

        return await task.ConfigureAwait(true);
    }
}