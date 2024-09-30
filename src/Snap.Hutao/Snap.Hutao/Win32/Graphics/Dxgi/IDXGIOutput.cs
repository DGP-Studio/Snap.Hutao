// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal static unsafe class IDXGIOutput
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xDB, 0xEE, 0x02, 0xAE, 0x35, 0xC7, 0x90, 0x46, 0x8D, 0x52, 0x5A, 0x8D, 0xC2, 0x02, 0x13, 0xAA]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIObject.Vftbl IDXGIObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_OUTPUT_DESC*, HRESULT> GetDesc;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_FORMAT, uint, uint*, DXGI_MODE_DESC*, HRESULT> GetDisplayModeList;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_MODE_DESC*, DXGI_MODE_DESC*, nint, HRESULT> FindClosestMatchingMode;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> WaitForVBlank;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, BOOL, HRESULT> TakeOwnership;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> ReleaseOwnership;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_GAMMA_CONTROL_CAPABILITIES*, HRESULT> GetGammaControlCapabilities;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_GAMMA_CONTROL*, HRESULT> SetGammaControl;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_GAMMA_CONTROL*, HRESULT> GetGammaControl;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> SetDisplaySurface;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> GetDisplaySurfaceData;
        internal readonly delegate* unmanaged[Stdcall]<nint, DXGI_FRAME_STATISTICS*, HRESULT> GetFrameStatistics;
    }
}