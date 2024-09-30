// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

[SuppressMessage("", "SA1313")]
internal unsafe struct IDXGISwapChain1
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xF7, 0x45, 0x0A, 0x79, 0x42, 0x0D, 0x76, 0x48, 0x98, 0x3A, 0x0A, 0x55, 0xCF, 0xE6, 0xF4, 0xAA]);
    }

    public HRESULT Present(uint SyncInterval, uint Flags)
    {
        return ThisPtr->IDXGISwapChainVftbl.Present((IDXGISwapChain*)Unsafe.AsPointer(ref this), SyncInterval, Flags);
    }

    public HRESULT GetBuffer<TVftbl>(uint Buffer, ref readonly Guid iid, out ObjectReference<TVftbl> surface)
        where TVftbl : unmanaged
    {
        fixed (Guid* riid = &iid)
        {
            TVftbl** pSurface = default;
            HRESULT hr = ThisPtr->IDXGISwapChainVftbl.GetBuffer((IDXGISwapChain*)Unsafe.AsPointer(ref this), Buffer, riid, (void**)&pSurface);
            surface = ObjectReference<TVftbl>.Attach(ref *(nint*)&pSurface, iid);
            return hr;
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGISwapChain.Vftbl IDXGISwapChainVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain1*, DXGI_SWAP_CHAIN_DESC1*, HRESULT> GetDesc1;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain1*, DXGI_SWAP_CHAIN_FULLSCREEN_DESC*, HRESULT> GetFullscreenDesc;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain1*, HWND*, HRESULT> GetHwnd;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain1*, Guid*, void**, HRESULT> GetCoreWindow;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain1*, uint, uint, DXGI_PRESENT_PARAMETERS*, HRESULT> Present1;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain1*, BOOL> IsTemporaryMonoSupported;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain1*, IDXGIOutput**, HRESULT> GetRestrictToOutput;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain1*, DXGI_RGBA*, HRESULT> SetBackgroundColor;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain1*, DXGI_RGBA*, HRESULT> GetBackgroundColor;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain1*, DXGI_MODE_ROTATION, HRESULT> SetRotation;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain1*, DXGI_MODE_ROTATION*, HRESULT> GetRotation;
    }
}