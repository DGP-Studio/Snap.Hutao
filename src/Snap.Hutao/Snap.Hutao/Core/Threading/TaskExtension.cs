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

    /// <summary>
    /// 安全的触发任务
    /// </summary>
    /// <param name="task">任务</param>
    public static async void SafeForget(this Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
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

    /// <summary>
    /// 安全的触发任务
    /// </summary>
    /// <param name="task">任务</param>
    /// <param name="logger">日志器</param>
    public static async void SafeForget(this Task task, ILogger logger)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception e)
        {
            logger?.LogError(e, "{Caller}:\r\n{Exception}", nameof(SafeForget), ExceptionFormat.Format(e.GetBaseException()));
        }
    }

    /// <summary>
    /// 安全的触发任务
    /// </summary>
    /// <param name="task">任务</param>
    /// <param name="logger">日志器</param>
    /// <param name="onException">发生异常时调用</param>
    public static async void SafeForget(this Task task, ILogger logger, Action<Exception> onException)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception e)
        {
            logger?.LogError(e, "{Caller}:\r\n{Exception}", nameof(SafeForget), ExceptionFormat.Format(e.GetBaseException()));
            onException?.Invoke(e);
        }
    }

    /// <summary>
    /// 安全的触发任务
    /// </summary>
    /// <param name="task">任务</param>
    /// <param name="logger">日志器</param>
    /// <param name="onCanceled">任务取消时调用</param>
    /// <param name="onException">发生异常时调用</param>
    public static async void SafeForget(this Task task, ILogger logger, Action onCanceled, Action<Exception>? onException = null)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            onCanceled?.Invoke();
        }
        catch (Exception e)
        {
            logger?.LogError(e, "{Caller}:\r\n{Exception}", nameof(SafeForget), ExceptionFormat.Format(e.GetBaseException()));
            onException?.Invoke(e);
        }
    }

    /// <summary>
    /// 安全的触发任务
    /// </summary>
    /// <param name="task">任务</param>
    public static async void SafeForget(this ValueTask task)
    {
        try
        {
            await task.ConfigureAwait(false);
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

    /// <summary>
    /// 安全的触发任务
    /// </summary>
    /// <param name="task">任务</param>
    /// <param name="logger">日志器</param>
    public static async void SafeForget(this ValueTask task, ILogger logger)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception e)
        {
            logger?.LogError(e, "{Caller}:\r\n{Exception}", nameof(SafeForget), ExceptionFormat.Format(e.GetBaseException()));
        }
    }

    /// <summary>
    /// 安全的触发任务
    /// </summary>
    /// <param name="task">任务</param>
    /// <param name="logger">日志器</param>
    /// <param name="onException">发生异常时调用</param>
    public static async void SafeForget(this ValueTask task, ILogger logger, Action<Exception> onException)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception e)
        {
            logger?.LogError(e, "{Caller}:\r\n{Exception}", nameof(SafeForget), ExceptionFormat.Format(e.GetBaseException()));
            onException?.Invoke(e);
        }
    }

    /// <summary>
    /// 安全的触发任务
    /// </summary>
    /// <param name="task">任务</param>
    /// <param name="logger">日志器</param>
    /// <param name="onCanceled">任务取消时调用</param>
    /// <param name="onException">发生异常时调用</param>
    public static async void SafeForget(this ValueTask task, ILogger logger, Action onCanceled, Action<Exception>? onException = null)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            onCanceled?.Invoke();
        }
        catch (Exception e)
        {
            logger?.LogError(e, "{Caller}:\r\n{Exception}", nameof(SafeForget), ExceptionFormat.Format(e.GetBaseException()));
            onException?.Invoke(e);
        }
    }
}