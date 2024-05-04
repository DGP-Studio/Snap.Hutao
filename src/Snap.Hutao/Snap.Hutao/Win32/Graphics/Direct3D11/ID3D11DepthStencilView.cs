// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal unsafe readonly struct ID3D11DepthStencilView
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x2A, 0xC9, 0xDA, 0x9F, 0x76, 0x18, 0xC3, 0x48, 0xAF, 0xAD, 0x25, 0xB9, 0x4F, 0x84, 0xA9, 0xB6];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11View.Vftbl ID3D11ViewVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DepthStencilView*, D3D11_DEPTH_STENCIL_VIEW_DESC*, void> GetDesc;
    }
}