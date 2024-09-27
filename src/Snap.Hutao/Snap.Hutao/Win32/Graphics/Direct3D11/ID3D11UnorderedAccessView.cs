// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal readonly unsafe struct ID3D11UnorderedAccessView
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x09, 0xF5, 0xAC, 0x28, 0x5C, 0x7F, 0xF6, 0x48, 0x86, 0x11, 0xF3, 0x16, 0x01, 0x0A, 0x63, 0x80]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11View.Vftbl ID3D11ViewVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11UnorderedAccessView*, D3D11_UNORDERED_ACCESS_VIEW_DESC*, void> GetDesc;
    }
}