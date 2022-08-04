// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using System.Numerics;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// Win32/COM 窗体互操作
/// </summary>
internal static class Interop
{
    /// <summary>
    /// 获取 <see cref="AppWindow"/>
    /// </summary>
    /// <param name="hwnd">窗体句柄</param>
    /// <returns>AppWindow</returns>
    public static AppWindow GetAppWindow(HWND hwnd)
    {
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        return AppWindow.GetFromWindowId(windowId);
    }

    /// <summary>
    /// 设置窗体位置
    /// </summary>
    /// <param name="hwnd">窗体句柄</param>
    public static void SetWindowPosition(HWND hwnd)
    {
        if (WindowRect.RetriveWindowRect() is RECT { Size: > 0 } retrived)
        {
            WINDOWPLACEMENT windowPlacement = WINDOWPLACEMENT.Create(new(-1, -1), retrived, SHOW_WINDOW_CMD.SW_SHOWNORMAL);
            SetWindowPlacement(hwnd, in windowPlacement);
        }
        else
        {
            // Set first launch size.
            Vector2 size = Interop.TransformSizeForWindow(new(1200, 741), hwnd);
            SET_WINDOW_POS_FLAGS first = SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER;
            SetWindowPos(hwnd, default, 0, 0, (int)size.X, (int)size.Y, first);

            // Make it centralized
            GetWindowRect(hwnd, out RECT rect);
            Interop.TransformToCenterScreen(ref rect);

            SET_WINDOW_POS_FLAGS center = SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE;
            SetWindowPos(hwnd, default, rect.left, rect.top, 0, 0, center);
        }
    }

    /// <summary>
    /// 转换到当前窗体的DPI的尺寸
    /// </summary>
    /// <param name="size">尺寸</param>
    /// <param name="hwnd">窗体句柄</param>
    /// <returns>对应当前窗体DPI的尺寸</returns>
    public static Vector2 TransformSizeForWindow(Vector2 size, HWND hwnd)
    {
        uint dpi = GetDpiForWindow(hwnd);
        float scale = (float)dpi / 96;
        return new(size.X * scale, size.Y * scale);
    }

    /// <summary>
    /// 将矩形转换到屏幕中央
    /// 当宽高超过时，会裁剪
    /// </summary>
    /// <param name="rect">矩形</param>
    public static void TransformToCenterScreen(ref RECT rect)
    {
        HMONITOR hMonitor = MonitorFromRect(rect, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
        MONITORINFO mi = MONITORINFO.Default;
        GetMonitorInfo(hMonitor, ref mi);

        RECT workAreaRect = mi.rcWork;

        int width = rect.right - rect.left;
        int height = rect.bottom - rect.top;

        rect.left = workAreaRect.left + ((workAreaRect.right - workAreaRect.left - width) / 2);
        rect.top = workAreaRect.top + ((workAreaRect.bottom - workAreaRect.top - height) / 2);
        rect.right = rect.left + width;
        rect.bottom = rect.top + height;
    }
}