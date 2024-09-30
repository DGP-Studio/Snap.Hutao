// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal static unsafe class ID3D11Query
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x47, 0x07, 0xC0, 0xD6, 0xB7, 0x87, 0x5E, 0x42, 0xB8, 0x4D, 0x44, 0xD1, 0x08, 0x56, 0x0A, 0xFD]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11Asynchronous.Vftbl ID3D11AsynchronousVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_QUERY_DESC*, void> GetDesc;
    }
}