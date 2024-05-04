// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Graphics.Direct3D;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SA1313")]
[SuppressMessage("", "SYSLIB1054")]
internal static class D3D12
{
    [DllImport("d3d12.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static unsafe extern HRESULT D3D12CreateDevice([AllowNull] IUnknown* pAdapter, D3D_FEATURE_LEVEL MinimumFeatureLevel, Guid* riid, void** ppDevice);

    public static unsafe HRESULT D3D12CreateDevice<T>([AllowNull] IUnknown* pAdapter, D3D_FEATURE_LEVEL MinimumFeatureLevel, ref readonly Guid riid, [MaybeNull] out T* pDevice)
        where T : unmanaged
    {
        fixed (Guid* riid2 = &riid)
        {
            fixed (T** pDevice2 = &pDevice)
            {
                return D3D12CreateDevice(pAdapter, MinimumFeatureLevel, riid2, (void**)pDevice2);
            }
        }
    }
}