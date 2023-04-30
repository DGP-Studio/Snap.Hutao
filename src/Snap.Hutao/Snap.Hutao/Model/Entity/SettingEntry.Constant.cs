// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 键名
/// </summary>
internal sealed partial class SettingEntry
{
    /// <summary>
    /// 游戏路径
    /// </summary>
    public const string GamePath = "GamePath";

    /// <summary>
    /// 空的历史记录卡池是否可见
    /// </summary>
    public const string IsEmptyHistoryWishVisible = "IsEmptyHistoryWishVisible";

    /// <summary>
    /// 窗口背景类型
    /// </summary>
    public const string SystemBackdropType = "SystemBackdropType";

    /// <summary>
    /// 启用高级功能
    /// </summary>
    public const string IsAdvancedLaunchOptionsEnabled = "IsAdvancedLaunchOptionsEnabled";

    /// <summary>
    /// 实时便笺刷新时间
    /// </summary>
    public const string DailyNoteRefreshSeconds = "DailyNote.RefreshSeconds";

    /// <summary>
    /// 实时便笺提醒式通知
    /// </summary>
    public const string DailyNoteReminderNotify = "DailyNote.ReminderNotify";

    /// <summary>
    /// 实时便笺免打扰模式
    /// </summary>
    public const string DailyNoteSilentWhenPlayingGame = "DailyNote.SilentWhenPlayingGame";

    /// <summary>
    /// 启动游戏 独占全屏
    /// </summary>
    public const string LaunchIsExclusive = "Launch.IsExclusive";

    /// <summary>
    /// 启动游戏 全屏
    /// </summary>
    public const string LaunchIsFullScreen = "Launch.IsFullScreen";

    /// <summary>
    /// 启动游戏 无边框
    /// </summary>
    public const string LaunchIsBorderless = "Launch.IsBorderless";

    /// <summary>
    /// 启动游戏 宽度
    /// </summary>
    public const string LaunchScreenWidth = "Launch.ScreenWidth";

    /// <summary>
    /// 启动游戏 高度
    /// </summary>
    public const string LaunchScreenHeight = "Launch.ScreenHeight";

    /// <summary>
    /// 启动游戏 解锁帧率
    /// </summary>
    public const string LaunchUnlockFps = "Launch.UnlockFps";

    /// <summary>
    /// 启动游戏 目标帧率
    /// </summary>
    public const string LaunchTargetFps = "Launch.TargetFps";

    /// <summary>
    /// 启动游戏 显示器编号
    /// </summary>
    public const string LaunchMonitor = "Launch.Monitor";

    /// <summary>
    /// 启动游戏 多倍启动
    /// </summary>
    public const string MultipleInstances = "Launch.MultipleInstances";

    /// <summary>
    /// 语言
    /// </summary>
    public const string Culture = "Culture";
}
