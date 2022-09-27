// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Win32;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.Win32.Foundation;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

/// <summary>
/// 窗体持久化
/// </summary>
internal static class Persistence
{
    /// <summary>
    /// 设置窗体位置
    /// </summary>
    /// <param name="appWindow">应用窗体</param>
    public static void RecoverOrInit(AppWindow appWindow)
    {
        // Set first launch size.
        HWND hwnd = (HWND)Win32Interop.GetWindowFromWindowId(appWindow.Id);
        SizeInt32 size = TransformSizeForWindow(new(1200, 741), hwnd);
        RectInt32 rect = StructMarshal.RectInt32(size);

        RectInt32 target = (CompactRect)LocalSetting.Get(SettingKeys.WindowRect, (ulong)(CompactRect)rect);
        //if(target.Width*target.Height)
        TransformToCenterScreen(ref target);
        appWindow.MoveAndResize(target);
    }

    /// <summary>
    /// 保存状态的位置
    /// </summary>
    /// <param name="appWindow">应用窗体</param>
    public static void Save(AppWindow appWindow)
    {
        LocalSetting.Set(SettingKeys.WindowRect, (ulong)(CompactRect)appWindow.GetRect());
    }

    /// <summary>
    /// 获取窗体当前的DPI缩放比
    /// </summary>
    /// <param name="hwnd">窗体句柄</param>
    /// <returns>缩放比</returns>
    public static double GetScaleForWindow(HWND hwnd)
    {
        uint dpi = GetDpiForWindow(hwnd);
        return Math.Round(dpi / 96d, 2, MidpointRounding.AwayFromZero);
    }

    private static SizeInt32 TransformSizeForWindow(SizeInt32 size, HWND hwnd)
    {
        double scale = GetScaleForWindow(hwnd);
        return new((int)(size.Width * scale), (int)(size.Height * scale));
    }

    private static void TransformToCenterScreen(ref RectInt32 rect)
    {
        DisplayArea displayArea = DisplayArea.GetFromRect(rect, DisplayAreaFallback.Primary);
        RectInt32 workAreaRect = displayArea.WorkArea;

        rect.X = workAreaRect.X + ((workAreaRect.Width - rect.Width) / 2);
        rect.Y = workAreaRect.Y + ((workAreaRect.Height - rect.Height) / 2);
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct CompactRect
    {
        [FieldOffset(0)]
        public short X;

        [FieldOffset(2)]
        public short Y;

        [FieldOffset(4)]
        public short Width;

        [FieldOffset(6)]
        public short Height;

        [FieldOffset(0)]
        public ulong Value;

        private CompactRect(int x, int y, int width, int height)
        {
            Value = 0;
            X = (short)x;
            Y = (short)y;
            Width = (short)width;
            Height = (short)height;
        }

        private CompactRect(ulong value)
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
            Value = value;
        }

        public static implicit operator RectInt32(CompactRect rect)
        {
            return new(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static explicit operator CompactRect(ulong value)
        {
            return new(value);
        }

        public static explicit operator CompactRect(RectInt32 rect)
        {
            return new(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static explicit operator ulong(CompactRect rect)
        {
            return rect.Value;
        }
    }
}