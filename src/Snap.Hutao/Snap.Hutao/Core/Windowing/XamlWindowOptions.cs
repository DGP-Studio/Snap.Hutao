// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Snap.Hutao.Win32.Foundation;
using Windows.Graphics;
using WinRT.Interop;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// Window 选项
/// </summary>
internal readonly struct XamlWindowOptions
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
    [Obsolete]
    public readonly bool PersistSize;

    public readonly string? PersistRectKey;

    public XamlWindowOptions(Window window, FrameworkElement titleBar, SizeInt32 initSize, string? persistSize = default)
    {
        Hwnd = WindowNative.GetWindowHandle(window);
        InputNonClientPointerSource = InputNonClientPointerSource.GetForWindowId(window.AppWindow.Id);
        TitleBar = titleBar;
        InitSize = initSize;
        PersistRectKey = persistSize;
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

        uint threadIdHwnd = GetWindowThreadProcessId(Hwnd, default);
        uint threadIdFgHwnd = GetWindowThreadProcessId(fgHwnd, default);

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