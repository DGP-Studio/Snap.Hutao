// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

[Guid("d00f73ff-a1c7-4091-8cb6-d90991dd40cb")]
internal sealed unsafe class HutaoNative
{
    private readonly ObjectReference<Vftbl2>? objRef2;

    public HutaoNative(ObjectReference<Vftbl> objRef)
    {
        ObjRef = objRef;
        objRef.TryAs(new("338487ee-9592-4171-89dd-1e6b9edb2c8e"), out objRef2);
    }

    [field: MaybeNull]
    public static HutaoNative Instance
    {
        get => LazyInitializer.EnsureInitialized(ref field, static () =>
        {
#if DEBUG
            HutaoNativeMethods.HutaoInitializeWilCallbacks();
#endif
            return HutaoNativeMethods.HutaoCreateInstance();
        });
    }

    private ObjectReference<Vftbl> ObjRef { get; }

    private ObjectReference<Vftbl2>? ObjRef2 { get => objRef2; }

    public HutaoNativeLoopbackSupport MakeLoopbackSupport()
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef.Vftbl.MakeLoopbackSupport(ObjRef.ThisPtr, (HutaoNativeLoopbackSupport.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativeLoopbackSupport.Vftbl>.Attach(ref pv, typeof(HutaoNativeLoopbackSupport).GUID));
    }

    public HutaoNativeRegistryNotification MakeRegistryNotification(ReadOnlySpan<char> keyPath)
    {
        fixed (char* keyPathPtr = keyPath)
        {
            nint pv = default;
            Marshal.ThrowExceptionForHR(ObjRef.Vftbl.MakeRegistryNotification(ObjRef.ThisPtr, keyPathPtr, (HutaoNativeRegistryNotification.Vftbl**)&pv));
            return new(ObjectReference<HutaoNativeRegistryNotification.Vftbl>.Attach(ref pv, typeof(HutaoNativeRegistryNotification).GUID));
        }
    }

    public HutaoNativeWindowSubclass MakeWindowSubclass(HWND hWnd, HutaoNativeWindowSubclassCallback callback, nint userData)
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef.Vftbl.MakeWindowSubclass(ObjRef.ThisPtr, hWnd, callback, userData, (HutaoNativeWindowSubclass.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativeWindowSubclass.Vftbl>.Attach(ref pv, typeof(HutaoNativeWindowSubclass).GUID));
    }

    public HutaoNativeDeviceCapabilities MakeDeviceCapabilities()
    {
        HutaoException.NotSupportedIf(ObjRef2 is null, "IHutaoNative2 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef2.Vftbl.MakeLoopbackSupport(ObjRef2.ThisPtr, (HutaoNativeDeviceCapabilities.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativeDeviceCapabilities.Vftbl>.Attach(ref pv, typeof(HutaoNativeDeviceCapabilities).GUID));
    }

    public HutaoNativePhysicalDrive MakePhysicalDrive()
    {
        HutaoException.NotSupportedIf(ObjRef2 is null, "IHutaoNative2 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef2.Vftbl.MakePhysicalDrive(ObjRef2.ThisPtr, (HutaoNativePhysicalDrive.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativePhysicalDrive.Vftbl>.Attach(ref pv, typeof(HutaoNativePhysicalDrive).GUID));
    }

    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeLoopbackSupport.Vftbl**, HRESULT> MakeLoopbackSupport;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HutaoNativeRegistryNotification.Vftbl**, HRESULT> MakeRegistryNotification;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, HutaoNativeWindowSubclassCallback, nint, HutaoNativeWindowSubclass.Vftbl**, HRESULT> MakeWindowSubclass;
#pragma warning restore CS0649
    }

    private readonly struct Vftbl2
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeDeviceCapabilities.Vftbl**, HRESULT> MakeLoopbackSupport;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativePhysicalDrive.Vftbl**, HRESULT> MakePhysicalDrive;
#pragma warning restore CS0649
    }
}