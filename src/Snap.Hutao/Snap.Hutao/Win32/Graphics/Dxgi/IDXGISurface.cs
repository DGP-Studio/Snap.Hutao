// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal unsafe struct IDXGISurface
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x6C, 0xB5, 0xFC, 0xCA, 0xC3, 0x6A, 0x89, 0x48, 0xBF, 0x47, 0x9E, 0x23, 0xBB, 0xD2, 0x60, 0xEC];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public HRESULT GetDevice<TVftbl>(ref readonly Guid iid, out ObjectReference<TVftbl> device)
        where TVftbl : unmanaged
    {
        fixed (Guid* riid = &iid)
        {
            TVftbl** pDevice;
            HRESULT hr = ThisPtr->IDXGIDeviceSubObjectVftbl.GetDevice((IDXGIDeviceSubObject*)Unsafe.AsPointer(ref this), riid, (void**)&pDevice);
            device = ObjectReference<TVftbl>.Attach(ref Unsafe.AsRef<nint>(&pDevice), iid);
            return hr;
        }
    }

    public HRESULT GetDesc(out DXGI_SURFACE_DESC desc)
    {
        fixed (DXGI_SURFACE_DESC* pDesc = &desc)
        {
            return ThisPtr->GetDesc((IDXGISurface*)Unsafe.AsPointer(ref this), pDesc);
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIDeviceSubObject.Vftbl IDXGIDeviceSubObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISurface*, DXGI_SURFACE_DESC*, HRESULT> GetDesc;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISurface*, DXGI_MAPPED_RECT*, uint, HRESULT> Map;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISurface*, HRESULT> Unmap;
    }
}