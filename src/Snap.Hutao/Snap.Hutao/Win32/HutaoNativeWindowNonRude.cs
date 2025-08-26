// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32;

internal sealed unsafe class HutaoNativeWindowNonRude
{
    private readonly ObjectReference<Vftbl> objRef;

    public HutaoNativeWindowNonRude(ObjectReference<Vftbl> objRef)
    {
        this.objRef = objRef;
    }

    public void Attach()
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Attach(objRef.ThisPtr));
    }

    public void Detach()
    {
        Marshal.ThrowExceptionForHR(objRef.Vftbl.Detach(objRef.ThisPtr));
    }

    [Guid(HutaoNativeMethods.IID_IHutaoNativeWindowNonRude)]
    internal readonly struct Vftbl
    {
#pragma warning disable CS0649
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Attach;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Detach;
#pragma warning restore CS0649
    }
}