// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal static unsafe class IDXGIFactory
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xEC, 0x66, 0x71, 0x7B, 0xC7, 0x21, 0xAE, 0x44, 0xB2, 0x1A, 0xC9, 0xAE, 0x32, 0x1A, 0xE3, 0x69]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIObject.Vftbl IDXGIObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint*, HRESULT> EnumAdapters;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, uint, HRESULT> MakeWindowAssociation;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND*, HRESULT> GetWindowAssociation;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, DXGI_SWAP_CHAIN_DESC*, nint*, HRESULT> CreateSwapChain;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> CreateSoftwareAdapter;
    }
}