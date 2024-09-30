// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SA1313")]
[SuppressMessage("", "SYSLIB1054")]
internal static class Dxgi
{
    [DllImport("dxgi.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows8.1")]
    public static extern unsafe HRESULT CreateDXGIFactory2(uint Flags, Guid* riid, void** ppFactory);

    public static unsafe HRESULT CreateDXGIFactory2<TVftbl>(uint Flags, ref readonly Guid iid, out ObjectReference<TVftbl> factory)
        where TVftbl : unmanaged
    {
        fixed (Guid* riid = &iid)
        {
            TVftbl** pFactory = default;
            HRESULT hr = CreateDXGIFactory2(Flags, riid, (void**)&pFactory);
            factory = ObjectReference<TVftbl>.Attach(ref *(nint*)&pFactory, iid);
            return hr;
        }
    }
}