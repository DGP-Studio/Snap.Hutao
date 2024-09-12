// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

[SupportedOSPlatform("windows8.0")]
internal readonly unsafe struct IDXGIFactory2
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x1C, 0x3A, 0xC8, 0x50, 0x72, 0xE0, 0x48, 0x4C, 0x87, 0xB0, 0x36, 0x30, 0xFA, 0x36, 0xA6, 0xD0];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIFactory1.Vftbl IDXGIFactory1Vftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory2*, BOOL> IsWindowedStereoEnabled;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory2*, IUnknown*, HWND, DXGI_SWAP_CHAIN_DESC1*, DXGI_SWAP_CHAIN_FULLSCREEN_DESC*, IDXGIOutput*, IDXGISwapChain1**, HRESULT> CreateSwapChainForHwnd;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory2*, IUnknown*, IUnknown*, DXGI_SWAP_CHAIN_DESC1*, IDXGIOutput*, IDXGISwapChain1**, HRESULT> CreateSwapChainForCoreWindow;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory2*, HANDLE, LUID*, HRESULT> GetSharedResourceAdapterLuid;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory2*, HWND, uint, uint*, HRESULT> RegisterStereoStatusWindow;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory2*, HANDLE, uint*, HRESULT> RegisterStereoStatusEvent;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory2*, uint, void> UnregisterStereoStatus;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory2*, HWND, uint, uint*, HRESULT> RegisterOcclusionStatusWindow;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory2*, HANDLE, uint*, HRESULT> RegisterOcclusionStatusEvent;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory2*, uint, void> UnregisterOcclusionStatus;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory2*, IUnknown*, DXGI_SWAP_CHAIN_DESC1*, IDXGIOutput*, IDXGISwapChain1**, HRESULT> CreateSwapChainForComposition;
    }
}