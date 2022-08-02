// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Control.HostBackdrop;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using Windows.Win32.UI.WindowsAndMessaging;
using WinRT.Interop;

using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core;

/// <summary>
/// 窗口管理器
/// 主要包含了针对窗体的 P/Inoke 逻辑
/// </summary>
internal class WindowManager
{
    private const int MinWidth = 848;
    private const int MinHeight = 524;
    private const int SubclassId = 101;

    private static readonly Windows.UI.Color SystemBaseLowColor = Windows.UI.Color.FromArgb(0x33, 0xFF, 0xFF, 0xFF);
    private static readonly Windows.UI.Color SystemBaseMediumLowColor = Windows.UI.Color.FromArgb(0x66, 0xFF, 0xFF, 0xFF);
    private readonly HWND handle;
    private readonly Window window;
    private readonly UIElement titleBar;
    private readonly ILogger<WindowManager> logger;

    // We have to explictly hold a reference to the SUBCLASSPROC,
    // otherwise will casuse System.ExecutionEngineException
    private SUBCLASSPROC? subClassProc;

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

        handle = (HWND)WindowNative.GetWindowHandle(window);

        InitializeWindow();
    }

    private static RECT RetriveWindowRect()
    {
        int left = LocalSetting.GetValueType<int>(SettingKeys.WindowLeft);
        int top = LocalSetting.GetValueType<int>(SettingKeys.WindowTop);
        int right = LocalSetting.GetValueType<int>(SettingKeys.WindowRight);
        int bottom = LocalSetting.GetValueType<int>(SettingKeys.WindowBottom);

        return new(left, top, right, bottom);
    }

    private static void SaveWindowRect(HWND handle)
    {
        WINDOWPLACEMENT windowPlacement = WINDOWPLACEMENT.Default;

        GetWindowPlacement(handle, ref windowPlacement);

        LocalSetting.Set(SettingKeys.WindowLeft, windowPlacement.rcNormalPosition.left);
        LocalSetting.Set(SettingKeys.WindowTop, windowPlacement.rcNormalPosition.top);
        LocalSetting.Set(SettingKeys.WindowRight, windowPlacement.rcNormalPosition.right);
        LocalSetting.Set(SettingKeys.WindowBottom, windowPlacement.rcNormalPosition.bottom);
    }

    private void InitializeWindow()
    {
        if (false && AppWindowTitleBar.IsCustomizationSupported())
        {
            AppWindow appWindow = GetAppWindow();
            AppWindowTitleBar titleBar = appWindow.TitleBar;
            titleBar.ExtendsContentIntoTitleBar = true;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonHoverBackgroundColor = SystemBaseLowColor;
            titleBar.ButtonPressedBackgroundColor = SystemBaseMediumLowColor;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // appWindow.TitleBar.SetDragRectangles();
            appWindow.Title = "胡桃";
        }
        else
        {
            window.ExtendsContentIntoTitleBar = true;
            window.SetTitleBar(titleBar);

            SetWindowText(handle, "胡桃");
        }

        window.Closed += OnWindowClosed;
        RECT rect = RetriveWindowRect();
        if (rect.Size > 0)
        {
            WINDOWPLACEMENT windowPlacement = WINDOWPLACEMENT.Create(new(-1, -1), rect, SHOW_WINDOW_CMD.SW_SHOWNORMAL);
            SetWindowPlacement(handle, in windowPlacement);
        }

        bool micaApplied = new SystemBackdrop(window).TrySetBackdrop();
        logger.LogInformation(EventIds.BackdropState, "Apply {name} : {result}", nameof(SystemBackdrop), micaApplied ? "succeed" : "failed");

        subClassProc = new(OnSubclassProcedure);
        bool subClassApplied = SetWindowSubclass(handle, subClassProc, SubclassId, 0);
        logger.LogInformation(EventIds.SubClassing, "Apply {name} : {result}", nameof(SUBCLASSPROC), subClassApplied ? "succeed" : "failed");
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        RemoveWindowSubclass(handle, subClassProc, SubclassId);
        subClassProc = null;
        SaveWindowRect(handle);
    }

    private LRESULT OnSubclassProcedure(HWND hwnd, uint uMsg, WPARAM wParam, LPARAM lParam, nuint uIdSubclass, nuint dwRefData)
    {
        switch (uMsg)
        {
            case WM_GETMINMAXINFO:
                {
                    uint dpi = GetDpiForWindow(handle);
                    float scalingFactor = dpi / 96f;
                    Win32.Unsafe.SetMinTrackSize(lParam, MinWidth * scalingFactor, MinHeight * scalingFactor);
                    break;
                }
        }

        return DefSubclassProc(hwnd, uMsg, wParam, lParam);
    }

    private AppWindow GetAppWindow()
    {
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(handle);
        return AppWindow.GetFromWindowId(windowId);
    }
}