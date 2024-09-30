// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT.Interop;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal static unsafe class IDXGIObject
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xB8, 0x2F, 0xC2, 0xAE, 0xF3, 0x76, 0x39, 0x46, 0x9B, 0xE0, 0x28, 0xEB, 0x43, 0xA6, 0x7A, 0x2E]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, Guid*, uint, void*, HRESULT> SetPrivateData;
        internal readonly delegate* unmanaged[Stdcall]<nint, Guid*, nint, HRESULT> SetPrivateDataInterface;
        internal readonly delegate* unmanaged[Stdcall]<nint, Guid*, uint*, void*, HRESULT> GetPrivateData;
        internal readonly delegate* unmanaged[Stdcall]<nint, Guid*, void**, HRESULT> GetParent;
    }
}