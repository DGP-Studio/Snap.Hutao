// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Win32;
using System.Runtime.CompilerServices;
using Windows.Graphics;
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
    /// <typeparam name="TWindow">窗体类型</typeparam>
    /// <param name="window">选项窗口</param>
    public static void RecoverOrInit<TWindow>(TWindow window)
        where TWindow : Window, IWindowOptionsSource
    {
        WindowOptions options = window.WindowOptions;

        // Set first launch size
        double scale = options.GetWindowScale();
        SizeInt32 transformedSize = options.InitSize.Scale(scale);
        RectInt32 rect = StructMarshal.RectInt32(transformedSize);

        if (options.PersistSize)
        {
            RectInt32 persistedRect = (CompactRect)LocalSetting.Get(SettingKeys.WindowRect, (CompactRect)rect);
            if (persistedRect.Size() >= options.InitSize.Size())
            {
                rect = persistedRect;
            }
        }

        TransformToCenterScreen(ref rect);
        window.AppWindow.MoveAndResize(rect);
    }

    /// <summary>
    /// 保存窗体的位置
    /// </summary>
    /// <param name="window">窗口</param>
    /// <typeparam name="TWindow">窗体类型</typeparam>
    public static void Save<TWindow>(TWindow window)
        where TWindow : Window, IWindowOptionsSource
    {
        WINDOWPLACEMENT windowPlacement = StructMarshal.WINDOWPLACEMENT();
        GetWindowPlacement(window.WindowOptions.Hwnd, ref windowPlacement);

        // prevent save value when we are maximized.
        if (!windowPlacement.showCmd.HasFlag(SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED))
        {
            LocalSetting.Set(SettingKeys.WindowRect, (CompactRect)window.AppWindow.GetRect());
        }
    }

    private static void TransformToCenterScreen(ref RectInt32 rect)
    {
        DisplayArea displayArea = DisplayArea.GetFromRect(rect, DisplayAreaFallback.Primary);
        RectInt32 workAreaRect = displayArea.WorkArea;

        rect.X = workAreaRect.X + ((workAreaRect.Width - rect.Width) / 2);
        rect.Y = workAreaRect.Y + ((workAreaRect.Height - rect.Height) / 2);
    }

    private readonly struct CompactRect
    {
        private readonly short x;
        private readonly short y;
        private readonly short width;
        private readonly short height;

        private CompactRect(int x, int y, int width, int height)
        {
            this.x = (short)x;
            this.y = (short)y;
            this.width = (short)width;
            this.height = (short)height;
        }

        public static implicit operator RectInt32(CompactRect rect)
        {
            return new(rect.x, rect.y, rect.width, rect.height);
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