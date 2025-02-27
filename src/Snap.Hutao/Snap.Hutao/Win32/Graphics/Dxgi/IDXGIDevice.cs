// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal static unsafe class IDXGIDevice
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xFA, 0x77, 0xEC, 0x54, 0x77, 0x13, 0xE6, 0x44, 0x8C, 0x32, 0x88, 0xFD, 0x5F, 0x44, 0xC8, 0x4C]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIObject.Vftbl IDXGIObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> GetAdapter;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_SURFACE_DESC*, uint, DXGI_USAGE, DXGI_SHARED_RESOURCE*, nint*, HRESULT> CreateSurface;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, DXGI_RESIDENCY*, uint, HRESULT> QueryResourceResidency;
        internal readonly delegate* unmanaged[Stdcall]<nint, int, HRESULT> SetGPUThreadPriority;
        internal readonly delegate* unmanaged[Stdcall]<nint, int*, HRESULT> GetGPUThreadPriority;
    }
}