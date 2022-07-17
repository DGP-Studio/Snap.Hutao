// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Logging;

namespace Snap.Hutao.Extension;

/// <summary>
/// 任务扩展
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// 安全的触发任务
    /// </summary>
    /// <param name="task">任务</param>
    /// <param name="continueOnCapturedContext">是否在捕获的上下文中继续执行</param>
    /// <param name="logger">日志器</param>
    [SuppressMessage("", "VSTHRD003")]
    [SuppressMessage("", "VSTHRD100")]
    public static async void SafeForget(this Task task, bool continueOnCapturedContext = true, ILogger? logger = null)
    {
        try
        {
            await task.ConfigureAwait(continueOnCapturedContext);
        }
        catch (Exception e)
        {
            logger?.LogError(EventIds.TaskException, e, "{caller}:{exception}", nameof(SafeForget), e.GetBaseException());
        }
    }
}
