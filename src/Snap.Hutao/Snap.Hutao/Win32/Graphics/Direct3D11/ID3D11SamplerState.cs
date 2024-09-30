// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal static unsafe class ID3D11SamplerState
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x51, 0xEA, 0x6F, 0xDA, 0x4C, 0x56, 0x87, 0x44, 0x98, 0x10, 0xF0, 0xD0, 0xF9, 0xB4, 0xE3, 0xA5]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11DeviceChild.Vftbl ID3D11DeviceChildVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, D3D11_SAMPLER_DESC*, void> GetDesc;
    }
}