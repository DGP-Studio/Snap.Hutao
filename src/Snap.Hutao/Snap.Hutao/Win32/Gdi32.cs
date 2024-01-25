// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Graphics.Gdi;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SH002")]
[SuppressMessage("", "SYSLIB1054")]
internal static class Gdi32
{
    [DllImport("GDI32.dll", ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern int GetDeviceCaps([AllowNull] HDC hdc, int index);

    [DebuggerStepThrough]
    public static int GetDeviceCaps([AllowNull] HDC hdc, GET_DEVICE_CAPS_INDEX index)
    {
        return GetDeviceCaps(hdc, (int)index);
    }
}