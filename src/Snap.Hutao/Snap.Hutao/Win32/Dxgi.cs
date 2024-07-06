// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SA1313")]
[SuppressMessage("", "SYSLIB1054")]
internal static class Dxgi
{
    [DllImport("dxgi.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows8.1")]
    public static extern unsafe HRESULT CreateDXGIFactory2(uint Flags, Guid* riid, void** ppFactory);

    public static unsafe HRESULT CreateDXGIFactory2<T>(uint Flags, ref readonly Guid iid, out T* pFactory)
        where T : unmanaged
    {
        fixed (Guid* riid = &iid)
        {
            fixed (T** ppFactory = &pFactory)
            {
                return CreateDXGIFactory2(Flags, riid, (void**)ppFactory);
            }
        }
    }
}