// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Content;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.UI.Xaml.Media.Backdrop;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Runtime.CompilerServices;
using WinRT.Interop;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Xaml;

internal static class WindowExtension
{
    private static readonly ConditionalWeakTable<Window, XamlWindowController> WindowControllers = [];

    public static void InitializeController<TWindow>(this TWindow window, IServiceProvider serviceProvider)
        where TWindow : Window
    {
        XamlWindowController windowController = new(window, serviceProvider);
        WindowControllers.Add(window, windowController);
    }

    public static bool IsControllerInitialized<TWindow>()
        where TWindow : Window
    {
        foreach ((Window window, _) in WindowControllers)
        {
            if (window is TWindow)
            {
                return true;
            }
        }

        return false;
    }

    public static void UninitializeController<TWindow>(this TWindow window)
        where TWindow : Window
    {
        WindowControllers.Remove(window);
    }

    public static XamlWindowController? GetController<TWindow>(this TWindow window)
        where TWindow : Window
    {
        WindowControllers.TryGetValue(window, out XamlWindowController? xamlWindowController);
        return xamlWindowController;
    }

    [SuppressMessage("", "SH007")]
    public static DesktopWindowXamlSource GetDesktopWindowXamlSource(this Window window)
    {
        if (window.SystemBackdrop is SystemBackdropDesktopWindowXamlSourceAccess access)
        {
            return access.DesktopWindowXamlSource!;
        }

        return default!;
    }

    public static InputNonClientPointerSource GetInputNonClientPointerSource(this Window window)
    {
        return InputNonClientPointerSource.GetForWindowId(window.AppWindow.Id);
    }

    public static InputPointerSource GetInputPointerSource(this Window window)
    {
        InputPointerSource inputPointerSource = default!;
        ContentIsland[] contentIslands = ContentIsland.FindAllForCurrentThread();
        foreach (ref readonly ContentIsland island in contentIslands.AsSpan())
        {
            if (island.Environment.AppWindowId == window.AppWindow.Id)
            {
                inputPointerSource = InputPointerSource.GetForIsland(island);
                break;
            }
        }

        return inputPointerSource;
    }

    public static HWND GetWindowHandle(this Window? window)
    {
        return WindowNative.GetWindowHandle(window);
    }

    public static void Show(this Window window)
    {
        ShowWindow(window.GetWindowHandle(), SHOW_WINDOW_CMD.SW_NORMAL);
    }

    public static void SwitchTo(this Window window)
    {
        SwitchTo(window.GetWindowHandle());
    }

    public static void SwitchTo(HWND hwnd)
    {
        if (!IsWindowVisible(hwnd))
        {
            ShowWindow(hwnd, SHOW_WINDOW_CMD.SW_SHOW);
        }

        if (IsIconic(hwnd))
        {
            ShowWindow(hwnd, SHOW_WINDOW_CMD.SW_RESTORE);
        }

        SetForegroundWindow(hwnd);
    }

    public static void Hide(this Window window)
    {
        ShowWindow(window.GetWindowHandle(), SHOW_WINDOW_CMD.SW_HIDE);
    }

    public static void AddExStyleLayered(this Window window)
    {
        HWND hwnd = WindowNative.GetWindowHandle(window);
        nint style = GetWindowLongPtrW(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        style |= (nint)WINDOW_EX_STYLE.WS_EX_LAYERED;
        SetWindowLongPtrW(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, style);
    }

    public static void RemoveExStyleLayered(this Window window)
    {
        HWND hwnd = WindowNative.GetWindowHandle(window);
        nint style = GetWindowLongPtrW(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        style &= ~(nint)WINDOW_EX_STYLE.WS_EX_LAYERED;
        SetWindowLongPtrW(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, style);
    }

    public static void AddExStyleToolWindow(this Window window)
    {
        HWND hwnd = WindowNative.GetWindowHandle(window);
        nint style = GetWindowLongPtrW(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        style |= (nint)WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;
        SetWindowLongPtrW(hwnd, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, style);
    }

    public static unsafe void BringToForeground(this Window window)
    {
        HWND fgHwnd = GetForegroundWindow();
        HWND hwnd = window.GetWindowHandle();

        uint threadIdHwnd = GetWindowThreadProcessId(hwnd, default);
        uint threadIdFgHwnd = GetWindowThreadProcessId(fgHwnd, default);

        if (threadIdHwnd != threadIdFgHwnd)
        {
            AttachThreadInput(threadIdHwnd, threadIdFgHwnd, true);
            SetForegroundWindow(hwnd);
            AttachThreadInput(threadIdHwnd, threadIdFgHwnd, false);
        }
        else
        {
            SetForegroundWindow(hwnd);
        }
    }

    public static double GetRasterizationScale(this Window window)
    {
        if (window is { Content.XamlRoot: { } xamlRoot })
        {
            return xamlRoot.RasterizationScale;
        }

        uint dpi = GetDpiForWindow(window.GetWindowHandle());
        return Math.Round(dpi / 96D, 2, MidpointRounding.AwayFromZero);
    }
}