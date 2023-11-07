// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using Windows.Win32.Foundation;
using WinRT.Interop;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// Window 选项
/// </summary>
internal readonly struct WindowOptions
{
    /// <summary>
    /// 窗体句柄
    /// </summary>
    public readonly HWND Hwnd;

    /// <summary>
    /// 非客户端区域指针源
    /// </summary>
    public readonly InputNonClientPointerSource InputNonClientPointerSource;

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
        InputNonClientPointerSource = InputNonClientPointerSource.GetForWindowId(window.AppWindow.Id);
        TitleBar = titleBar;
        InitSize = initSize;
        PersistSize = persistSize;
    }

    /// <summary>
    /// 获取窗体当前的DPI缩放比
    /// </summary>
    /// <returns>缩放比</returns>
    public double GetRasterizationScale()
    {
        uint dpi = GetDpiForWindow(Hwnd);
        return Math.Round(dpi / 96D, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// 将窗口设为前台窗口
    /// </summary>
    /// <param name="hwnd">窗口句柄</param>
    public unsafe void BringToForeground()
    {
        HWND fgHwnd = GetForegroundWindow();

        uint threadIdHwnd = GetWindowThreadProcessId(Hwnd);
        uint threadIdFgHwnd = GetWindowThreadProcessId(fgHwnd);

        if (threadIdHwnd != threadIdFgHwnd)
        {
            AttachThreadInput(threadIdHwnd, threadIdFgHwnd, true);
            SetForegroundWindow(Hwnd);
            AttachThreadInput(threadIdHwnd, threadIdFgHwnd, false);
        }
        else
        {
            SetForegroundWindow(Hwnd);
        }
    }
}