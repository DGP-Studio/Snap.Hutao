// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dxgi;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.System.WinRT.Xaml;

[SuppressMessage("", "SYSLIB1096")]
[ComImport]
[Guid("63AAD0B8-7C24-40FF-85A8-640D944CC325")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ISwapChainPanelNative
{
    [PreserveSig]
    unsafe HRESULT SetSwapChain(IDXGISwapChain* swapChain);
}