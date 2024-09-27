// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.Graphics.Direct3D11;

[SupportedOSPlatform("windows6.1")]
internal readonly unsafe struct ID3D11DeviceChild
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xC8, 0xE5, 0x41, 0x18, 0xB0, 0x16, 0x9B, 0x48, 0xBC, 0xC8, 0x44, 0xCF, 0xB0, 0xD5, 0xDE, 0xAE]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceChild*, ID3D11Device**, void> GetDevice;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceChild*, Guid*, uint*, void*, HRESULT> GetPrivateData;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceChild*, Guid*, uint, void*, HRESULT> SetPrivateData;
        internal readonly delegate* unmanaged[Stdcall]<ID3D11DeviceChild*, Guid*, IUnknown*, HRESULT> SetPrivateDataInterface;
    }
}