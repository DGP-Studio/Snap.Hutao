// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativeLoopbackSupport
{
    private readonly ObjectReference<Vftbl> objRef;
    private readonly ObjectReference<Vftbl2>? objRef2;

    public HutaoNativeLoopbackSupport(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
        objRef.TryAs(typeof(Vftbl2).GUID, out objRef2);
    }

    public BOOL IsPublicFirewallEnabled
    {
        get
        {
            HutaoException.NotSupportedIf(objRef2 is null, "IHutaoNativeLoopbackSupport2 is not supported");

            BOOL isEnabled = default;
            Marshal.ThrowExceptionForHR(objRef2.Vftbl.IsPublicFirewallEnabled(objRef2.ThisPtr, &isEnabled));
            return isEnabled;
        }
    }

    public BOOL IsEnabled(ReadOnlySpan<char> familyName, out string? sid)
    {
        fixed (char* pFamilyName = familyName)
        {
            nint pSid = default;
            BOOL enabled = default;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.IsEnabled(objRef.ThisPtr, pFamilyName, (HutaoString.Vftbl**)&pSid, &enabled));
            sid = HutaoString.AttachAbi(ref pSid).Value;
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

    [Guid(HutaoNativeMethods.IID_IHutaoNativeLoopbackSupport2)]
    internal readonly struct Vftbl2
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL*, HRESULT> IsPublicFirewallEnabled;
#pragma warning restore CS0649
    }
}