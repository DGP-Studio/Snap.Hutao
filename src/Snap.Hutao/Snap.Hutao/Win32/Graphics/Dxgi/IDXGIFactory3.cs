// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

[SupportedOSPlatform("windows8.1")]
internal static unsafe class IDXGIFactory3
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x23, 0x38, 0x48, 0x25, 0x46, 0xCD, 0x7D, 0x4C, 0x86, 0xCA, 0x47, 0xAA, 0x95, 0xB8, 0x37, 0xBD]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIFactory2.Vftbl IDXGIFactory2Vftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint> GetCreationFlags;
    }
}