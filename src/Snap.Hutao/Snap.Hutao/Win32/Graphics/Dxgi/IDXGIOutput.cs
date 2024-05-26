// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Dxgi.Common;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.Graphics.Dxgi;

internal unsafe readonly struct IDXGIOutput
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xDB, 0xEE, 0x02, 0xAE, 0x35, 0xC7, 0x90, 0x46, 0x8D, 0x52, 0x5A, 0x8D, 0xC2, 0x02, 0x13, 0xAA];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IDXGIObject.Vftbl IDXGIObjectVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIOutput*, DXGI_OUTPUT_DESC*, HRESULT> GetDesc;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIOutput*, DXGI_FORMAT, uint, uint*, DXGI_MODE_DESC*, HRESULT> GetDisplayModeList;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIOutput*, DXGI_MODE_DESC*, DXGI_MODE_DESC*, IUnknown*, HRESULT> FindClosestMatchingMode;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIOutput*, HRESULT> WaitForVBlank;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIOutput*, IUnknown*, BOOL, HRESULT> TakeOwnership;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIOutput*, HRESULT> ReleaseOwnership;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIOutput*, DXGI_GAMMA_CONTROL_CAPABILITIES*, HRESULT> GetGammaControlCapabilities;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIOutput*, DXGI_GAMMA_CONTROL*, HRESULT> SetGammaControl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIOutput*, DXGI_GAMMA_CONTROL*, HRESULT> GetGammaControl;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIOutput*, IDXGISurface*, HRESULT> SetDisplaySurface;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIOutput*, IDXGISurface*, HRESULT> GetDisplaySurfaceData;
        internal readonly delegate* unmanaged[Stdcall]<IDXGIOutput*, DXGI_FRAME_STATISTICS*, HRESULT> GetFrameStatistics;
    }
}