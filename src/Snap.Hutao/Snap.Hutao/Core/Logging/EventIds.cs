// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// 事件Id定义
/// </summary>
internal static class EventIds
{
    /// <summary>
    /// 导航失败
    /// </summary>
    public static readonly EventId NavigationFailed = new(100000, nameof(NavigationFailed));

    /// <summary>
    /// 未处理的异常
    /// </summary>
    public static readonly EventId UnhandledException = new(100001, nameof(UnhandledException));

    /// <summary>
    /// Forget任务执行异常
    /// </summary>
    public static readonly EventId TaskException = new(100002, nameof(TaskException));
}