// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using WinRT.Interop;

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

    public static bool TryGetAssociatedServiceProvider(this Window window, out IServiceProvider serviceProvider)
    {
        if (WindowControllers.TryGetValue(window, out XamlWindowController? controller))
        {
            serviceProvider = controller.ServiceProvider;
            return true;
        }

        serviceProvider = default!;
        return false;
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

    public static HWND GetWindowHandle(this Window? window)
    {
        return WindowNative.GetWindowHandle(window);
    }

    public static void SwitchTo(this Window window)
    {
        SwitchTo(window.GetWindowHandle());
    }

    public static void SwitchTo(HWND hwnd)
    {
        WindowUtilities.SwitchToWindow(hwnd);
    }

    public static void AddExtendedStyleLayered(this Window window)
    {
        WindowUtilities.AddExtendedStyleLayered(window.GetWindowHandle());
    }

    public static void RemoveExtendedStyleLayered(this Window window)
    {
        WindowUtilities.RemoveExtendedStyleLayered(window.GetWindowHandle());
    }

    public static void SetLayeredWindowTransparency(this Window window, byte alpha)
    {
        WindowUtilities.SetLayeredWindowTransparency(window.GetWindowHandle(), alpha);
    }

    public static void AddExtendedStyleToolWindow(this Window window)
    {
        WindowUtilities.AddExtendedStyleToolWindow(window.GetWindowHandle());
    }

    public static void RemoveStyleOverlappedWindow(this Window window)
    {
        WindowUtilities.RemoveStyleOverlappedWindow(window.GetWindowHandle());
    }

    public static double GetRasterizationScale(this Window window)
    {
        return window is { Content.XamlRoot: { } xamlRoot }
            ? xamlRoot.RasterizationScale
            : WindowUtilities.GetRasterizationScaleForWindow(window.GetWindowHandle());
    }
}