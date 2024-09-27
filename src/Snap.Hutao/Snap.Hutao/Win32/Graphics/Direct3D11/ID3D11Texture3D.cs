// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal readonly unsafe struct ID3D11Texture3D
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x6E, 0x86, 0x7E, 0x03, 0x6D, 0xF5, 0x57, 0x43, 0xA8, 0xAF, 0x9D, 0xAB, 0xBE, 0x6E, 0x25, 0x0E]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11Resource.Vftbl ID3D11ResourceVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Texture3D*, D3D11_TEXTURE3D_DESC*, void> GetDesc;
    }
}