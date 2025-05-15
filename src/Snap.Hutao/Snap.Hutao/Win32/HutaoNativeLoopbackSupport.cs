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

    public HutaoNativeLoopbackSupport(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public BOOL IsEnabled(ReadOnlySpan<char> familyName, out ReadOnlySpan<char> sid)
    {
        fixed (char* pFamilyName = familyName)
        {
            PWSTR pSid = default;
            BOOL enabled = default;
            Marshal.ThrowExceptionForHR(objRef.Vftbl.IsEnabled(objRef.ThisPtr, pFamilyName, &pSid, &enabled));
            sid = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pSid);
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

    [Guid("8607ace4-313c-4c26-b1fb-ca11173b6953")]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PWSTR*, BOOL*, HRESULT> IsEnabled;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> Enable;
#pragma warning restore CS0649
    }
}