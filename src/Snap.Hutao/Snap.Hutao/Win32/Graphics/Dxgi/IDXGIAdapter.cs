﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal readonly unsafe struct IDXGIAdapter
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xE1, 0xE7, 0x11, 0x24, 0xAC, 0x12, 0xCF, 0x4C, 0xBD, 0x14, 0x97, 0x98, 0xE8, 0x53, 0x4D, 0xC0];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIObject.Vftbl IDXGIObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIAdapter*, uint, IDXGIOutput**, HRESULT> EnumOutputs;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIAdapter*, DXGI_ADAPTER_DESC*, HRESULT> GetDesc;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIAdapter*, Guid*, long*, HRESULT> CheckInterfaceSupport;
    }
}