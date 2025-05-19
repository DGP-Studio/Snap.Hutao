// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativeNotifyIcon
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoNativeNotifyIcon(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public void Create(HutaoNativeNotifyIconCallback callback, nint userData, ReadOnlySpan<char> tip)
    {
        fixed (char* pTip = tip)
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.Create(objRef.ThisPtr, callback, userData, pTip));
        }
    }

    public void Recreate(ReadOnlySpan<char> tip)
    {
        fixed (char* pTip = tip)
        {
            Marshal.ThrowExceptionForHR(objRef.Vftbl.Recreate(objRef.ThisPtr, pTip));
        }
    }

    public void Destroy()
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Destroy(objRef.ThisPtr));
    }

    public BOOL IsPromoted()
    {
        BOOL promoted = default;
        Marshal.ThrowExceptionForHR(objRef.Vftbl.IsPromoted(objRef.ThisPtr, &promoted));
        return promoted;
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativeNotifyIcon)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HutaoNativeNotifyIconCallback, nint, PCWSTR, HRESULT> Create;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> Recreate;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Destroy;
        internal readonly delegate* unmanaged[Stdcall]<nint, BOOL*, HRESULT> IsPromoted;
#pragma warning restore CS0649
    }
}