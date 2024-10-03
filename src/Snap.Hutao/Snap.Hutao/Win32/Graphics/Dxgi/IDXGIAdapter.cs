// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal static unsafe class IDXGIAdapter
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xE1, 0xE7, 0x11, 0x24, 0xAC, 0x12, 0xCF, 0x4C, 0xBD, 0x14, 0x97, 0x98, 0xE8, 0x53, 0x4D, 0xC0]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIObject.Vftbl IDXGIObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint*, HRESULT> EnumOutputs;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_ADAPTER_DESC*, HRESULT> GetDesc;
        internal readonly delegate* unmanaged[Stdcall]<nint, Guid*, long*, HRESULT> CheckInterfaceSupport;
    }
}