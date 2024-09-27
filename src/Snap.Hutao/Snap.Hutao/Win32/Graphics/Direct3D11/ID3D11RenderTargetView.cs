// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal readonly unsafe struct ID3D11RenderTargetView
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x67, 0xA0, 0xDB, 0xDF, 0x8D, 0x0B, 0x65, 0x48, 0x87, 0x5B, 0xD7, 0xB4, 0x51, 0x6C, 0xC1, 0x64]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11View.Vftbl ID3D11ViewVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11RenderTargetView*, D3D11_RENDER_TARGET_VIEW_DESC*, void> GetDesc;
    }
}