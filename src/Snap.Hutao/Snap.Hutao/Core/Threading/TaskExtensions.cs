// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Logging;

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 任务扩展
/// </summary>
[SuppressMessage("", "VSTHRD003")]
[SuppressMessage("", "VSTHRD100")]
public static class TaskExtensions
{
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
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    /// <summary>
    /// 安全的触发任务
    /// </summary>
    /// <param name="task">任务</param>
    /// <param name="logger">日志器</param>
    public static async void SafeForget(this Task task, ILogger? logger = null)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            // Do nothing
        }
        catch (System.Exception e)
        {
            logger?.LogError(EventIds.TaskException, e, "{caller}:\r\n{exception}", nameof(SafeForget), e.GetBaseException());
        }
    }

    /// <summary>
    /// 安全的触发任务
    /// </summary>
    /// <param name="task">任务</param>
    /// <param name="logger">日志器</param>
    /// <param name="onException">发生异常时调用</param>
    public static async void SafeForget(this Task task, ILogger? logger = null, Action<System.Exception>? onException = null)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            // Do nothing
        }
        catch (System.Exception e)
        {
            logger?.LogError(EventIds.TaskException, e, "{caller}:\r\n{exception}", nameof(SafeForget), e.GetBaseException());
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
    public static async void SafeForget(this Task task, ILogger? logger = null, Action? onCanceled = null, Action<System.Exception>? onException = null)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            onCanceled?.Invoke();
        }
        catch (System.Exception e)
        {
            logger?.LogError(EventIds.TaskException, e, "{caller}:\r\n{exception}", nameof(SafeForget), e.GetBaseException());
            onException?.Invoke(e);
        }
    }
}