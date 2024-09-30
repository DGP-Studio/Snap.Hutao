// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

[SuppressMessage("", "SA1313")]
internal static unsafe class IDXGISwapChain1
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xF7, 0x45, 0x0A, 0x79, 0x42, 0x0D, 0x76, 0x48, 0x98, 0x3A, 0x0A, 0x55, 0xCF, 0xE6, 0xF4, 0xAA]);
    }

    public static HRESULT Present(this ObjectReference<Vftbl> objRef, uint SyncInterval, uint Flags)
    {
        return objRef.Vftbl.IDXGISwapChainVftbl.Present(objRef.ThisPtr, SyncInterval, Flags);
    }

    public static HRESULT GetBuffer<TVftbl>(this ObjectReference<Vftbl> objRef, uint Buffer, ref readonly Guid iid, out ObjectReference<TVftbl> surface)
        where TVftbl : unmanaged
    {
        fixed (Guid* riid = &iid)
        {
            nint pSurface = default;
            HRESULT hr = objRef.Vftbl.IDXGISwapChainVftbl.GetBuffer(objRef.ThisPtr, Buffer, riid, (void**)&pSurface);
            surface = ObjectReference<TVftbl>.Attach(ref pSurface, iid);
            return hr;
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGISwapChain.Vftbl IDXGISwapChainVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_SWAP_CHAIN_DESC1*, HRESULT> GetDesc1;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_SWAP_CHAIN_FULLSCREEN_DESC*, HRESULT> GetFullscreenDesc;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND*, HRESULT> GetHwnd;
        internal readonly delegate* unmanaged[Stdcall]<nint, Guid*, void**, HRESULT> GetCoreWindow;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, DXGI_PRESENT_PARAMETERS*, HRESULT> Present1;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL> IsTemporaryMonoSupported;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> GetRestrictToOutput;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_RGBA*, HRESULT> SetBackgroundColor;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_RGBA*, HRESULT> GetBackgroundColor;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_MODE_ROTATION, HRESULT> SetRotation;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_MODE_ROTATION*, HRESULT> GetRotation;
    }
}