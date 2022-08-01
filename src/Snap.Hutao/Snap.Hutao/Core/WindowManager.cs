// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Control.HostBackdrop;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Core.Win32;
using WinRT.Interop;

namespace Snap.Hutao.Core;

/// <summary>
/// 窗口状态管理器
/// </summary>
internal class WindowManager
{
    private readonly IntPtr handle;
    private readonly Window window;
    private readonly UIElement titleBar;
    private readonly ILogger<WindowManager> logger;

    /// <summary>
    /// 构造一个新的窗口状态管理器
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="titleBar">充当标题栏的元素</param>
    public WindowManager(Window window, UIElement titleBar)
    {
        this.window = window;
        this.titleBar = titleBar;
        logger = Ioc.Default.GetRequiredService<ILogger<WindowManager>>();

        handle = WindowNative.GetWindowHandle(window);

        InitializeWindow();
    }

    private static RECT RetriveWindowRect()
    {
        int left = LocalSetting.GetValueType<int>(SettingKeys.WindowLeft);
        int top = LocalSetting.GetValueType<int>(SettingKeys.WindowTop);
        int right = LocalSetting.GetValueType<int>(SettingKeys.WindowRight);
        int bottom = LocalSetting.GetValueType<int>(SettingKeys.WindowBottom);

        return new RECT(left, top, right, bottom);
    }

    private static void SaveWindowRect(IntPtr handle)
    {
        WINDOWPLACEMENT windowPlacement = WINDOWPLACEMENT.Default;
        User32.GetWindowPlacement(handle, ref windowPlacement);

        LocalSetting.Set(SettingKeys.WindowLeft, windowPlacement.NormalPosition.Left);
        LocalSetting.Set(SettingKeys.WindowTop, windowPlacement.NormalPosition.Top);
        LocalSetting.Set(SettingKeys.WindowRight, windowPlacement.NormalPosition.Right);
        LocalSetting.Set(SettingKeys.WindowBottom, windowPlacement.NormalPosition.Bottom);
    }

    private void InitializeWindow()
    {
        window.ExtendsContentIntoTitleBar = true;
        window.SetTitleBar(titleBar);
        window.Closed += OnWindowClosed;

        User32.SetWindowText(handle, "胡桃");
        RECT rect = RetriveWindowRect();
        if (rect.Area > 0)
        {
            WINDOWPLACEMENT windowPlacement = WINDOWPLACEMENT.Create(new POINT(-1, -1), rect, ShowWindowCommand.Normal);
            User32.SetWindowPlacement(handle, ref windowPlacement);
        }

        bool micaApplied = new SystemBackdrop(window).TrySetBackdrop();
        logger.LogInformation(EventIds.BackdropState, "Apply {name} : {result}", nameof(SystemBackdrop), micaApplied ? "succeed" : "failed");
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        SaveWindowRect(handle);
    }
}