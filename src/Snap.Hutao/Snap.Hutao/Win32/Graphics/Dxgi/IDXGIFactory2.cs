// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

[SupportedOSPlatform("windows8.0")]
internal static unsafe class IDXGIFactory2
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x1C, 0x3A, 0xC8, 0x50, 0x72, 0xE0, 0x48, 0x4C, 0x87, 0xB0, 0x36, 0x30, 0xFA, 0x36, 0xA6, 0xD0]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIFactory1.Vftbl IDXGIFactory1Vftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL> IsWindowedStereoEnabled;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HWND, DXGI_SWAP_CHAIN_DESC1*, DXGI_SWAP_CHAIN_FULLSCREEN_DESC*, nint, nint*, HRESULT> CreateSwapChainForHwnd;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, nint, DXGI_SWAP_CHAIN_DESC1*, nint, nint*, HRESULT> CreateSwapChainForCoreWindow;
        internal readonly delegate* unmanaged[Stdcall]<nint, HANDLE, LUID*, HRESULT> GetSharedResourceAdapterLuid;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, uint, uint*, HRESULT> RegisterStereoStatusWindow;
        internal readonly delegate* unmanaged[Stdcall]<nint, HANDLE, uint*, HRESULT> RegisterStereoStatusEvent;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, void> UnregisterStereoStatus;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, uint, uint*, HRESULT> RegisterOcclusionStatusWindow;
        internal readonly delegate* unmanaged[Stdcall]<nint, HANDLE, uint*, HRESULT> RegisterOcclusionStatusEvent;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, void> UnregisterOcclusionStatus;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, DXGI_SWAP_CHAIN_DESC1*, nint, nint*, HRESULT> CreateSwapChainForComposition;
    }
}