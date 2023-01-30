// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.System.Diagnostics.ToolHelp;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 游戏帧率解锁器异常
/// </summary>
internal class GameFpsUnlockerException : Exception
{
    /// <summary>
    /// 构造一个新的用户数据损坏异常
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="innerException">内部错误</param>
    public GameFpsUnlockerException(Exception innerException)
        : base($"解锁帧率失败: {innerException.Message}", innerException)
    {
    }
}