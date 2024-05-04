// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal unsafe readonly struct ID3D11Predicate
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xDD, 0x76, 0xB5, 0x9E, 0x77, 0x9F, 0x86, 0x4D, 0x81, 0xAA, 0x8B, 0xAB, 0x5F, 0xE4, 0x90, 0xE2];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11Query.Vftbl ID3D11QueryVftbl;
    }
}