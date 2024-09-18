// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SYSLIB1054")]
internal static class Ole32
{
    [DllImport("OLE32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern unsafe HRESULT CoCreateInstance(Guid* rclsid, [AllowNull] IUnknown pUnkOuter, CLSCTX dwClsContext, Guid* riid, void** ppv);

    [DebuggerStepThrough]
    public static unsafe HRESULT CoCreateInstance<TVftbl>(ref readonly Guid clsid, [AllowNull] IUnknown pUnkOuter, CLSCTX dwClsContext, ref readonly Guid iid, out ObjectReference<TVftbl> v)
        where TVftbl : unmanaged
    {
        fixed (Guid* rclsid = &clsid)
        {
            fixed (Guid* riid = &iid)
            {
                TVftbl** pv = default;
                HRESULT hr = CoCreateInstance(rclsid, pUnkOuter, dwClsContext, riid, (void**)&pv);
                v = ObjectReference<TVftbl>.Attach(ref Unsafe.AsRef<nint>(&pv), iid);
                return hr;
            }
        }
    }

    [DllImport("OLE32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern unsafe void CoTaskMemFree([AllowNull] void* pv);

    [DllImport("OLE32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern unsafe HRESULT CoWaitForMultipleObjects(uint dwFlags, uint dwTimeout, uint cHandles, HANDLE* pHandles, uint* lpdwindex);

    [DebuggerStepThrough]
    public static unsafe HRESULT CoWaitForMultipleObjects(CWMO_FLAGS dwFlags, uint dwTimeout, ReadOnlySpan<HANDLE> handles, out uint dwindex)
    {
        fixed (HANDLE* pHandles = handles)
        {
            fixed (uint* lpdwindex = &dwindex)
            {
                return CoWaitForMultipleObjects((uint)dwFlags, dwTimeout, (uint)handles.Length, pHandles, lpdwindex);
            }
        }
    }
}