// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal readonly unsafe struct ID3D11BlendState
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xAA, 0x8F, 0xB6, 0x75, 0x7D, 0x34, 0x59, 0x41, 0x8F, 0x45, 0xA0, 0x64, 0x0F, 0x01, 0xCD, 0x9A]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11DeviceChild.Vftbl ID3D11DeviceChildVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11BlendState*, D3D11_BLEND_DESC*, void> GetDesc;
    }
}