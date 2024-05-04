// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal unsafe readonly struct IDXGIDevice
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xFA, 0x77, 0xEC, 0x54, 0x77, 0x13, 0xE6, 0x44, 0x8C, 0x32, 0x88, 0xFD, 0x5F, 0x44, 0xC8, 0x4C];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIObject.Vftbl IDXGIObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIDevice*, IDXGIAdapter**, HRESULT> GetAdapter;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIDevice*, DXGI_SURFACE_DESC*, uint, DXGI_USAGE, DXGI_SHARED_RESOURCE*, IDXGISurface**, HRESULT> CreateSurface;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIDevice*, IUnknown**, DXGI_RESIDENCY*, uint, HRESULT> QueryResourceResidency;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIDevice*, int, HRESULT> SetGPUThreadPriority;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIDevice*, int*, HRESULT> GetGPUThreadPriority;
    }
}