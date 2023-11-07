// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Windowing;
using Windows.Win32.Foundation;
using WinRT.Interop;

namespace Snap.Hutao.Core.LifeCycle;

internal static class CurrentWindowReferenceExtension
{
    public static XamlRoot GetXamlRoot(this ICurrentWindowReference reference)
    {
        return reference.Window.Content.XamlRoot;
    }

    public static HWND GetWindowHandle(this ICurrentWindowReference reference)
    {
        return reference.Window is IWindowOptionsSource optionsSource
            ? optionsSource.WindowOptions.Hwnd
            : (HWND)WindowNative.GetWindowHandle(reference.Window);
    }
}
