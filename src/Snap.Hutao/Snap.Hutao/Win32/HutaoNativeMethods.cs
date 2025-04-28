// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao.Win32;

internal static unsafe class HutaoNativeMethods
{
    [DllImport("Snap.Hutao.Native.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern HRESULT HutaoCreateInstance(HutaoNative.Vftbl** ppv);

    public static HutaoNative HutaoCreateInstance()
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(HutaoCreateInstance((HutaoNative.Vftbl**)&pv));
        return new(ObjectReference<HutaoNative.Vftbl>.Attach(ref pv, typeof(HutaoNative).GUID));
    }

    [DllImport("Snap.Hutao.Native.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern HRESULT HutaoInitializeDiagnostics(delegate* unmanaged[Stdcall]<PCWSTR, void> logger);

    public static void HutaoInitializeDiagnostics()
    {
        Marshal.ThrowExceptionForHR(HutaoInitializeDiagnostics(&DebugWriteLine));
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void DebugWriteLine(PCWSTR str)
    {
        ReadOnlySpan<char> span = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(str);
        Debug.WriteLine(span.ToString());
    }
}