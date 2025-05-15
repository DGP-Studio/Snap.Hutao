// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao.Win32;

internal static unsafe class HutaoNativeMethods
{
    public const string DllName = "Snap.Hutao.Native.dll";

    public static HutaoNative HutaoCreateInstance()
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(HutaoCreateInstance((HutaoNative.Vftbl**)&pv));
        return new(ObjectReference<HutaoNative.Vftbl>.Attach(ref pv, typeof(HutaoNative.Vftbl).GUID));
    }

    [DllImport(DllName, CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    private static extern HRESULT HutaoCreateInstance(HutaoNative.Vftbl** ppv);


}