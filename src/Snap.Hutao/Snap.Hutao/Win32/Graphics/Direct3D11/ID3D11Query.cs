// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal unsafe readonly struct ID3D11Query
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x47, 0x07, 0xC0, 0xD6, 0xB7, 0x87, 0x5E, 0x42, 0xB8, 0x4D, 0x44, 0xD1, 0x08, 0x56, 0x0A, 0xFD];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11Asynchronous.Vftbl ID3D11AsynchronousVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Query*, D3D11_QUERY_DESC*, void> GetDesc;
    }
}