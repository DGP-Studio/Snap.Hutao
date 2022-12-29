// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 启动游戏配置
/// </summary>
internal readonly struct LaunchConfiguration
{
    /// <summary>
    /// 是否为独占全屏
    /// </summary>
    public readonly bool IsExclusive;

    /// <summary>
    /// 是否全屏，全屏时无边框设置将被覆盖
    /// </summary>
    public readonly bool IsFullScreen;

    /// <summary>
    /// 是否为无边框窗口
    /// </summary>
    public readonly bool IsBorderless;

    /// <summary>
    /// 窗口宽度
    /// </summary>
    public readonly int ScreenWidth;

    /// <summary>
    /// 窗口高度
    /// </summary>
    public readonly int ScreenHeight;

    /// <summary>
    /// 是否启用解锁帧率
    /// </summary>
    public readonly bool UnlockFPS;

    /// <summary>
    /// 目标帧率
    /// </summary>
    public readonly int TargetFPS;

    /// <summary>
    /// 构造一个新的启动配置
    /// </summary>
    /// <param name="isExclusive">独占全屏</param>
    /// <param name="isFullScreen">全屏</param>
    /// <param name="isBorderless">无边框</param>
    /// <param name="screenWidth">宽度</param>
    /// <param name="screenHeight">高度</param>
    /// <param name="unlockFps">解锁帧率</param>
    /// <param name="targetFps">目标帧率</param>
    public LaunchConfiguration(bool isExclusive, bool isFullScreen, bool isBorderless, int screenWidth, int screenHeight, bool unlockFps, int targetFps)
    {
        IsExclusive = isExclusive;
        IsFullScreen = isFullScreen;
        IsBorderless = isBorderless;
        ScreenHeight = screenHeight;
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;
        UnlockFPS = unlockFps;
        TargetFPS = targetFps;
    }
}