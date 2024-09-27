// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal readonly unsafe struct ID3D11Predicate
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xDD, 0x76, 0xB5, 0x9E, 0x77, 0x9F, 0x86, 0x4D, 0x81, 0xAA, 0x8B, 0xAB, 0x5F, 0xE4, 0x90, 0xE2]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11Query.Vftbl ID3D11QueryVftbl;
    }
}