// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal unsafe readonly struct IDXGIFactory
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xEC, 0x66, 0x71, 0x7B, 0xC7, 0x21, 0xAE, 0x44, 0xB2, 0x1A, 0xC9, 0xAE, 0x32, 0x1A, 0xE3, 0x69];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIObject.Vftbl IDXGIObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory*, uint, IDXGIAdapter**, HRESULT> EnumAdapters;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory*, HWND, uint, HRESULT> MakeWindowAssociation;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory*, HWND*, HRESULT> GetWindowAssociation;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory*, IUnknown*, DXGI_SWAP_CHAIN_DESC*, IDXGISwapChain**, HRESULT> CreateSwapChain;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIFactory*, IDXGIAdapter**, HRESULT> CreateSoftwareAdapter;
    }
}