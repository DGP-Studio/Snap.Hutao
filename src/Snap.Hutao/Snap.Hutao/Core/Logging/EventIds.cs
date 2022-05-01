// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Logging;

namespace Snap.Hutao.Core.Logging;

/// <summary>
/// 事件Id定义
/// </summary>
internal static class EventIds
{
    /// <summary>
    /// 导航失败
    /// </summary>
    public static readonly EventId NavigationFailed = new(100000, "NavigationFailed");
}
