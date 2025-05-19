// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao.Win32;

internal static unsafe class HutaoNativeMethods
{
    // ReSharper disable InconsistentNaming
    public const string IID_IHutaoString = "F1F44E9E-858D-4746-B44E-213A1DDA4510";

    public const string IID_IHutaoNativeLoopbackSupport = "8607ACE4-313C-4C26-B1FB-CA11173B6953";
    public const string IID_IHutaoNativeRegistryNotification = "EF118E91-8AD9-4C27-997D-DAF8910B34BE";
    public const string IID_IHutaoNativeWindowSubclass = "9631921E-A6CA-4150-9939-99B5467B2FD6";
    public const string IID_IHutaoNative = "D00F73FF-A1C7-4091-8CB6-D90991DD40CB";
    public const string IID_IHutaoNativeDeviceCapabilities = "1920EFA1-9953-4260-AFB1-35B1672758C1";
    public const string IID_IHutaoNativePhysicalDrive = "F90535D7-AFF6-4008-BA8C-15C47FC9660D";
    public const string IID_IHutaoNative2 = "338487EE-9592-4171-89DD-1E6B9EDB2C8E";
    public const string IID_IHutaoNativeInputLowLevelKeyboardSource = "8628902F-835C-4293-8580-794EC9BCCB98";
    public const string IID_IHutaoNative3 = "135FACE1-3184-4D12-B4D0-21FFB6A88D25";
    public const string IID_IHutaoNativeFileSystem = "FDD58117-0C7F-44D8-A7A2-8B1766474A93";
    public const string IID_IHutaoNativeFileSystem2 = "62616943-38e6-4bbb-84d1-dab847cb2145";
    public const string IID_IHutaoNativeFileSystem3 = "6DBCFC5C-2555-44DB-AF9D-2A2628CF726E";
    public const string IID_IHutaoNative4 = "27942FBE-322F-4157-9B8C-A38FDB827B05";
    public const string IID_IHutaoNativeNotifyIcon = "C2203FA4-BB97-47E0-8F72-C96879E4CB11";
    public const string IID_IHutaoNative5 = "7B4D20F1-4DAD-492E-8485-B4701A2ED19B";

    // ReSharper restore InconsistentNaming
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