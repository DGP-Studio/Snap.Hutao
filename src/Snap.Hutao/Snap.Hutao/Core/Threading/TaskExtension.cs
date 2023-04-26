// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// 任务扩展
/// </summary>
[HighQuality]
[SuppressMessage("", "VSTHRD003")]
[SuppressMessage("", "VSTHRD100")]
internal static class TaskExtension
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
        catch (OperationCanceledException)
        {
            // Do nothing
        }
#if DEBUG
        catch (Exception ex)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                _ = ex;
                System.Diagnostics.Debugger.Break();
            }
        }
#else
        catch (Exception)
        {
        }
#endif
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
        catch (OperationCanceledException)
        {
            // Do nothing
        }
        catch (Exception e)
        {
            logger?.LogError(e, "{caller}:\r\n{exception}", nameof(SafeForget), e.GetBaseException());
        }
    }

    /// <summary>
    /// 安全的触发任务
    /// </summary>
    /// <param name="task">任务</param>
    /// <param name="logger">日志器</param>
    /// <param name="onException">发生异常时调用</param>
    public static async void SafeForget(this Task task, ILogger? logger = null, Action<Exception>? onException = null)
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
            logger?.LogError(e, "{caller}:\r\n{exception}", nameof(SafeForget), e.GetBaseException());
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
    public static async void SafeForget(this Task task, ILogger? logger = null, Action? onCanceled = null, Action<Exception>? onException = null)
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
            logger?.LogError(e, "{caller}:\r\n{exception}", nameof(SafeForget), e.GetBaseException());
            onException?.Invoke(e);
        }
    }
}