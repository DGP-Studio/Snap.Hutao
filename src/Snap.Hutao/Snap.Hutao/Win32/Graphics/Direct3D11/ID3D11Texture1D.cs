// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal unsafe readonly struct ID3D11Texture1D
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x27, 0x5C, 0xFB, 0xF8, 0xB3, 0xC6, 0x75, 0x4F, 0xA4, 0xC8, 0x43, 0x9A, 0xF2, 0xEF, 0x56, 0x4C];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11Resource.Vftbl ID3D11ResourceVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Texture1D*, D3D11_TEXTURE1D_DESC*, void> GetDesc;
    }
}