// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

internal unsafe readonly struct ID3D11ClassLinkage
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xBA, 0x7C, 0xF5, 0xDD, 0x43, 0x95, 0xE4, 0x46, 0xA1, 0x2B, 0xF2, 0x07, 0xA0, 0xFE, 0x7F, 0xED];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11DeviceChild.Vftbl ID3D11DeviceChildVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11ClassLinkage*, PCSTR, uint, ID3D11ClassInstance**, HRESULT> GetClassInstance;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11ClassLinkage*, PCSTR, uint, uint, uint, uint, ID3D11ClassInstance**, HRESULT> CreateClassInstance;
    }
}