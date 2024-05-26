// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal unsafe readonly struct ID3D11Resource
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xF3, 0x63, 0x8E, 0xDC, 0x2B, 0xD1, 0x52, 0x49, 0xB4, 0x7B, 0x5E, 0x45, 0x02, 0x6A, 0x86, 0x2D];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11DeviceChild.Vftbl ID3D11DeviceChildVftbl;
        internal new readonly delegate* unmanaged[Stdcall]<ID3D11Resource*, D3D11_RESOURCE_DIMENSION*, void> GetType;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Resource*, uint, void> SetEvictionPriority;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11Resource*, uint> GetEvictionPriority;
    }
}