// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal static unsafe class HutaoNative
{
    public static ref readonly Guid IIDHutaoNative
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xFF, 0x73, 0x0F, 0xD0, 0xC7, 0xA1, 0x91, 0x40, 0x8C, 0xB6, 0xD9, 0x09, 0x91, 0xDD, 0x40, 0xCB]);
    }

    public static ref readonly Guid IIDHutaoNativeLoopbackSupport
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xE4, 0xAC, 0x07, 0x86, 0x3C, 0x31, 0x26, 0x4C, 0xB1, 0xFB, 0xCA, 0x11, 0x17, 0x3B, 0x69, 0x53]);
    }

    [field: MaybeNull]
    public static ObjectReference<IHutaoNative> Instance
    {
        get => LazyInitializer.EnsureInitialized(ref field, () =>
        {
            HutaoCreateInstance(out ObjectReference<IHutaoNative> result);
            return result;
        });
    }

    [DllImport("Snap.Hutao.Native.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    public static extern HRESULT HutaoCreateInstance(IHutaoNative** ppv);

    public static HRESULT HutaoCreateInstance(out ObjectReference<IHutaoNative> v)
    {
        nint pv = default;
        HRESULT hr = HutaoCreateInstance((IHutaoNative**)&pv);
        v = ObjectReference<IHutaoNative>.Attach(ref pv, IIDHutaoNative);
        return hr;
    }

    public static HRESULT MakeLoopbackSupport(this ObjectReference<IHutaoNative> objRef, out ObjectReference<IHutaoNativeLoopbackSupport> v)
    {
        nint pv = default;
        HRESULT hr = objRef.Vftbl.MakeLoopbackSupport(objRef.ThisPtr, (IHutaoNativeLoopbackSupport**)&pv);
        v = ObjectReference<IHutaoNativeLoopbackSupport>.Attach(ref pv, IIDHutaoNativeLoopbackSupport);
        return hr;
    }

    public static HRESULT IsEnabled(this ObjectReference<IHutaoNativeLoopbackSupport> objRef, ReadOnlySpan<char> familyName, out ReadOnlySpan<char> sid, out BOOL enabled)
    {
        fixed (char* pFamilyName = familyName)
        {
            PWSTR pSid = default;
            fixed (BOOL* pEnabled = &enabled)
            {
                HRESULT hr = objRef.Vftbl.IsEnabled(objRef.ThisPtr, pFamilyName, &pSid, pEnabled);
                sid = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pSid);
                return hr;
            }
        }
    }

    public static HRESULT Enable(this ObjectReference<IHutaoNativeLoopbackSupport> objRef, ReadOnlySpan<char> sid)
    {
        fixed (char* pSid = sid)
        {
            return objRef.Vftbl.Enable(objRef.ThisPtr, pSid);
        }
    }

    internal readonly struct IHutaoNative
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, IHutaoNativeLoopbackSupport**, HRESULT> MakeLoopbackSupport;
    }

    internal readonly struct IHutaoNativeLoopbackSupport
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PWSTR*, BOOL*, HRESULT> IsEnabled;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> Enable;
    }
}