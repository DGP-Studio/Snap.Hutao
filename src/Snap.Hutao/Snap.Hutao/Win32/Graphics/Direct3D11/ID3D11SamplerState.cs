﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal readonly unsafe struct ID3D11SamplerState
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x51, 0xEA, 0x6F, 0xDA, 0x4C, 0x56, 0x87, 0x44, 0x98, 0x10, 0xF0, 0xD0, 0xF9, 0xB4, 0xE3, 0xA5];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11DeviceChild.Vftbl ID3D11DeviceChildVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11SamplerState*, D3D11_SAMPLER_DESC*, void> GetDesc;
    }
}