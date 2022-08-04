// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Logging;
using System.Collections.Generic;
using Windows.Graphics;
using Windows.UI;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Shell;
using WinRT.Interop;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 窗口管理器
/// 主要包含了针对窗体的 P/Inoke 逻辑
/// </summary>
internal sealed class WindowManager : IDisposable
{
    private const int MinWidth = 848;
    private const int MinHeight = 524;
    private const int SubclassId = 101;

    private readonly HWND handle;
    private readonly Window window;
    private readonly FrameworkElement titleBar;
    private readonly ILogger<WindowManager> logger;
    private readonly WindowSubclassManager subclassManager;

    /// <summary>
    /// 构造一个新的窗口状态管理器
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="titleBar">充当标题栏的元素</param>
    public WindowManager(Window window, FrameworkElement titleBar)
    {
        this.window = window;
        this.titleBar = titleBar;
        logger = Ioc.Default.GetRequiredService<ILogger<WindowManager>>();

        handle = (HWND)WindowNative.GetWindowHandle(window);
        subclassManager = new(handle);

        InitializeWindow();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        WindowRect.SaveWindowRect(handle);
        subclassManager.Dispose();
    }

    private void InitializeWindow()
    {
        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            AppWindow appWindow = Interop.GetAppWindow(handle);
            appWindow.Title = "胡桃";

            AppWindowTitleBar appTitleBar = appWindow.TitleBar;
            appTitleBar.ExtendsContentIntoTitleBar = true;

            UpdateTitleButtonColor(appTitleBar);
            UpdateDragRectangles(appTitleBar);
            titleBar.SizeChanged += (s, e) => UpdateDragRectangles(appTitleBar);
            titleBar.ActualThemeChanged += (s, e) => UpdateTitleButtonColor(appTitleBar);

            appWindow.Show(true);
        }
        else
        {
            SetWindowText(handle, "胡桃");
            window.ExtendsContentIntoTitleBar = true;
            window.SetTitleBar(titleBar);

            window.Activate();
        }

        Interop.SetWindowPosition(handle);

        bool micaApplied = new SystemBackdrop(window).TrySetBackdrop();
        logger.LogInformation(EventIds.BackdropState, "Apply {name} : {result}", nameof(SystemBackdrop), micaApplied ? "succeed" : "failed");

        bool subClassApplied = subclassManager.TrySetWindowSubclass();
        logger.LogInformation(EventIds.SubClassing, "Apply {name} : {result}", nameof(SUBCLASSPROC), subClassApplied ? "succeed" : "failed");
    }

    private void UpdateDragRectangles(AppWindowTitleBar appTitleBar)
    {
        uint dpi = GetDpiForWindow(handle);

        // double scaleAdjustment = (uint)((((long)dpi * 100) + (96 >> 1)) / 96) / 100.0;
        double scale = Math.Round(dpi / 96d, 2, MidpointRounding.AwayFromZero);

        List<RectInt32> dragRectsList = new();

        // 48 is the navigation button leftInset
        RectInt32 dragRect = new((int)(48 * scale), 0, (int)(titleBar.ActualWidth * scale), (int)(titleBar.ActualHeight * scale));
        dragRectsList.Add(dragRect);

        RectInt32[] dragRects = dragRectsList.ToArray();

        appTitleBar.SetDragRectangles(dragRects);
    }

    [SuppressMessage("", "CA1822")]
    private void UpdateTitleButtonColor(AppWindowTitleBar appTitleBar)
    {
        appTitleBar.ButtonBackgroundColor = Colors.Transparent;
        appTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        Color systemBaseLowColor = (Color)App.Current.Resources["SystemBaseLowColor"];
        appTitleBar.ButtonHoverBackgroundColor = systemBaseLowColor;

        Color systemBaseMediumLowColor = (Color)App.Current.Resources["SystemBaseMediumLowColor"];
        appTitleBar.ButtonPressedBackgroundColor = systemBaseMediumLowColor;

        // The Foreground doesn't accept Alpha channel. So we translate it to gray.
        byte light = (byte)((systemBaseMediumLowColor.R + systemBaseMediumLowColor.G + systemBaseMediumLowColor.B) / 3);
        byte result = (byte)((systemBaseMediumLowColor.A / 255.0) * light);
        appTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(0XFF, result, result, result);

        Color systemBaseHighColor = (Color)App.Current.Resources["SystemBaseHighColor"];
        appTitleBar.ButtonForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonHoverForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonPressedForegroundColor = systemBaseHighColor;
    }
}
