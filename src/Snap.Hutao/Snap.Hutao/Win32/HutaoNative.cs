// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNative
{
    private readonly ObjectReference<Vftbl2>? objRef2;
    private readonly ObjectReference<Vftbl3>? objRef3;
    private readonly ObjectReference<Vftbl4>? objRef4;
    private readonly ObjectReference<Vftbl5>? objRef5;

    public HutaoNative(ObjectReference<Vftbl> objRef)
    {
        ObjRef = objRef;
        objRef.TryAs(typeof(Vftbl2).GUID, out objRef2);
        objRef.TryAs(typeof(Vftbl3).GUID, out objRef3);
        objRef.TryAs(typeof(Vftbl4).GUID, out objRef4);
        objRef.TryAs(typeof(Vftbl5).GUID, out objRef5);
    }

    [field: MaybeNull]
    public static HutaoNative Instance
    {
        get => LazyInitializer.EnsureInitialized(ref field, static () =>
        {
#if DEBUG
            HutaoNativeWilCallbacks.HutaoInitializeWilCallbacks();
#endif
            return HutaoNativeMethods.HutaoCreateInstance();
        });
    }

    private ObjectReference<Vftbl> ObjRef { get; }

    private ObjectReference<Vftbl2>? ObjRef2 { get => objRef2; }

    private ObjectReference<Vftbl3>? ObjRef3 { get => objRef3; }

    private ObjectReference<Vftbl4>? ObjRef4 { get => objRef4; }

    private ObjectReference<Vftbl5>? ObjRef5 { get => objRef5; }

    public HutaoNativeLoopbackSupport MakeLoopbackSupport()
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef.Vftbl.MakeLoopbackSupport(ObjRef.ThisPtr, (HutaoNativeLoopbackSupport.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativeLoopbackSupport.Vftbl>.Attach(ref pv, typeof(HutaoNativeLoopbackSupport.Vftbl).GUID));
    }

    public HutaoNativeRegistryNotification MakeRegistryNotification(ReadOnlySpan<char> keyPath)
    {
        fixed (char* keyPathPtr = keyPath)
        {
            nint pv = default;
            Marshal.ThrowExceptionForHR(ObjRef.Vftbl.MakeRegistryNotification(ObjRef.ThisPtr, keyPathPtr, (HutaoNativeRegistryNotification.Vftbl**)&pv));
            return new(ObjectReference<HutaoNativeRegistryNotification.Vftbl>.Attach(ref pv, typeof(HutaoNativeRegistryNotification.Vftbl).GUID));
        }
    }

    public HutaoNativeWindowSubclass MakeWindowSubclass(HWND hWnd, HutaoNativeWindowSubclassCallback callback, nint userData)
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef.Vftbl.MakeWindowSubclass(ObjRef.ThisPtr, hWnd, callback, userData, (HutaoNativeWindowSubclass.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativeWindowSubclass.Vftbl>.Attach(ref pv, typeof(HutaoNativeWindowSubclass.Vftbl).GUID));
    }

    public HutaoNativeDeviceCapabilities MakeDeviceCapabilities()
    {
        HutaoException.NotSupportedIf(ObjRef2 is null, "IHutaoNative2 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef2.Vftbl.MakeLoopbackSupport(ObjRef2.ThisPtr, (HutaoNativeDeviceCapabilities.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativeDeviceCapabilities.Vftbl>.Attach(ref pv, typeof(HutaoNativeDeviceCapabilities.Vftbl).GUID));
    }

    public HutaoNativePhysicalDrive MakePhysicalDrive()
    {
        HutaoException.NotSupportedIf(ObjRef2 is null, "IHutaoNative2 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef2.Vftbl.MakePhysicalDrive(ObjRef2.ThisPtr, (HutaoNativePhysicalDrive.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativePhysicalDrive.Vftbl>.Attach(ref pv, typeof(HutaoNativePhysicalDrive.Vftbl).GUID));
    }

    public HutaoNativeInputLowLevelKeyboardSource MakeInputLowLevelKeyboardSource()
    {
        HutaoException.NotSupportedIf(ObjRef3 is null, "IHutaoNative3 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef3.Vftbl.MakeInputLowLevelKeyboardSource(ObjRef3.ThisPtr, (HutaoNativeInputLowLevelKeyboardSource.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativeInputLowLevelKeyboardSource.Vftbl>.Attach(ref pv, typeof(HutaoNativeInputLowLevelKeyboardSource.Vftbl).GUID));
    }

    public HutaoNativeFileSystem MakeFileSystem()
    {
        HutaoException.NotSupportedIf(ObjRef4 is null, "IHutaoNative4 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef4.Vftbl.MakeFileSystem(ObjRef4.ThisPtr, (HutaoNativeFileSystem.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativeFileSystem.Vftbl>.Attach(ref pv, typeof(HutaoNativeFileSystem.Vftbl).GUID));
    }

    public HutaoNativeNotifyIcon MakeNotifyIcon(ReadOnlySpan<char> iconPath, ref readonly Guid id)
    {
        HutaoException.NotSupportedIf(ObjRef5 is null, "IHutaoNative5 is not supported");
        fixed (char* pIconPath = iconPath)
        {
            fixed (Guid* pId = &id)
            {
                nint pv = default;
                Marshal.ThrowExceptionForHR(ObjRef5.Vftbl.MakeNotifyIcon(ObjRef5.ThisPtr, pIconPath, pId, (HutaoNativeNotifyIcon.Vftbl**)&pv));
                return new(ObjectReference<HutaoNativeNotifyIcon.Vftbl>.Attach(ref pv, typeof(HutaoNativeNotifyIcon.Vftbl).GUID));
            }
        }
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNative)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeLoopbackSupport.Vftbl**, HRESULT> MakeLoopbackSupport;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HutaoNativeRegistryNotification.Vftbl**, HRESULT> MakeRegistryNotification;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, HutaoNativeWindowSubclassCallback, nint, HutaoNativeWindowSubclass.Vftbl**, HRESULT> MakeWindowSubclass;
#pragma warning restore CS0649
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNative2)]
    private readonly struct Vftbl2
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeDeviceCapabilities.Vftbl**, HRESULT> MakeLoopbackSupport;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativePhysicalDrive.Vftbl**, HRESULT> MakePhysicalDrive;
#pragma warning restore CS0649
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNative3)]
    private readonly struct Vftbl3
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeInputLowLevelKeyboardSource.Vftbl**, HRESULT> MakeInputLowLevelKeyboardSource;
#pragma warning restore CS0649
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNative4)]
    private readonly struct Vftbl4
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeFileSystem.Vftbl**, HRESULT> MakeFileSystem;
#pragma warning restore CS0649
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNative5)]
    private readonly struct Vftbl5
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, Guid*, HutaoNativeNotifyIcon.Vftbl**, HRESULT> MakeNotifyIcon;
#pragma warning restore CS0649
    }
}