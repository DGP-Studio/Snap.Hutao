// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using Windows.Win32.Foundation;
using WinRT.Interop;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// Window 选项
/// </summary>
/// <typeparam name="TWindow">窗体类型</typeparam>
internal readonly struct WindowOptions
{
    /// <summary>
    /// 窗体句柄
    /// </summary>
    public readonly HWND Hwnd;

    /// <summary>
    /// 标题栏元素
    /// </summary>
    public readonly FrameworkElement TitleBar;

    /// <summary>
    /// 初始大小
    /// </summary>
    public readonly SizeInt32 InitSize;

    /// <summary>
    /// 是否持久化尺寸
    /// </summary>
    public readonly bool PersistSize;

    /// <summary>
    /// 是否使用 Win UI 3 自带的拓展标题栏实现
    /// </summary>
    public readonly bool UseLegacyDragBarImplementation = !AppWindowTitleBar.IsCustomizationSupported();

    /// <summary>
    /// 构造一个新的窗体选项
    /// </summary>
    /// <param name="window">窗体</param>
    /// <param name="titleBar">标题栏</param>
    /// <param name="initSize">初始尺寸</param>
    /// <param name="persistSize">持久化尺寸</param>
    public WindowOptions(Window window, FrameworkElement titleBar, SizeInt32 initSize, bool persistSize = false)
    {
        Hwnd = (HWND)WindowNative.GetWindowHandle(window);
        TitleBar = titleBar;
        InitSize = initSize;
        PersistSize = persistSize;
    }
}