// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal unsafe readonly struct ID3D11Counter
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xFB, 0x49, 0x8C, 0x6E, 0x71, 0xA3, 0x70, 0x47, 0xB4, 0x40, 0x29, 0x08, 0x60, 0x22, 0xB7, 0x41];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11Asynchronous.Vftbl ID3D11AsynchronousVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Query*, D3D11_COUNTER_DESC*, void> GetDesc;
    }
}