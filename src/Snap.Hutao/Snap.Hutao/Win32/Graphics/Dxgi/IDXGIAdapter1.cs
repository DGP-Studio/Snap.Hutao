// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal static unsafe class IDXGIAdapter1
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x61, 0x8F, 0x03, 0x29, 0x39, 0x38, 0x26, 0x46, 0x91, 0xFD, 0x08, 0x68, 0x79, 0x01, 0x1A, 0x05]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIAdapter.Vftbl IDXGIAdapterVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_ADAPTER_DESC1*, HRESULT> GetDesc1;
    }
}