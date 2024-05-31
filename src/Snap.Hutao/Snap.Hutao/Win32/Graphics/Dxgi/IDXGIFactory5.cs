// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal unsafe readonly struct IDXGIFactory5
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xF5, 0xE1, 0x32, 0x76, 0x65, 0xEE, 0xCA, 0x4D, 0x87, 0xFD, 0x84, 0xCD, 0x75, 0xF8, 0x83, 0x8D];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIFactory4.Vftbl IDXGIFactory4Vftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory5*, DXGI_FEATURE, void*, uint, HRESULT> CheckFeatureSupport;
    }
}