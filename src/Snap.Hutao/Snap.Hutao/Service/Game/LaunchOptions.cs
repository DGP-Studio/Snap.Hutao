﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
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
    private bool? multipleInstances;
    private bool? dllInjector;
    private string? dllPath;

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

        // This list can't use foreach
        // https://github.com/microsoft/microsoft-ui-xaml/issues/6454
        IReadOnlyList<DisplayArea> displayAreas = DisplayArea.FindAll();
        for (int i = 0; i < displayAreas.Count; i++)
        {
            DisplayArea displayArea = displayAreas[i];
            int index = i + 1;
            Monitors.Add(new($"{displayArea.DisplayId.Value:X8}:{index}", index));
        }

        InitializeScreenFps(out primaryScreenFps);
    }

    /// <summary>
    /// 是否全屏
    /// </summary>
    public bool IsFullScreen
    {
        get => GetOption(ref isFullScreen, SettingEntry.LaunchIsFullScreen);
        set
        {
            if (SetOption(ref isFullScreen, SettingEntry.LaunchIsFullScreen, value) && value)
            {
                IsBorderless = false;
            }
        }
    }

    /// <summary>
    /// 是否无边框
    /// </summary>
    public bool IsBorderless
    {
        get => GetOption(ref isBorderless, SettingEntry.LaunchIsBorderless);
        set
        {
            if (SetOption(ref isBorderless, SettingEntry.LaunchIsBorderless, value) && value)
            {
                IsExclusive = false;
                IsFullScreen = false;
            }
        }
    }

    /// <summary>
    /// 是否独占全屏
    /// </summary>
    public bool IsExclusive
    {
        get => GetOption(ref isExclusive, SettingEntry.LaunchIsExclusive);
        set
        {
            if (SetOption(ref isExclusive, SettingEntry.LaunchIsExclusive, value) && value)
            {
                IsFullScreen = true;
            }
        }
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
    public NameValue<int> Monitor
    {
        get => GetOption(ref monitor, SettingEntry.LaunchMonitor, index => Monitors[int.Parse(index) - 1], Monitors[0]);
        set
        {
            if (value != null)
            {
                SetOption(ref monitor, SettingEntry.LaunchMonitor, value, selected => selected.Value.ToString() ?? "1");
            }
        }
    }

    /// <summary>
    /// 多开启动原神
    /// </summary>
    public bool MultipleInstances
    {
        get => GetOption(ref multipleInstances, SettingEntry.MultipleInstances);
        set => SetOption(ref multipleInstances, SettingEntry.MultipleInstances, value);
    }

    /// <summary>
    /// DLL注入器
    /// </summary>
    public bool DllInjector
    {
        get => GetOption(ref dllInjector, SettingEntry.DllInjector);
        set => SetOption(ref dllInjector, SettingEntry.DllInjector, value);
    }

    /// <summary>
    /// DLL路径
    /// </summary>
    public string DllPath
    {
        get => GetOption(ref dllPath, SettingEntry.DllPath);
        set => SetOption(ref dllPath, SettingEntry.DllPath, value);
    }

    private static void InitializeScreenFps(out int fps)
    {
        HDC hDC = GetDC(HWND.Null);
        fps = GetDeviceCaps(hDC, GET_DEVICE_CAPS_INDEX.VREFRESH);
        if (ReleaseDC(HWND.Null, hDC) == 0)
        {
            // not released
            throw new Win32Exception();
        }
    }
}