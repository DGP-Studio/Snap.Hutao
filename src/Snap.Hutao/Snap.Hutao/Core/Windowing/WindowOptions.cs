// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Win32.Foundation;
using WinRT.Interop;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// Window 选项
/// </summary>
/// <typeparam name="TWindow">窗体类型</typeparam>
internal readonly struct WindowOptions<TWindow>
    where TWindow : Window, IExtendedWindowSource
{
    /// <summary>
    /// 窗体句柄
    /// </summary>
    public readonly HWND Hwnd;

    /// <summary>
    /// AppWindow
    /// </summary>
    public readonly AppWindow AppWindow;

    /// <summary>
    /// 窗体
    /// </summary>
    public readonly TWindow Window;

    /// <summary>
    /// 标题栏元素
    /// </summary>
    public readonly FrameworkElement TitleBar;

    /// <summary>
    /// 是否使用 Win UI 3 自带的拓展标题栏实现
    /// </summary>
    public readonly bool UseLegacyDragBarImplementation = !AppWindowTitleBar.IsCustomizationSupported();

    public WindowOptions(TWindow window, FrameworkElement titleBar)
    {
        Window = window;
        Hwnd = (HWND)WindowNative.GetWindowHandle(window);
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(Hwnd);
        AppWindow = AppWindow.GetFromWindowId(windowId);

        TitleBar = titleBar;
    }
}