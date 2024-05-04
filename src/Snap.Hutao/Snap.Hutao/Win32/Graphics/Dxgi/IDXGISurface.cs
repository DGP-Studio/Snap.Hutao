// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal unsafe readonly struct IDXGISurface
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

    internal readonly struct Vftbl
    {
        internal readonly IDXGIDeviceSubObject.Vftbl IDXGIDeviceSubObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISurface*, DXGI_SURFACE_DESC*, HRESULT> GetDesc;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISurface*, DXGI_MAPPED_RECT*, uint, HRESULT> Map;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISurface*, HRESULT> Unmap;
    }
}