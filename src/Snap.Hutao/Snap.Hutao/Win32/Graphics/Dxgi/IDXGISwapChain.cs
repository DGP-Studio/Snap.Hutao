// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal readonly unsafe struct IDXGISwapChain
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xA0, 0x36, 0x0D, 0x31, 0xE7, 0xD2, 0x0A, 0x4C, 0xAA, 0x04, 0x6A, 0x9D, 0x23, 0xB8, 0x88, 0x6A];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIDeviceSubObject.Vftbl IDXGIDeviceSubObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain*, uint, uint, HRESULT> Present;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain*, uint, Guid*, void**, HRESULT> GetBuffer;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain*, BOOL, IDXGIOutput*, HRESULT> SetFullscreenState;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain*, BOOL*, IDXGIOutput**, HRESULT> GetFullscreenState;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain*, DXGI_SWAP_CHAIN_DESC*, HRESULT> GetDesc;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain*, uint, uint, uint, DXGI_FORMAT, uint, HRESULT> ResizeBuffers;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain*, DXGI_MODE_DESC*, HRESULT> ResizeTarget;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain*, IDXGIOutput**, HRESULT> GetContainingOutput;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain*, DXGI_FRAME_STATISTICS*, HRESULT> GetFrameStatistics;
        internal readonly delegate* unmanaged[Stdcall]<IDXGISwapChain*, uint*, HRESULT> GetLastPresentCount;
    }
}