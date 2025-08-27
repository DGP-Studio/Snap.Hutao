// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using System.Buffers;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNative
{
    private readonly ObjectReference<VftblPrivate>? objRefPrivate;
    private readonly ObjectReference<VftblPrivate2>? objRefPrivate2;
    private readonly ObjectReference<Vftbl2>? objRef2;
    private readonly ObjectReference<Vftbl3>? objRef3;
    private readonly ObjectReference<Vftbl4>? objRef4;
    private readonly ObjectReference<Vftbl5>? objRef5;
    private readonly ObjectReference<Vftbl6>? objRef6;
    private readonly ObjectReference<Vftbl7>? objRef7;

    public HutaoNative(ObjectReference<Vftbl> objRef)
    {
        ObjRef = objRef;
        objRef.TryAs(typeof(VftblPrivate).GUID, out objRefPrivate);
        objRef.TryAs(typeof(VftblPrivate2).GUID, out objRefPrivate2);
        objRef.TryAs(typeof(Vftbl2).GUID, out objRef2);
        objRef.TryAs(typeof(Vftbl3).GUID, out objRef3);
        objRef.TryAs(typeof(Vftbl4).GUID, out objRef4);
        objRef.TryAs(typeof(Vftbl5).GUID, out objRef5);
        objRef.TryAs(typeof(Vftbl6).GUID, out objRef6);
        objRef.TryAs(typeof(Vftbl7).GUID, out objRef7);
    }

    [field: MaybeNull]
    public static HutaoNative Instance
    {
        get => LazyInitializer.EnsureInitialized(ref field, static () =>
        {
            HutaoNativeWilCallbacks.HutaoInitializeWilCallbacks();
            return HutaoNativeMethods.HutaoCreateInstance();
        });
    }

    private ObjectReference<Vftbl> ObjRef { get; }

    private ObjectReference<VftblPrivate>? ObjRefPrivate { get => objRefPrivate; }

    private ObjectReference<VftblPrivate2>? ObjRefPrivate2 { get => objRefPrivate2; }

    private ObjectReference<Vftbl2>? ObjRef2 { get => objRef2; }

    private ObjectReference<Vftbl3>? ObjRef3 { get => objRef3; }

    private ObjectReference<Vftbl4>? ObjRef4 { get => objRef4; }

    private ObjectReference<Vftbl5>? ObjRef5 { get => objRef5; }

    private ObjectReference<Vftbl6>? ObjRef6 { get => objRef6; }

    private ObjectReference<Vftbl7>? ObjRef7 { get => objRef7; }

    public static BOOL IsWin32(HRESULT hr, WIN32_ERROR error)
    {
        return HutaoNativeMethods.IsWin32(hr, error);
    }

    public static BOOL IsWin32(HRESULT hr, ReadOnlySpan<WIN32_ERROR> errors)
    {
        foreach (ref readonly WIN32_ERROR error in errors)
        {
            if (HutaoNativeMethods.IsWin32(hr, error))
            {
                return true;
            }
        }

        return false;
    }

    public BOOL IsCurrentWindowsVersionSupported()
    {
        HutaoException.NotSupportedIf(ObjRefPrivate is null, "IHutaoPrivate is not supported");

        BOOL isSupported = default;
        Marshal.ThrowExceptionForHR(ObjRefPrivate.Vftbl.IsCurrentWindowsVersionSupported(ObjRefPrivate.ThisPtr, &isSupported));
        return isSupported;
    }

    public HutaoPrivateWindowsVersion GetCurrentWindowsVersion()
    {
        HutaoException.NotSupportedIf(ObjRefPrivate is null, "IHutaoPrivate is not supported");

        HutaoPrivateWindowsVersion version = default;
        Marshal.ThrowExceptionForHR(ObjRefPrivate.Vftbl.GetWindowsVersion(ObjRefPrivate.ThisPtr, &version));
        return version;
    }

    public void ShowErrorMessage(ReadOnlySpan<char> title, ReadOnlySpan<char> message)
    {
        HutaoException.NotSupportedIf(ObjRefPrivate is null, "IHutaoPrivate is not supported");

        fixed (char* pTitle = title)
        {
            fixed (char* pMessage = message)
            {
                Marshal.ThrowExceptionForHR(ObjRefPrivate.Vftbl.ShowErrorMessage(ObjRefPrivate.ThisPtr, pTitle, pMessage));
            }
        }
    }

    public string ExchangeGameUidForIdentifier1820(ReadOnlySpan<char> gameUid)
    {
        if (gameUid.IsEmpty)
        {
            return string.Empty;
        }

        HutaoException.NotSupportedIf(ObjRefPrivate2 is null, "IHutaoPrivate2 is not supported");

        fixed (char* pGameUid = gameUid)
        {
            byte[] data = ArrayPool<byte>.Shared.Rent(gameUid.Length * 2);
            try
            {
                fixed (byte* identifier = data)
                {
                    Marshal.ThrowExceptionForHR(ObjRefPrivate2.Vftbl.ExchangeGameUidForIdentifier1820(ObjRefPrivate2.ThisPtr, pGameUid, identifier));
                    return Convert.ToBase64String(data.AsSpan(0, gameUid.Length * 2));
                }
            }
            catch (Exception ex)
            {
                ex.Data["GameUid"] = gameUid.ToString();
                throw;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
            }
        }
    }

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

    public HutaoNativeWindowNonRude MakeWindowNonRude(HWND hWnd)
    {
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef.Vftbl.MakeWindowNonRude(ObjRef.ThisPtr, hWnd, (HutaoNativeWindowNonRude.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativeWindowNonRude.Vftbl>.Attach(ref pv, typeof(HutaoNativeWindowNonRude.Vftbl).GUID));
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

    public HutaoNativeLogicalDrive MakeLogicalDrive()
    {
        HutaoException.NotSupportedIf(ObjRef2 is null, "IHutaoNative2 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef2.Vftbl.MakeLogicalDrive(ObjRef2.ThisPtr, (HutaoNativeLogicalDrive.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativeLogicalDrive.Vftbl>.Attach(ref pv, typeof(HutaoNativeLogicalDrive.Vftbl).GUID));
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

    public HutaoNativeHotKeyAction MakeHotKeyAction(HutaoNativeHotKeyActionKind kind, HutaoNativeHotKeyActionCallback callback, nint userData)
    {
        HutaoException.NotSupportedIf(ObjRef6 is null, "IHutaoNative6 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef6.Vftbl.MakeHotKeyAction(ObjRef6.ThisPtr, kind, callback, userData, (HutaoNativeHotKeyAction.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativeHotKeyAction.Vftbl>.Attach(ref pv, typeof(HutaoNativeHotKeyAction.Vftbl).GUID));
    }

    public HutaoNativeProcess MakeProcess(HutaoNativeProcessStartInfo info)
    {
        HutaoException.NotSupportedIf(ObjRef7 is null, "IHutaoNative7 is not supported");
        nint pv = default;
        Marshal.ThrowExceptionForHR(ObjRef7.Vftbl.MakeProcess(ObjRef7.ThisPtr, info, (HutaoNativeProcess.Vftbl**)&pv));
        return new(ObjectReference<HutaoNativeProcess.Vftbl>.Attach(ref pv, typeof(HutaoNativeProcess.Vftbl).GUID));
    }

#pragma warning disable CS0649
    [Guid(HutaoNativeMethods.IID_IHutaoNative)]
    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeLoopbackSupport.Vftbl**, HRESULT> MakeLoopbackSupport;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HutaoNativeRegistryNotification.Vftbl**, HRESULT> MakeRegistryNotification;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, HutaoNativeWindowSubclassCallback, nint, HutaoNativeWindowSubclass.Vftbl**, HRESULT> MakeWindowSubclass;
        internal readonly delegate* unmanaged[Stdcall]<nint, HWND, HutaoNativeWindowNonRude.Vftbl**, HRESULT> MakeWindowNonRude;
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNative2)]
    private readonly struct Vftbl2
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeDeviceCapabilities.Vftbl**, HRESULT> MakeLoopbackSupport;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativePhysicalDrive.Vftbl**, HRESULT> MakePhysicalDrive;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeLogicalDrive.Vftbl**, HRESULT> MakeLogicalDrive;
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNative3)]
    private readonly struct Vftbl3
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeInputLowLevelKeyboardSource.Vftbl**, HRESULT> MakeInputLowLevelKeyboardSource;
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNative4)]
    private readonly struct Vftbl4
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeFileSystem.Vftbl**, HRESULT> MakeFileSystem;
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNative5)]
    private readonly struct Vftbl5
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, Guid*, HutaoNativeNotifyIcon.Vftbl**, HRESULT> MakeNotifyIcon;
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNative6)]
    private readonly struct Vftbl6
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeHotKeyActionKind, HutaoNativeHotKeyActionCallback, nint, HutaoNativeHotKeyAction.Vftbl**, HRESULT> MakeHotKeyAction;
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNative7)]
    private readonly struct Vftbl7
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeProcessStartInfo, HutaoNativeProcess.Vftbl**, HRESULT> MakeProcess;
    }

    [Guid(HutaoNativeMethods.IID_IHutaoPrivate)]
    private readonly struct VftblPrivate
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL*, HRESULT> IsCurrentWindowsVersionSupported;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoPrivateWindowsVersion*, HRESULT> GetWindowsVersion;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, HRESULT> ShowErrorMessage;
    }

    [Guid(HutaoNativeMethods.IID_IHutaoPrivate2)]
    private readonly struct VftblPrivate2
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, byte*, HRESULT> ExchangeGameUidForIdentifier1820;
    }
#pragma warning restore CS0649
}