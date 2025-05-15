// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.UI.Xaml;

internal static class FrameworkTheming
{
    public static void SetTheme(Theme theme)
    {
        Marshal.ThrowExceptionForHR(FrameworkThemingSetTheme(theme));
    }

    [SuppressMessage("", "SYSLIB1054")]
    [DllImport("Snap.Hutao.Native.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern HRESULT FrameworkThemingSetTheme(Theme theme);
}