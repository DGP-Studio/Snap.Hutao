// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal readonly unsafe struct ID3D11View
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x16, 0x12, 0x9D, 0x83, 0x2E, 0xBB, 0x2B, 0x41, 0xB7, 0xF4, 0xA9, 0xDB, 0xEB, 0xE0, 0x8E, 0xD1]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11Resource.Vftbl ID3D11ResourceVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11View*, ID3D11Resource**, void> GetResource;
    }
}