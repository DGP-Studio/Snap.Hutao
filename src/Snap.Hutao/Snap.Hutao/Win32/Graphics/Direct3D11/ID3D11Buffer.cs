// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal static unsafe class ID3D11Buffer
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x85, 0x0B, 0x57, 0x48, 0xEE, 0xD1, 0xCD, 0x4F, 0xA2, 0x50, 0xEB, 0x35, 0x07, 0x22, 0xB0, 0x37]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11Resource.Vftbl ID3D11ResourceVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_BUFFER_DESC*, void> GetDesc;
    }
}