// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

internal static class Dxgi
{
    [DllImport("dxgi.dll", ExactSpelling = true, PreserveSig = false)]
    [SupportedOSPlatform("windows8.1")]
    public unsafe static extern HRESULT CreateDXGIFactory2([In] uint Flags, [In][Const] Guid* riid, [Out][ComOutPtr] void** ppFactory);
}