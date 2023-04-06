// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Win32;
using System.Runtime.CompilerServices;
using Windows.Graphics;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 窗体持久化
/// </summary>
[HighQuality]
internal static class Persistence
{
    /// <summary>
    /// 设置窗体位置
    /// </summary>
    /// <param name="options">选项</param>
    /// <typeparam name="TWindow">窗体类型</typeparam>
    public static void RecoverOrInit<TWindow>(WindowOptions<TWindow> options)
        where TWindow : Window, IExtendedWindowSource
    {
        // Set first launch size.
        double scale = GetScaleForWindowHandle(options.Hwnd);
        SizeInt32 transformedSize = options.Window.InitSize.Scale(scale);
        RectInt32 rect = StructMarshal.RectInt32(transformedSize);

        if (options.Window.PersistSize)
        {
            RectInt32 persistedRect = (CompactRect)LocalSetting.Get(SettingKeys.WindowRect, (ulong)(CompactRect)rect);
            if (persistedRect.Size() >= options.Window.InitSize.Size())
            {
                rect = persistedRect;
            }
        }

        TransformToCenterScreen(ref rect);
        options.AppWindow.MoveAndResize(rect);
    }

    /// <summary>
    /// 保存窗体的位置
    /// </summary>
    /// <param name="options">选项</param>
    /// <typeparam name="TWindow">窗体类型</typeparam>
    public static void Save<TWindow>(WindowOptions<TWindow> options)
        where TWindow : Window, IExtendedWindowSource
    {
        WINDOWPLACEMENT windowPlacement = StructMarshal.WINDOWPLACEMENT();
        GetWindowPlacement(options.Hwnd, ref windowPlacement);

        // prevent save value when we are maximized.
        if (!windowPlacement.showCmd.HasFlag(SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED))
        {
            LocalSetting.Set(SettingKeys.WindowRect, (CompactRect)options.AppWindow.GetRect());
        }
    }

    /// <summary>
    /// 获取窗体当前的DPI缩放比
    /// </summary>
    /// <param name="hwnd">窗体句柄</param>
    /// <returns>缩放比</returns>
    public static double GetScaleForWindowHandle(HWND hwnd)
    {
        uint dpi = GetDpiForWindow(hwnd);
        return Math.Round(dpi / 96D, 2, MidpointRounding.AwayFromZero);
    }

    private static void TransformToCenterScreen(ref RectInt32 rect)
    {
        DisplayArea displayArea = DisplayArea.GetFromRect(rect, DisplayAreaFallback.Primary);
        RectInt32 workAreaRect = displayArea.WorkArea;

        rect.X = workAreaRect.X + ((workAreaRect.Width - rect.Width) / 2);
        rect.Y = workAreaRect.Y + ((workAreaRect.Height - rect.Height) / 2);
    }

    private struct CompactRect
    {
        public short X;
        public short Y;
        public short Width;
        public short Height;

        private CompactRect(int x, int y, int width, int height)
        {
            X = (short)x;
            Y = (short)y;
            Width = (short)width;
            Height = (short)height;
        }

        public static implicit operator RectInt32(CompactRect rect)
        {
            return new(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static explicit operator CompactRect(RectInt32 rect)
        {
            return new(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static unsafe explicit operator CompactRect(ulong value)
        {
            Unsafe.SkipInit(out CompactRect rect);
            *(ulong*)&rect = value;
            return rect;
        }

        public static unsafe implicit operator ulong(CompactRect rect)
        {
            return *(ulong*)&rect;
        }
    }
}