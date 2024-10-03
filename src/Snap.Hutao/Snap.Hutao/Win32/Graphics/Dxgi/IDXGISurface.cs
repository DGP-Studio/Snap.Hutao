// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal static unsafe class IDXGISurface
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x6C, 0xB5, 0xFC, 0xCA, 0xC3, 0x6A, 0x89, 0x48, 0xBF, 0x47, 0x9E, 0x23, 0xBB, 0xD2, 0x60, 0xEC]);
    }

    public static HRESULT GetDevice<TVftbl>(this ObjectReference<Vftbl> objRef, ref readonly Guid iid, out ObjectReference<TVftbl> device)
        where TVftbl : unmanaged
    {
        fixed (Guid* riid = &iid)
        {
            nint pDevice;
            HRESULT hr = objRef.Vftbl.IDXGIDeviceSubObjectVftbl.GetDevice(objRef.ThisPtr, riid, (void**)&pDevice);
            device = ObjectReference<TVftbl>.Attach(ref pDevice, iid);
            return hr;
        }
    }

    public static HRESULT GetDesc(this ObjectReference<Vftbl> objRef, out DXGI_SURFACE_DESC desc)
    {
        fixed (DXGI_SURFACE_DESC* pDesc = &desc)
        {
            return objRef.Vftbl.GetDesc(objRef.ThisPtr, pDesc);
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIDeviceSubObject.Vftbl IDXGIDeviceSubObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_SURFACE_DESC*, HRESULT> GetDesc;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_MAPPED_RECT*, uint, HRESULT> Map;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Unmap;
    }
}