// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.Win32.Foundation;
using WinRT;

namespace Snap.Hutao.Core.LifeCycle;

internal static class CurrentXamlWindowReferenceExtension
{
    public static XamlRoot? GetXamlRoot(this ICurrentXamlWindowReference reference)
    {
        return reference.Window?.Content?.XamlRoot;
    }

    public static HWND GetWindowHandle(this ICurrentXamlWindowReference reference)
    {
        return reference.Window.GetWindowHandle();
    }

    public static ElementTheme GetRequestedTheme(this ICurrentXamlWindowReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference.Window);
        return reference.Window.Content.As<FrameworkElement>().RequestedTheme;
    }
}