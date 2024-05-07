// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Gdi;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.System.WinRT.Graphics.Capture;

[SuppressMessage("", "SYSLIB1096")]
[ComImport]
[Guid("3628E81B-3CAC-4C60-B7F4-23CE0E0C3356")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IGraphicsCaptureItemInterop
{
    [PreserveSig]
    unsafe HRESULT CreateForWindow(HWND window, Guid* riid, void** result);

    [PreserveSig]
    unsafe HRESULT CreateForMonitor(HMONITOR monitor, Guid* riid, void** result);
}