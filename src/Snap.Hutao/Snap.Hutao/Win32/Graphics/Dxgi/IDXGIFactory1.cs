// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

[SupportedOSPlatform("windows6.1")]
internal readonly unsafe struct IDXGIFactory1
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x78, 0xAE, 0x0A, 0x77, 0x6F, 0xF2, 0xBA, 0x4D, 0xA8, 0x29, 0x25, 0x3C, 0x83, 0xD1, 0xB3, 0x87]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIFactory.Vftbl IDXGIFactoryVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory1*, uint, IDXGIAdapter1**, HRESULT> EnumAdapters1;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory1*, BOOL> IsCurrent;
    }
}