// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal readonly unsafe struct IDXGIDeviceSubObject
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x79, 0x03, 0x3E, 0x3D, 0xDE, 0xF9, 0x58, 0x4D, 0xBB, 0x6C, 0x18, 0xD6, 0x29, 0x92, 0xF1, 0xA6]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIObject.Vftbl IDXGIObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIDeviceSubObject*, Guid*, void**, HRESULT> GetDevice;
    }
}