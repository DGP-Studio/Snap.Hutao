// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System.Globalization;
using Windows.Graphics;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 启动游戏选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class LaunchOptions : DbStoreOptions
{
    private readonly int primaryScreenWidth;
    private readonly int primaryScreenHeight;
    private readonly int primaryScreenFps;

    private bool? isFullScreen;
    private bool? isBorderless;
    private bool? isExclusive;
    private int? screenWidth;
    private int? screenHeight;
    private bool? unlockFps;
    private int? targetFps;
    private NameValue<int>? monitor;

    /// <summary>
    /// 构造一个新的启动游戏选项
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public LaunchOptions(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        RectInt32 primaryRect = DisplayArea.Primary.OuterBounds;
        primaryScreenWidth = primaryRect.Width;
        primaryScreenHeight = primaryRect.Height;

        InitializeMonitors(Monitors);
        InitializeScreenFps(out primaryScreenFps);
    }

    /// <summary>
    /// 是否全屏
    /// </summary>
    public bool IsFullScreen
    {
        get => GetOption(ref isFullScreen, SettingEntry.LaunchIsFullScreen);
        set => SetOption(ref isFullScreen, SettingEntry.LaunchIsFullScreen, value);
    }

    /// <summary>
    /// 是否无边框
    /// </summary>
    public bool IsBorderless
    {
        get => GetOption(ref isBorderless, SettingEntry.LaunchIsBorderless);
        set => SetOption(ref isBorderless, SettingEntry.LaunchIsBorderless, value);
    }

    /// <summary>
    /// 是否独占全屏
    /// </summary>
    public bool IsExclusive
    {
        get => GetOption(ref isExclusive, SettingEntry.LaunchIsExclusive);
        set => SetOption(ref isExclusive, SettingEntry.LaunchIsExclusive, value);
    }

    /// <summary>
    /// 屏幕宽度
    /// </summary>
    public int ScreenWidth
    {
        get => GetOption(ref screenWidth, SettingEntry.LaunchScreenWidth, primaryScreenWidth);
        set => SetOption(ref screenWidth, SettingEntry.LaunchScreenWidth, value);
    }

    /// <summary>
    /// 屏幕高度
    /// </summary>
    public int ScreenHeight
    {
        get => GetOption(ref screenHeight, SettingEntry.LaunchScreenHeight, primaryScreenHeight);
        set => SetOption(ref screenHeight, SettingEntry.LaunchScreenHeight, value);
    }

    /// <summary>
    /// 是否全屏
    /// </summary>
    public bool UnlockFps
    {
        get => GetOption(ref unlockFps, SettingEntry.LaunchUnlockFps);
        set => SetOption(ref unlockFps, SettingEntry.LaunchUnlockFps, value);
    }

    /// <summary>
    /// 目标帧率
    /// </summary>
    public int TargetFps
    {
        get => GetOption(ref targetFps, SettingEntry.LaunchTargetFps, primaryScreenFps);
        set => SetOption(ref targetFps, SettingEntry.LaunchTargetFps, value);
    }

    /// <summary>
    /// 所有监视器
    /// </summary>
    public List<NameValue<int>> Monitors { get; } = new();

    /// <summary>
    /// 目标帧率
    /// </summary>
    [AllowNull]
    public NameValue<int> Monitor
    {
        get => GetOption(ref monitor, SettingEntry.LaunchMonitor, index => Monitors[int.Parse(index, CultureInfo.InvariantCulture) - 1], Monitors[0]);
        set
        {
            if (value is not null)
            {
                SetOption(ref monitor, SettingEntry.LaunchMonitor, value, selected => selected.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }

    private static void InitializeMonitors(List<NameValue<int>> monitors)
    {
        // This list can't use foreach
        // https://github.com/microsoft/CsWinRT/issues/747
        IReadOnlyList<DisplayArea> displayAreas = DisplayArea.FindAll();
        for (int i = 0; i < displayAreas.Count; i++)
        {
            DisplayArea displayArea = displayAreas[i];
            int index = i + 1;
            monitors.Add(new($"{displayArea.DisplayId.Value:X8}:{index}", index));
        }
    }

    private static void InitializeScreenFps(out int fps)
    {
        HDC hDC = default;
        try
        {
            hDC = GetDC(HWND.Null);
            fps = GetDeviceCaps(hDC, GET_DEVICE_CAPS_INDEX.VREFRESH);
        }
        finally
        {
            _ = ReleaseDC(HWND.Null, hDC);
        }
    }
}