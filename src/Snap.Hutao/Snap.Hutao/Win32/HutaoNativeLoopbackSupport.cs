// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativeLoopbackSupport
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoNativeLoopbackSupport(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public BOOL IsEnabled(ReadOnlySpan<char> familyName, out string? sid)
    {
        fixed (char* pFamilyName = familyName)
        {
            nint pSid = default;
            BOOL enabled = default;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.IsEnabled(objRef.ThisPtr, pFamilyName, (HutaoString.Vftbl**)&pSid, &enabled));
            sid = HutaoString.AttachAbi(ref pSid).Get();
            return enabled;
        }
    }

    public void Enable(ReadOnlySpan<char> sid)
    {
        fixed (char* pSid = sid)
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.Enable(objRef.ThisPtr, pSid));
        }
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativeLoopbackSupport)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HutaoString.Vftbl**, BOOL*, HRESULT> IsEnabled;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> Enable;
#pragma warning restore CS0649
    }
}