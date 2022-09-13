// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Logging;
using Windows.Graphics;
using Windows.UI;
using Windows.Win32.Foundation;
using WinRT.Interop;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 窗口管理器
/// 主要包含了针对窗体的 P/Inoke 逻辑
/// </summary>
internal sealed class WindowManager : IDisposable
{
    private readonly HWND handle;
    private readonly AppWindow appWindow;

    private readonly Window window;
    private readonly FrameworkElement titleBar;

    private readonly ILogger<WindowManager> logger;
    private readonly WindowSubclassManager subclassManager;

    private readonly bool useLegacyDragBar;

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

        WindowId windowId = Win32Interop.GetWindowIdFromWindow(handle);
        appWindow = AppWindow.GetFromWindowId(windowId);

        useLegacyDragBar = !AppWindowTitleBar.IsCustomizationSupported();
        subclassManager = new(handle, useLegacyDragBar);

        InitializeWindow();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Persistence.Save(appWindow);
        subclassManager?.Dispose();
    }

    private static void UpdateTitleButtonColor(AppWindowTitleBar appTitleBar)
    {
        appTitleBar.ButtonBackgroundColor = Colors.Transparent;
        appTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        App app = Ioc.Default.GetRequiredService<App>();

        Color systemBaseLowColor = (Color)app.Resources["SystemBaseLowColor"];
        appTitleBar.ButtonHoverBackgroundColor = systemBaseLowColor;

        Color systemBaseMediumLowColor = (Color)app.Resources["SystemBaseMediumLowColor"];
        appTitleBar.ButtonPressedBackgroundColor = systemBaseMediumLowColor;

        // The Foreground doesn't accept Alpha channel. So we translate it to gray.
        byte light = (byte)((systemBaseMediumLowColor.R + systemBaseMediumLowColor.G + systemBaseMediumLowColor.B) / 3);
        byte result = (byte)((systemBaseMediumLowColor.A / 255.0) * light);
        appTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(0xFF, result, result, result);

        Color systemBaseHighColor = (Color)app.Resources["SystemBaseHighColor"];
        appTitleBar.ButtonForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonHoverForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonPressedForegroundColor = systemBaseHighColor;
    }

    private static (string PosString, string SizeString) GetPostionAndSize(AppWindow appWindow)
    {
        PointInt32 pos = appWindow.Position;
        string posString = $"{pos.X},{pos.Y}";
        SizeInt32 size = appWindow.Size;
        string sizeString = $"{size.Width},{size.Height}";

        return (posString, sizeString);
    }

    private void InitializeWindow()
    {
        appWindow.Title = "胡桃";

        ExtendsContentIntoTitleBar();
        Persistence.RecoverOrInit(appWindow);

        // Log basic window state here.
        (string pos, string size) = GetPostionAndSize(appWindow);
        logger.LogInformation(EventIds.WindowState, "Postion: [{pos}], Size: [{size}]", pos, size);

        appWindow.Show(true);

        bool micaApplied = new SystemBackdrop(window).TrySetBackdrop();
        logger.LogInformation(EventIds.BackdropState, "Apply {name} : {result}", nameof(SystemBackdrop), micaApplied ? "succeed" : "failed");

        bool subClassApplied = subclassManager.TrySetWindowSubclass();
        logger.LogInformation(EventIds.SubClassing, "Apply {name} : {result}", nameof(WindowSubclassManager), subClassApplied ? "succeed" : "failed");
    }

    private void ExtendsContentIntoTitleBar()
    {
        if (useLegacyDragBar)
        {
            // use normal Window method to extend.
            window.ExtendsContentIntoTitleBar = true;
            window.SetTitleBar(titleBar);
        }
        else
        {
            AppWindowTitleBar appTitleBar = appWindow.TitleBar;
            appTitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            appTitleBar.ExtendsContentIntoTitleBar = true;

            UpdateTitleButtonColor(appTitleBar);
            UpdateDragRectangles(appTitleBar);
            titleBar.SizeChanged += (s, e) => UpdateDragRectangles(appTitleBar);
            titleBar.ActualThemeChanged += (s, e) => UpdateTitleButtonColor(appTitleBar);
        }
    }

    private void UpdateDragRectangles(AppWindowTitleBar appTitleBar)
    {
        double scale = Persistence.GetScaleForWindow(handle);

        List<RectInt32> dragRectsList = new();

        // 48 is the navigation button leftInset
        RectInt32 dragRect = new((int)(48 * scale), 0, (int)(titleBar.ActualWidth * scale), (int)(titleBar.ActualHeight * scale));
        dragRectsList.Add(dragRect);

        RectInt32[] dragRects = dragRectsList.ToArray();

        appTitleBar.SetDragRectangles(dragRects);
    }
}
