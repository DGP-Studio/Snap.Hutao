// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal static unsafe class IDXGIFactory4
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x02, 0xEA, 0xC6, 0x1B, 0x36, 0xEF, 0x4F, 0x46, 0xBF, 0x0C, 0x21, 0xCA, 0x39, 0xE5, 0x16, 0x8A]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIFactory3.Vftbl IDXGIFactory3Vftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, LUID, Guid*, void**, HRESULT> EnumAdapterByLuid;
        internal readonly delegate* unmanaged[Stdcall]<nint, Guid*, void**, HRESULT> EnumWarpAdapter;
    }
}
