// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Runtime.CompilerServices;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using WinRT.Interop;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.Windowing;

internal static class WindowExtension
{
    private static readonly ConditionalWeakTable<Window, WindowController> WindowControllers = [];

    public static void InitializeController<TWindow>(this TWindow window, IServiceProvider serviceProvider)
        where TWindow : Window, IWindowOptionsSource
    {
        WindowController windowController = new(window, window.WindowOptions, serviceProvider);
        WindowControllers.Add(window, windowController);
    }

    public static void SetLayeredWindow(this Window window)
    {
        HWND hwnd = (HWND)WindowNative.GetWindowHandle(window);
        nint style = GetWindowLongPtr(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        style |= (nint)WINDOW_EX_STYLE.WS_EX_LAYERED;
        SetWindowLongPtr(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, style);
    }
}