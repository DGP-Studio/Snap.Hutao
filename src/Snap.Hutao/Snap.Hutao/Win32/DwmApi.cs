// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dwm;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SH002")]
[SuppressMessage("", "SYSLIB1054")]
internal static class DwmApi
{
    [DllImport("dwmapi.dll", ExactSpelling = true)]
    [SupportedOSPlatform("windows6.0.6000")]
    public static unsafe extern HRESULT DwmSetWindowAttribute(HWND hwnd, uint dwAttribute, void* pvAttribute, uint cbAttribute);

    [DebuggerStepThrough]
    public static unsafe HRESULT DwmSetWindowAttribute<T>(HWND hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref readonly T attribute)
        where T : unmanaged
    {
        fixed (T* pvAttribute = &attribute)
        {
            return DwmSetWindowAttribute(hwnd, (uint)dwAttribute, &pvAttribute, (uint)sizeof(T));
        }
    }
}