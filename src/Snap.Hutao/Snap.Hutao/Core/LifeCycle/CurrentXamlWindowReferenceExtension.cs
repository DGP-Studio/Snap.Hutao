// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.Win32.Foundation;
using WinRT;

namespace Snap.Hutao.Core.LifeCycle;

internal static class CurrentXamlWindowReferenceExtension
{
    public static XamlRoot? GetXamlRoot<TWindow>(this ICurrentXamlWindowReference<TWindow> reference)
        where TWindow : Window
    {
        return reference.Window?.Content?.XamlRoot;
    }

    public static HWND GetWindowHandle<TWindow>(this ICurrentXamlWindowReference<TWindow> reference)
        where TWindow : Window
    {
        return reference.Window.GetWindowHandle();
    }

    public static ElementTheme GetRequestedTheme<TWindow>(this ICurrentXamlWindowReference<TWindow> reference)
        where TWindow : Window
    {
        TWindow? window = reference.Window;
        ArgumentNullException.ThrowIfNull(window);
        return window.Content.As<FrameworkElement>().RequestedTheme;
    }
}