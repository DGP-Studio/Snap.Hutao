// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Shell;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

// [SuppressMessage("", "SH002")]
[SuppressMessage("", "SYSLIB1054")]
internal static class ComCtl32
{
    [DllImport("COMCTL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern LRESULT DefSubclassProc(HWND hWnd, uint uMsg, WPARAM wParam, LPARAM lParam);

    [DllImport("COMCTL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern BOOL RemoveWindowSubclass(HWND hWnd, SUBCLASSPROC pfnSubclass, nuint uIdSubclass);

    [DllImport("COMCTL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static unsafe extern BOOL SetWindowSubclass(HWND hWnd, SUBCLASSPROC pfnSubclass, nuint uIdSubclass, nuint dwRefData);
}