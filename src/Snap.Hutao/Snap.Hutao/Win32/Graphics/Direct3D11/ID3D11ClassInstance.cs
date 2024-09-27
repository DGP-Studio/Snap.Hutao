// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal readonly unsafe struct ID3D11ClassInstance
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xAA, 0x7F, 0xCD, 0xA6, 0xB7, 0xB0, 0x2F, 0x4A, 0x94, 0x36, 0x86, 0x62, 0xA6, 0x57, 0x97, 0xCB]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ID3D11DeviceChild.Vftbl ID3D11DeviceChildVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11ClassInstance*, ID3D11ClassLinkage**, void> GetClassLinkage;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11ClassInstance*, D3D11_CLASS_INSTANCE_DESC*, void> GetDesc;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11ClassInstance*, PSTR, nuint*, void> GetInstanceName;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11ClassInstance*, PSTR, nuint*, void> GetTypeName;
    }
}