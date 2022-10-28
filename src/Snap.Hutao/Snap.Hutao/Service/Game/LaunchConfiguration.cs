// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 启动游戏配置
/// </summary>
internal struct LaunchConfiguration
{
    /// <summary>
    /// 是否全屏，全屏时无边框设置将被覆盖
    /// </summary>
    public bool IsFullScreen { get; set; }

    /// <summary>
    /// Override fullscreen windowed mode. Accepted values are exclusive or borderless.
    /// </summary>
    public string WindowMode { get; private set; }

    /// <summary>
    /// 是否启用解锁帧率
    /// </summary>
    public bool UnlockFPS { get; private set; }

    /// <summary>
    /// 目标帧率
    /// </summary>
    public int TargetFPS { get; private set; }

    /// <summary>
    /// 窗口宽度
    /// </summary>
    public int ScreenWidth { get; private set; }

    /// <summary>
    /// 窗口高度
    /// </summary>
    public int ScreenHeight { get; private set; }

    /// <summary>
    /// 显示器编号
    /// </summary>
    public int Monitor { get; private set; }
}